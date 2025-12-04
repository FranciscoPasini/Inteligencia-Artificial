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

        // --- ROULETTE WHEEL PARA DEFINIR DURACIÓN DEL IDLE ---
        float[] weights = { 0.5f, 0.5f };

        int option = RouletteWheel.Select(weights);

        switch (option)
        {
            case 0:
                npc.CurrentIdleDuration = npc.idleDuration;        // idle normal
                break;

            case 1:
                npc.CurrentIdleDuration = npc.idleDuration * 2.5f; // idle medio (ej: 5s si idle=2)
                break;
        }

        Debug.Log($"{npc.name} IdleDuration set to {npc.CurrentIdleDuration}");
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

        if (timer >= npc.CurrentIdleDuration)
        {
            npc.ChangeState(npc.PatrolState);
            return;
        }
    }

    public void Exit()
    {
    }
}
