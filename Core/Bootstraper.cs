using Autofac;
using Lines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


public class Bootstraper : DrawableGameComponent
{
    private readonly Game _game;
    private readonly ContainerBuilder _container;
    private readonly Scene _scene;
    private readonly SpriteBatch _spriteBatch;
    public Bootstraper(Game game) : base(game)
    {
        _game = game;
        _container = new ContainerBuilder();
        _scene = new("Scene", new("Canvas", Settings.ScreenWidth, Settings.ScreenHeight), game.GraphicsDevice);
        _spriteBatch = new SpriteBatch(game.GraphicsDevice);
    }
    public override void Draw(GameTime gameTime) => _scene.Draw(_spriteBatch);

    public override void Update(GameTime gameTime) => _scene.Update(gameTime);

    public override void Initialize() => InitializeContainer();

    private void InitializeContainer()
    {
        _container.RegisterInstance(_game).As<IGraphicsDeviceProvider>().SingleInstance();
        _container.RegisterModule(new ServicesInstaller());
        _container.RegisterModule(new ModelsInstaller());
        _container.RegisterModule(new FactoryIstaller());
        _container.RegisterModule(new BehaviourInstaller());
        _container.RegisterInstance(_scene);
        _container.RegisterType<GameInitializer>().AsSelf().As<IStartable>().SingleInstance();

        _container.Build().BeginLifetimeScope();
    }
}
