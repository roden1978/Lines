using Microsoft.Xna.Framework;

public class ScoreLabelFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;

    public ScoreLabelFactory(IContentProvider contentProvider)
    {
        _contentProvider = contentProvider; 
    }
    public GameObject Create()
    {
        GameObject scoreLabelDrawer = new("ScoreLabelDrawer", new(0, -150), 0, new(.5f, .5f));

        TextDrawer textDrawer = new (_contentProvider.FontSequence)
        {
            Text = "SCORES",
            TextColor = Color.Blue
        };
        scoreLabelDrawer
            .AddComponent(new CanvasHandler())
            .AddComponent(textDrawer);

        return scoreLabelDrawer;
    }
}
