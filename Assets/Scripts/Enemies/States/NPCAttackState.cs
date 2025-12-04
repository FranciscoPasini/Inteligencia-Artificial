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
        // Acciones puras
        ActionNode flee = new ActionNode(StartFleeAction);
        ActionNode pursuit = new ActionNode(StartPursuitAction);

        // Ataque con posibilidad de pánico
        ActionNode attack = new ActionNode(AttackWithPanicChance);

        //  DECISIONES 

        // 1. ¿Está en rango de ataque?
        QuestionNode inAttackRange = new QuestionNode(() =>
            npc.target != null &&
            Vector3.Distance(npc.transform.position, npc.target.transform.position) < npc.attackRange,
            attack, pursuit);

        // 2. ¿Es cobarde? Siempre huye
        QuestionNode isCoward = new QuestionNode(() =>
            npc.enemyType == NPCTree.EnemyType.Coward,
            flee, inAttackRange);

        // ÚNICA raíz
        rootNode = isCoward;
    }

    public void Enter()
    {
        Debug.Log($"{npc.name}: Enter Attack");
        npc.isSearching = false;
    }

    public void Execute()
    {
        // jugador a la vista  árbol
        if (npc.IsPlayerInSight())
        {
            npc.lastSeenPosition = npc.target.transform.position;
            rootNode.Execute();
            return;
        }

        // jugador NO visible  búsqueda multinodo
        if (!npc.isSearching)
            npc.StartNodeSearch();

        int searchState = npc.UpdateNodeSearch();

        if (searchState == 1)
        {
            // Volvió a verlo  árbol lo maneja
            return;
        }
        else if (searchState == 0)
        {
            // No lo encontró en el tiempo  patrulla
            npc.ChangeState(npc.PatrolState);
            return;
        }
    }

    public void Exit()
    {
        Debug.Log($"{npc.name}: Exit Attack");
    }


    // ===================
    // ACTION WRAPPERS
    // ===================

    private void AttackWithPanicChance()
    {
        // Solo agresivos tienen chance de entrar en pánico
        if (npc.enemyType == NPCTree.EnemyType.Aggressive)
        {
            // 50% atacar / 50% huir
            float[] weights = { 0.5f, 0.5f };
            int result = RouletteWheel.Select(weights);

            if (result == 1)
            {
                Debug.Log($"{npc.name}: Entra en pánico y huye!");
                StartFleeAction();
                return;
            }
        }

        npc.Attack(); // ataque normal
    }

    private void StartFleeAction()
    {
        npc.isSearching = false;
        npc.Flee();
    }

    private void StartPursuitAction()
    {
        if (npc.IsPlayerInSight())
            npc.Persuit();
        else
            npc.PersuitWithPathfinding();
    }
}
