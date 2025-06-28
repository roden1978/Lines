using Microsoft.Xna.Framework;

public class TitleFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;

    public TitleFactory(IContentProvider contentProvider)
    {
        _contentProvider = contentProvider; 
    }
    public GameObject Create()
    {
        GameObject titleDrawer = new("TitleDrawer", new(0, -330), 0, new(1.5f, 1.5f));

        TextDrawer textDrawer = new (_contentProvider.FontSequence)
        {
            Text = "LINES",
            TextColor = Color.Green
        };
        titleDrawer
            .AddComponent(new CanvasHandler())
            .AddComponent(textDrawer);

        return titleDrawer;
    }
}
