using Microsoft.Xna.Framework.Graphics;

public class SplashFactory : IFactory<GameObject>
{
    private readonly IContentProvider _contentProvider;
    private readonly IIdentifierService _identifierService;

    public string Name => "SpalshFactory";

    public SplashFactory(IContentProvider contentProvider, IIdentifierService identifierService)
    {
        _contentProvider = contentProvider;
        _identifierService = identifierService;
    }
     public GameObject Create()
    {
        Texture2D bubblePop = _contentProvider.GetTextureByType(TextureTypes.BubblePop);

        GameObject splash = new($"BubblePop {_identifierService.Next()}");
        Sequence sequence = new Sequence(new SpriteOrder(bubblePop, CalculateSpritesCount(bubblePop.Width, bubblePop.Height), 1));
        UIImage image = new UIImage(sequence[0]);
                                                            
        splash.AddComponent(new CanvasHandler())
                .AddComponent(new Splash())
                .AddComponent(image)
                .AddComponent(CreateSplashAnimator(image, sequence));
    
        return splash;
    }

    private static Animator CreateSplashAnimator(UIImage image, Sequence sequence)
    {
        return new Animator(image, new AnimationController([
                                                    new Animation(sequence,
                                                            20,
                                                            "Splash", false)
                                                    ])
                                                    .SetAnimation("Splash")
                                                );
    }
    private static int CalculateSpritesCount(int width, int height) => width / height;
}
