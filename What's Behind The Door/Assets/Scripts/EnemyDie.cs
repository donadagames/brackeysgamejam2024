using UnityEngine;

public class EnemyDie : IState
{
    private readonly EnemySpawner spawner;

    private float time;
    private bool shouldCheck = true;

    private float clipDuration;

    public EnemyDie(EnemySpawner _spawner)
    {
        spawner = _spawner;
    }

    public void OnEnter()
    {
        time = Time.time;
        spawner.shouldSpawn = false;

        var index = Random.Range(0, spawner.enemy.DIE.Length);
        clipDuration = spawner.enemy.dieClipTime[index];
        spawner.enemy.animator.Play(spawner.enemy.DIE[index]);
        spawner.enemy.rb.isKinematic = true;
        spawner.enemy._collider.isTrigger = true;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        if (Time.time > time + clipDuration && shouldCheck)
        {
            shouldCheck = false;

            if (spawner.enemy.isOrcBoss)
            {
                spawner.player.AddKey();
            }

            if (spawner.enemy.isFinalBoss)
            {
                spawner.enemy.finalDoor.OpenDor();
            }
        }
    }

   
}
