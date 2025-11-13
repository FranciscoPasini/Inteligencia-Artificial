using UnityEngine;

public class NPCIdleState : IState
{
    private NPCTree npc;
    private float timer;

    public NPCIdleState(NPCTree npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        timer = 0f;
        Debug.Log($"{npc.name}: Enter Idle");
    }

    public void Execute()
    {
        timer += Time.deltaTime;

        // Si ve al player, cambiar a Attack inmediatamente
        if (npc.IsPlayerInSight())
        {
            npc.ChangeState(npc.AttackState);
            return;
        }

        if (timer >= npc.idleDuration)
        {
            npc.ChangeState(npc.PatrolState);
            return;
        }
    }

    public void Exit()
    {
        // reset timer handled on Enter next time
    }
}
