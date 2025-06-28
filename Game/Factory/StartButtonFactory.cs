using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class StartButtonFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;
    private readonly StartButtonBehaviour _startButtonBehaviour;

    public StartButtonFactory(IContentProvider contentProvider, StartButtonBehaviour startButtonBehaviour)
    {
        _contentProvider = contentProvider;
        _startButtonBehaviour = startButtonBehaviour;
    }
    public GameObject Create()
    {
        GameObject startButton = new("StartButton", new(0, -30), 0, Vector2.One);

        Texture2D startButtonTexture = _contentProvider.GetTextureByType(TextureTypes.PlayButton);
        startButton
        .AddComponent(new CanvasHandler())
        .AddComponent(new Button(new Sprite(startButtonTexture), Color.White, Color.LightBlue, Color.LightGreen))
        .AddComponent(_startButtonBehaviour);
        return startButton;
    }
}
