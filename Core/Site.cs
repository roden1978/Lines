using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Site : IStartable
{
    private readonly Scene _scene;
    private readonly GameFactory _gameFactory;

    public Site(Scene scene, GameFactory gameFactory)
    {
        _scene = scene;
        _gameFactory = gameFactory;
    }
    public void Start() => Initialize();
    private void Initialize() => _scene.Register(_gameFactory.Create());
    public void Update(GameTime gameTime) => _scene.Update(gameTime);
    public void Draw(SpriteBatch spriteBatch) => _scene.Draw(spriteBatch);
}