using UnityEngine;

public class Attack : IState
{
    private readonly Player player;
    private float time;
    float clipDuration;

    public Attack(Player _player)
    {
        player = _player;
    }

    public void OnEnter()
    {
        time = Time.time;
        player.sword.shouldCheck = true;
        player.canAttack = false;
        var index = Random.Range(0, player.ATTACKS.Length);
        clipDuration = player.ATTACKS_DURATION[index];
        player.PlayAnimation(player.ATTACKS[index]);
    }

    public void OnExit()
    {
        player.isInteracting = false;
        player.canAttack = true;
        player.hasPressedMeleeAttackButton = false;
        player.hasPressedJumpButton = false;
        player.sword.shouldCheck = true;
    }

    public void Tick()
    {
        if (Time.time > time + clipDuration)
            player.canAttack = true;
    }
}
