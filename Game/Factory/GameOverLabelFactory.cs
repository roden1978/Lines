using Microsoft.Xna.Framework;

public class GameOverLabelFactory : IFactory<GameObject>
{
    private readonly IContentProvider _contentProvider;
    private readonly GameOverLabelBehaviour _gameOverLabelBehaviour;

    public GameOverLabelFactory(IContentProvider contentProvider, GameOverLabelBehaviour gameOverLabelBehaviour)
    {
        _contentProvider = contentProvider;
        _gameOverLabelBehaviour = gameOverLabelBehaviour; 
    }

    public string Name => GetType().Name;

    public GameObject Create()
    {
        GameObject gameOverLabelDrawer = new("GameOverLabelDrawer", new(0, 30), 0, Vector2.One);

        TextDrawer textDrawer = new (_contentProvider.FontSequence)
        {
            Text = "GAME OVER",
            TextColor = Color.Red
        };
        
        gameOverLabelDrawer
            .AddComponent(new CanvasHandler())
            .AddComponent(textDrawer)
            .AddComponent(_gameOverLabelBehaviour);

        gameOverLabelDrawer.SetActive(false);
        return gameOverLabelDrawer;
    }
}
