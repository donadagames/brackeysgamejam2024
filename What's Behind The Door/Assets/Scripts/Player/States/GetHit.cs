using UnityEngine;

public class GetHit : IState
{
    private readonly Player player;
    private float time;

    public GetHit(Player _player)
    {
        player = _player;
    }


    public void OnEnter()
    {
        time = Time.time;
        player.sword.shouldCheck = false;
        player.canAttack = false;
        player.PlayAnimation(player.GETHIT);
    }

    public void OnExit()
    {
        player.isInteracting = false;
        player.canAttack = true;
    }

    public void Tick()
    {
        if (Time.time > time + player.GETHIT_DURATION)
            player.canAttack = true;
    }
}
