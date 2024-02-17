using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Enemy enemy;
    public bool shouldSpawn = true;
    public Player player;
    public float chaseDistance = 6;
    public StateMachine stateMachine;
    public bool canAttack = true;
    private EnemyIdle idle;
    private EnemyChase chase;
    public EnemyAttackPosition enemyPosition;
    public Transform playerTarget;

    private void Awake()
    {
        shouldSpawn = false;

        stateMachine = new StateMachine();
        idle = new EnemyIdle(this);
        chase = new EnemyChase(this);
        var attack = new EnemyAttack(this);
        EnemyDie die = new EnemyDie(this);
        var getHit = new EnemyGetHit(this);

        void AddTransition(IState from, IState to,
            Func<bool> condition) =>
            stateMachine.AddTransition
            ((IState)from, (IState)to, condition);

        AddTransition
            (idle, chase, IsCloseToPlayerToChase());

        AddTransition
            (chase, attack, IsCloseToPlayerToAttack());
        AddTransition
           (chase, idle, IsFarAwayFromPlayer());

        AddTransition
            (attack, chase, IsCloseToPlayerToChaseAfterAttack());

        AddTransition(getHit, chase, EndGetHitAnimation());

        stateMachine.AddAnyTransition
            (die, () => !enemy.isAlive && player.isAlive);
        stateMachine.AddAnyTransition
            (getHit, () => enemy.isAlive && enemy.isDamaged && player.isAlive);

        stateMachine.AddAnyTransition
               (idle, () => !player.isAlive);


        Func<bool> IsFarAwayFromPlayer() => () => enemy.distance > chaseDistance + .5f && player.isAlive;
        Func<bool> IsCloseToPlayerToAttack() => () => canAttack && enemy.distance < enemy.distanceToAttack && player.isAlive;
        Func<bool> IsCloseToPlayerToChase() => () => enemy.distance < chaseDistance && enemy.distance > enemy.distanceToAttack && player.isAlive;
        Func<bool> IsCloseToPlayerToChaseAfterAttack() => () => canAttack && player.isAlive;
        Func<bool> EndGetHitAnimation() => () => !enemy.isDamaged && enemy.isAlive && player.isAlive;
    }

    public void ResetSpawner()
    {
        if(enemy != null)
        {
            Destroy(enemy.gameObject);
        }

        shouldSpawn = false;
    }

    private void Start()
    {
        player = Player.instance;
    }

    public void SpawEnemy()
    {
        if (shouldSpawn && enemy == null)
        {
            shouldSpawn = false;
            var _enemy =
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemy = _enemy.GetComponent<Enemy>();
            enemy.spawner = this;
            enemyPosition = player.GetEnemyPosition();

            if (enemyPosition == null)
            {
                playerTarget = player.transform;
            }
            else
            {
                playerTarget = enemyPosition.transform;
            }

            stateMachine.SetState(idle);
        }

        else return;
    }

    public void ResetEnemyPositionIndex()
    {
        if (enemyPosition != null)
            enemyPosition.isInUse = false;
    }

    public void DeathVFX()
    {
        //Instantiate(enemy.death_VFX, enemy.transform.position + new Vector3(0, .8f, 0), Quaternion.AngleAxis(-90, Vector3.left));
        // Destroy(enemy.gameObject);
    }
}
