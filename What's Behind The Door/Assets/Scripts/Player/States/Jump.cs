using UnityEngine;

public class Jump : IState
{
    private readonly Player player;
    private float time;

    public Jump(Player _player)
    {
        player = _player;
    }


    public void OnEnter()
    {
        time = Time.time;
        player.sword.shouldCheck = false;
        player.canAttack = false;
        player.canJump = false;
        player.hasPressedMeleeAttackButton = false;
        player.hasPressedJumpButton = false;
        player.PlayAnimation(player.JUMP);
        player.Jump();

    }

    public void OnExit()
    {
        player.isInteracting = false;
        player.canAttack = true;
        player.hasPressedMeleeAttackButton = false;
        player.hasPressedJumpButton = false;
    }

    public void Tick()
    {
        player.GetInput();
        player.ApplyAllMovement();

        if (Time.time > time + player.JUMP_DURATION)
            player.canJump = true;
    }
}
