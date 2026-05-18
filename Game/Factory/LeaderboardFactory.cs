using Microsoft.Xna.Framework;

public sealed class LeaderboardFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;

    private readonly ILeaderboardService _leaderboardService;
    private readonly IContentProvider _contentProvider;
    private const int StartPositionY = 60;
    private const int PositionY = 80;
    private const int StepY = 35;

    public LeaderboardFactory(ILeaderboardService leaderboardService, IContentProvider contentProvider)
    {
        _leaderboardService = leaderboardService;
        _contentProvider = contentProvider;
    }
    public GameObject Create()
    {
        GameObject leaderBoard = new("Leaderboard", new(0, PositionY), 0, Vector2.One);
        leaderBoard.AddComponent(new CanvasHandler());
        CreateLeaderboardTiltle(leaderBoard.Transform);
        CreateLeaderboard(leaderBoard.Transform);

        return leaderBoard;
    }
    public void CreateLeaderboard(Transform2D parent)
    {
        int index = 0;
        foreach (StringSource item in _leaderboardService.Leaders)
        {
            GameObject row = new($"Row{index}");

            TextDrawer textDrawer = new(_contentProvider.FontSequence, item)
            {
                TextColor = Color.Gray
            };

            if (index == 0)
            {
                row.Transform.Scale = new Vector2(.8f, .9f);
                row.Transform.Parent = parent;
                textDrawer.TextColor = Color.Green;
            }

            row.Transform.Position = new(0, StartPositionY + StepY * index);
            row.Transform.Scale = new Vector2(.6f, .7f);
            row.Transform.Parent = parent;
            row.AddComponent(textDrawer);
            index ++;
        }
    }

    private void CreateLeaderboardTiltle(Transform2D parent)
    {
        GameObject leaderboardTitle = new("LeaderboardTitle");
        TextDrawer textDrawer = new(_contentProvider.FontSequence)
        {
            Text = "TOP 5",
            TextColor = Color.Red
        };
        leaderboardTitle.AddComponent(textDrawer);
        leaderboardTitle.Transform.Parent = parent;
    }
}

