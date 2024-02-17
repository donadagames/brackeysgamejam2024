using UnityEngine;

public class Interact : IState
{
    private readonly Player player;
    private float time;

    

    public Interact(Player _player)
    {
        player = _player;
    }

    public void OnEnter()
    {
        time = Time.time;
        player.sword.shouldCheck = false;
        player.canAttack = false;
        player.canJump = false;
        player.PlayAnimation(player.CHUTE);
    }

    public void OnExit()
    {
        player.isInteracting = false;
        player.canAttack = true;
        player.hasPressedMeleeAttackButton = false;
        player.hasPressedJumpButton = false;
        player.hasPressedInteractionButton = false;
    }

    public void Tick()
    {
        player.SearchForEnemySpawner();


        if (Time.time > time + player.CHUTE_DURATION)
            player.isInteracting = false;
    }
}
