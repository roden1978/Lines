using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GamePanelFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;
    private readonly GameBehaviour _gameBehaviour;
    private readonly RandomTileAnimationBehavoiur _randomTileAnimationBehavoiur;

    public GamePanelFactory(IContentProvider contentProvider, GameBehaviour gameBehaviour, RandomTileAnimationBehavoiur randomTileAnimationBehavoiur)
    {
        _contentProvider = contentProvider;
        _gameBehaviour = gameBehaviour;
        _randomTileAnimationBehavoiur = randomTileAnimationBehavoiur;
    }
    public GameObject Create()
    {
        GameObject gamePanel = new($"GamePanel", new Vector2(360, 360), 0, Vector2.One);
        gamePanel.AddComponent(new CanvasHandler());
        
        CreateBackgroundShadow(gamePanel.Transform);
        CreateBackground(gamePanel.Transform);
        
        GameObject tilesHolder = new("TilesHolder");
        tilesHolder.Transform.Parent = gamePanel.Transform;
        tilesHolder
            .AddComponent(new CanvasHandler())
            .AddComponent(new GridLayoutGroup(Settings.TilesCount))
            .AddComponent(_gameBehaviour)
            .AddComponent(_randomTileAnimationBehavoiur);

        return gamePanel;
    }
    private GameObject CreateBackground(Transform2D parent)
    {
        Texture2D backgroundTexture = _contentProvider.GetTextureByType(TextureTypes.GamePanel);
        GameObject background = new("GamePanelBackground");
        background.Transform.Parent = parent;
        background
        .AddComponent(new CanvasHandler())
        .AddComponent(new UIImage(new Sprite(backgroundTexture), 1f){
            Color = Color.DarkSlateGray
        });

        return background;
    }

    private GameObject CreateBackgroundShadow(Transform2D parent)
    {
        Texture2D backgroundTexture = _contentProvider.GetTextureByType(TextureTypes.GamePanel);
        GameObject shadow = new("GamePanelShadow", new(5, 5), 0, Vector2.One)
        {
            Name = "GamePanelShadow"
        };

        shadow.Transform.Parent = parent;
        shadow
        .AddComponent(new CanvasHandler())
        .AddComponent(new UIImage(new Sprite(backgroundTexture), .7f)
        {
            Color = Color.Black
        });

        return shadow;
    }   
}
