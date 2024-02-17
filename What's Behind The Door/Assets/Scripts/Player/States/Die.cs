using UnityEngine;

public class Die : IState
{
    private readonly Player player;
    private float time;

    public Die(Player _player)
    {
        player = _player;
    }

    public void OnEnter()
    {
        time = Time.time;
        player.sword.shouldCheck = false;
        player.canAttack = false;
        player.SetDefaultInteractionSprite();
        player.canJump = false;
        player.interactable = null;
        player.canMove = false; 
        player.isAlive = false;
        player.isDamaged = false;
        player.isInteracting = false;
        player.hasPressedInteractionButton = false;
        player.hasPressedJumpButton = false;
        player.hasPressedMeleeAttackButton = false;
        player.PlayAnimation(player.DIE);

        player.GetTimeCount();
    }

    public void OnExit()
    {
        player.sword.shouldCheck = true;
        player.canAttack = true;
        player.canJump = true;
        player.canMove = true;
        player.isAlive = true;
        player.isDamaged = false;
        player.isInteracting = false;
        player.hasPressedInteractionButton = false;
        player.hasPressedJumpButton = false;
        player.hasPressedMeleeAttackButton = false;
    }

    public void Tick()
    {
        if (Time.time > time + player.DIE_DURATION)
            player.isInteracting = false;
    }
}
