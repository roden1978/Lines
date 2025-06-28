using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lines;

public interface IGraphicsDeviceProvider
{
    GraphicsDevice GraphicsDevice { get; }
}

public class Game1 : Game, IGraphicsDeviceProvider
{
    private readonly ContainerBuilder _container;
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Vector3 _cameraPosition = new(0, -64, 0);
    private Site _site;
    ILifetimeScope _scope;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _container = new ContainerBuilder();
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        InitializeGraphicsDevice();
        InitializeContainer();
        base.Initialize();
    }

    private void InitializeGraphicsDevice()
    {
        // TODO: Add your initialization logic here
        Window.Title = "Lines";

        _graphics.GraphicsProfile = GraphicsProfile.Reach;
        _graphics.PreferredBackBufferWidth = Settings.ScreenWidth;
        _graphics.PreferredBackBufferHeight = Settings.ScreenHeight;
        _graphics.ApplyChanges();

        // Update camera View and Projection.
        Viewport vp = GraphicsDevice.Viewport;
    }

    private void InitializeContainer()
    {
        _container.RegisterInstance(this).As<IGraphicsDeviceProvider>().SingleInstance();
        _container.RegisterModule(new ServicesInstaller());
        _container.RegisterModule(new ModelsInstaller());
        _container.RegisterModule(new FactoryIstaller());
        _container.RegisterModule(new BehaviourInstaller());
        _container.RegisterInstance<Scene>(new("Scene", new("Canvas", Settings.ScreenWidth, Settings.ScreenHeight), this));
        _container.RegisterType<Site>().AsSelf().As<IStartable>().SingleInstance();
        _scope = _container.Build().BeginLifetimeScope();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        using var scope = _scope;
        _site = scope.Resolve<Site>();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _site.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.AntiqueWhite);

        _site.Draw(_spriteBatch);

        base.Draw(gameTime);
    }
}
