using Microsoft.Xna.Framework;

public class NextLabelFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;

    public NextLabelFactory(IContentProvider contentProvider)
    {
        _contentProvider = contentProvider; 
    }
    public GameObject Create()
    {
        GameObject nextLabelDrawer = new("NextLabelDrawer", new(0, -250), 0, new(.5f, .5f));

        TextDrawer textDrawer = new (_contentProvider.FontSequence)
        {
            Text = "NEXT",
            TextColor = Color.Chocolate
        };
        nextLabelDrawer
        .AddComponent(new CanvasHandler())
        .AddComponent(textDrawer);

        return nextLabelDrawer;
    }
}
