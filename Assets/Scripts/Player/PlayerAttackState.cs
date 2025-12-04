using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float attackDuration = 0.3f;
    private float timer;

    public PlayerAttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        timer = attackDuration;

        player.PlayAttackEffect();
        player.PerformRadialAttack();
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
            player.ChangeState(new PlayerIdleState(player));
    }

    public override void Exit() { }
}
