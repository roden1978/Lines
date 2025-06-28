using Microsoft.Xna.Framework;

public class ScoreTextFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;
    private readonly IScoreProvider _scoreProvider;

    public ScoreTextFactory(IContentProvider contentProvider, IScoreProvider scoreProvider)
    {
        _contentProvider = contentProvider;
        _scoreProvider = scoreProvider;
    }
    public GameObject Create()
    {
        GameObject scoreTextDrawer = new("ScoreTextDrawer", new(0, -120), 0, Vector2.One)
        {
            Tag = Tags.ScoreTextDrawer
        };
        TextDrawer textDrawer = new (_contentProvider.FontSequence, _scoreProvider.StringSource)
        {
            Text = "0",
            TextColor = Color.Blue
        };
        scoreTextDrawer
            .AddComponent(new CanvasHandler())
            .AddComponent(textDrawer);

        return scoreTextDrawer;
    }
}
