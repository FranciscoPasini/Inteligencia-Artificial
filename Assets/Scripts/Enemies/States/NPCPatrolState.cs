using UnityEngine;

public class NPCPatrolState : IState
{
    private NPCTree npc;
    private bool patrolForward = true;

    public NPCPatrolState(NPCTree npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        Debug.Log($"{npc.name}: Enter Patrol");
    }

    public void Execute()
    {
        // Si ve al player ? entrar a AttackState
        if (npc.IsPlayerInSight())
        {
            npc.ChangeState(npc.AttackState);
            return;
        }

        // Patrullar con arrive
        if (npc.waypoints == null || npc.waypoints.Length == 0)
            return;

        // Nos movemos hacia el waypoint actual
        npc.arrive.SetTarget = npc.waypoints[npc.currentWP];
        var steer = npc.arrive.GetSteerDir(npc.velocity);
        // Apply steering already encapsulated in NPCTree.Patrol (but aquí lo llamamos directo)
        npc.ApplySteering(steer);

        // Si llegó al waypoint:
        if (npc.IsAtWaypoint())
        {
            // Roulette Wheel: 50% Idle / 50% seguir patrullando
            float[] weights = { 0.5f, 0.5f };
            int choice = RouletteWheel.Select(weights);

            if (choice == 0)
            {
                npc.ChangeState(npc.IdleState);
                return;
            }
            else
            {
                // Avanzar al siguiente waypoint (manejamos ida y vuelta)
                if (patrolForward) npc.currentWP++;
                else npc.currentWP--;

                if (npc.currentWP >= npc.waypoints.Length)
                {
                    npc.currentWP = npc.waypoints.Length - 2;
                    patrolForward = false;
                }
                else if (npc.currentWP < 0)
                {
                    npc.currentWP = 1;
                    patrolForward = true;
                }
            }
        }
    }

    public void Exit()
    {
    }
}
