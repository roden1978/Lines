using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class RestartButtonFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;
    private readonly RestartButtonBehaviour _restartButtonBehaviour;

    public RestartButtonFactory(IContentProvider contentProvider, RestartButtonBehaviour restartButtonBehaviour)
    {
        _contentProvider = contentProvider;
        _restartButtonBehaviour = restartButtonBehaviour;
    }
    public GameObject Create()
    {
        GameObject restartButton = new("RestartButton", new(0, 30), 0, new(.7f, .7f));

        Texture2D restartButtonTexture = _contentProvider.GetTextureByType(TextureTypes.RestartButton);
        restartButton
        .AddComponent(new CanvasHandler())
        .AddComponent(new Button(new Sprite(restartButtonTexture), Color.White, Color.LightBlue, Color.LightGreen)
        {
            Interactable = false
        })
        .AddComponent(_restartButtonBehaviour);
        return restartButton;
    }
}
