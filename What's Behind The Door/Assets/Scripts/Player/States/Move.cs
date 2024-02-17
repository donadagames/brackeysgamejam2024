public class Move : IState
{
    private readonly Player player;

    public Move(Player _player)
    {
        player = _player;
    }

    public void OnEnter()
    {
        player.sword.shouldCheck = false;
        player.PlayAnimation(player.MOVE);
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        player.GetDirection();
        player.ApplyAllMovement();
        player.SearchForEnemySpawner();
        player.SearchForInteractables();
        player.DetectWater();
    }
}
