public class Idle : IState
{
    private readonly Player player;

    public Idle(Player _player)
    {
        player = _player;
    }

    public void OnEnter()
    {
        player.sword.shouldCheck = false;
        player.PlayAnimation(player.IDLE);
    }

    public void OnExit()
    {
        return;
    }

    public void Tick()
    {
        player.GetDirection();
        player.ApplyGravity();
        player.SearchForEnemySpawner();
    }
}
