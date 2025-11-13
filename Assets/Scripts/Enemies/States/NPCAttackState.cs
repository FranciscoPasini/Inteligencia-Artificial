using System;
using UnityEngine;

public class NPCAttackState : IState
{
    private NPCTree npc;
    private ITreeNode rootNode;

    public NPCAttackState(NPCTree npc)
    {
        this.npc = npc;
        BuildTree();
    }

    private void BuildTree()
    {
        // ActionNodes apuntando a los métodos del NPC
        ActionNode die = new ActionNode(npc.Die);
        ActionNode flee = new ActionNode(StartFleeAction);
        ActionNode pursuit = new ActionNode(StartPursuitAction);
        ActionNode attack = new ActionNode(npc.Attack);

        // Si está en rango de ataque -> Attack, sino Pursuit
        QuestionNode inAttackRange = new QuestionNode(() =>
            npc.target != null && Vector3.Distance(npc.transform.position, npc.target.transform.position) < npc.attackRange,
            attack, pursuit);

        // Si baja vida -> Flee, sino evaluar inAttackRange
        QuestionNode lowHealth = new QuestionNode(() =>
            npc.IsLowHealth(),
            flee, inAttackRange);

        // Si es cobarde -> huye directo, sino baja vida/inAttackRange
        QuestionNode behaviorType = new QuestionNode(() =>
            npc.enemyType == NPCTree.EnemyType.Coward,
            flee, lowHealth);

        // Si está muerto -> Die, sino behaviorType
        QuestionNode isAlive = new QuestionNode(() =>
            npc.health <= 0,
            die, behaviorType
        );

        rootNode = isAlive;
    }

    public void Enter()
    {
        Debug.Log($"{npc.name}: Enter Attack");
        // reseteamos búsqueda por si venimos de búsqueda
        npc.isSearching = false;
    }

    public void Execute()
    {
        if (npc.target == null)
        {
            npc.ChangeState(npc.PatrolState);
            return;
        }

        // Si ve al jugador: ejecutar árbol normal
        if (npc.IsPlayerInSight())
        {
            // Actualizar last seen
            npc.lastSeenPosition = npc.target.transform.position;
            rootNode.Execute();
            return;
        }

        // Si no lo ve: iniciar búsqueda con pathfinding por searchDuration
        if (!npc.isSearching)
        {
            npc.StartSearch(npc.lastSeenPosition, npc.searchDuration);
        }

        int searchState = npc.UpdateSearch(); // -1 buscando, 0 expiró, 1 encontró
        if (searchState == 1)
        {
            // lo encontró, volvemos a ejecutar árbol normalmente en el próximo frame
            return;
        }
        else if (searchState == 0)
        {
            // expiró la búsqueda
            npc.ChangeState(npc.PatrolState);
            return;
        }
        // si -1 => sigue buscando (UpdateSearch ya movió al NPC por los nodos)
    }

    public void Exit()
    {
        Debug.Log($"{npc.name}: Exit Attack");
    }

    // Helpers que envuelven las acciones para adaptar a ActionNode
    private void StartFleeAction()
    {
        // Cuando el árbol decide huir, empezamos la acción de huida normal (evade)
        npc.isSearching = false; // cancelar búsqueda si la hubiese
        // Flee se ejecutará en próximos frames mientras el estado sea Attack y el árbol siga retornando flee
        npc.Flee();
    }

    private void StartPursuitAction()
    {
        // Si está en rango de visión usamos persuit directo (steering)
        if (npc.IsPlayerInSight())
        {
            npc.Persuit();
        }
        else
        {
            // Si no lo ve, usamos persuit con pathfinding (search behavior)
            npc.PersuitWithPathfinding();
        }
    }
}
