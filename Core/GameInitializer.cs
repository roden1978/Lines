using Autofac;

public class GameInitializer : IStartable
{
    private readonly Scene _scene;
    private readonly GameFactory _gameFactory;

    public GameInitializer(Scene scene, GameFactory gameFactory)
    {
        _scene = scene;
        _gameFactory = gameFactory;
    }
    public void Start() => Initialize();
    private void Initialize() => _scene.Register(_gameFactory.Create());
}
