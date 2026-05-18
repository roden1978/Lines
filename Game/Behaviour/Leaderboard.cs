public class Leaderboard : Component
{
    private readonly LeaderboardFactory _leaderboardFactory;
    private readonly Scene _scene;

    public Leaderboard(LeaderboardFactory leaderboardFactory, Scene scene)
    {
        _leaderboardFactory = leaderboardFactory;
        _scene = scene;
    }

    public void UpdateLeaders()
    {
        ClearLeaderboard();
        CreateLeaderboard();
    }
    private void ClearLeaderboard()
    {
        foreach(Transform2D item in gameObject.Transform.Childrens)
        {
            item.Parent = null;
            _scene.Unregister(item.Gameobject);
        }
    }

    private void CreateLeaderboard()
    {
        _leaderboardFactory.CreateLeaderboard(gameObject.Transform);
    }
}