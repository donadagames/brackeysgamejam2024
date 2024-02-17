using UnityEngine;

public class EnemyAttack : IState
{
    private readonly EnemySpawner spawner;
    private float attackClipDuration;
    private float time;

    public EnemyAttack(EnemySpawner _spawner)
    {
        spawner = _spawner;
    }

    public void OnEnter()
    {
        time = Time.time;
        spawner.canAttack = false;
        attackClipDuration = spawner.enemy.Attack();
    }

    public void OnExit()
    {
        spawner.canAttack = true;
    }

    public void Tick()
    {
        spawner.enemy.GetPlayerDistance(spawner.player.transform);
        spawner.enemy.FaceTarget(spawner.player.transform);

        if (spawner.player.isAlive == false)
        { 
            
        }

        if (Time.time > time + attackClipDuration)
        {
            spawner.canAttack = true;
        }
    }
}
