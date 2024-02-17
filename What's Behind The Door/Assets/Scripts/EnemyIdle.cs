public class EnemyIdle : IState
{
    private readonly EnemySpawner spawner;

    public EnemyIdle(EnemySpawner _spawner)
    {
        spawner = _spawner;
    }

    public void OnEnter()
    {
        spawner.enemy.animator.Play(spawner.enemy.IDLE);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        spawner.enemy.GetPlayerDistance(spawner.player.transform);
        spawner.enemy.FaceTarget(spawner.player.transform);
    }
}
