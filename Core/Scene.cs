using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Lines;

public class Scene
{
    public event Action<GameObject> Adding;
    public string Name;
    public Canvas Canvas => _canvas;
    public bool Active { get; set; }
    public bool Started { get; private set; }

    private readonly BasicEffect _spriteBatchEffect;
    private readonly Vector3 _cameraPosition = new(0, -64, 0);
    private readonly Repository<GameObject> _gameObjects = new();
    private readonly Canvas _canvas;

    public Scene(string name, Canvas canvas, IGraphicsDeviceProvider graphicsDeviceProvider)
    {
        Name = name;
        _canvas = canvas;

        _spriteBatchEffect = new BasicEffect(graphicsDeviceProvider.GraphicsDevice)
        {
            TextureEnabled = true,
            View = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up),
            Projection = Matrix.CreateOrthographic(Settings.ScreenWidth, Settings.ScreenHeight, 0f, -1f)
        };
    }

    public void Register(IEnumerable<GameObject> gameObjects)
    {
        foreach (GameObject gameObject in gameObjects)
            Traverse(Add, gameObject);
    }

    public Scene Register(GameObject gameObject)
    {
        Traverse(Add, gameObject);

        return this;
    }
    private void Traverse(Action<GameObject> add, GameObject gameObject)
    {
        add(gameObject);
        foreach (Transform2D child in gameObject.Transform.Childrens)
            Traverse(Add, child.Gameobject);
    }
    private void Add(GameObject gameObject)
    {
        gameObject.Scene = this;

        if (gameObject.ComponentsContainer.HasAnyIUIComponent())
        {
            if (gameObject.TryGetComponent(out CanvasHandler _) == false)
            {
                gameObject.AddComponent(new CanvasHandler());
            }
            _canvas.Register(gameObject);
            return;
        };

        _gameObjects.Add(gameObject);

        Adding?.Invoke(gameObject);

        if (false == gameObject.Started)
            gameObject.Start();
    }
    public void Unregister(GameObject gameObject)
    {
        if (_gameObjects.Contains(gameObject))
        {
            gameObject.Scene = null;
            _gameObjects.Remove(gameObject);
        }

        if (false == _canvas.Contains(gameObject)) return;
        gameObject.Canvas = null;
        gameObject.Scene = null;
        _canvas.Remove(gameObject);
    }

    public void Update(GameTime gameTime)
    {
        UpdateCanvas(gameTime);
        UpdateScene(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(SpriteSortMode.BackToFront, null, null, null, RasterizerState.CullClockwise, _spriteBatchEffect);
        DrawScene(spriteBatch);
        spriteBatch.End();

        //Draw canvas elements
        spriteBatch.Begin();
        DrawCanvas(spriteBatch);
        spriteBatch.End();
    }

    private void UpdateScene(GameTime gameTime)
    {
        for (int i = 0; i < _gameObjects.Container.Count; i++)
            _gameObjects.Container[i]?.Update(gameTime);
    }

    private void DrawScene(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _gameObjects.Container.Count; i++)
            _gameObjects.Container[i]?.Draw(spriteBatch);
    }
    private void UpdateCanvas(GameTime gameTime)
    {
        _canvas.Update(gameTime);
    }

    private void DrawCanvas(SpriteBatch spriteBatch)
    {
        _canvas.Draw(spriteBatch);
    }

    public void Start()
    {
        for (int i = 0; i < _gameObjects.Container.Count; i++)
            _gameObjects.Container[i]?.Start();

        _canvas.Start();

        SetStarted(true);
    }

    public GameObject FindGameObjectWithTag(Tags tag) =>
        _gameObjects.FindAll().FirstOrDefault(x => x.Tag == tag);

    public GameObject FindGameObjectWithComponent<T>() =>
        _gameObjects.Container.FirstOrDefault(x => x.ComponentsContainer.Repository.ContainsKey(typeof(T)));

    public IReadOnlyList<GameObject> FindGameObjectsWithComponent<T>() =>
        _gameObjects.Container.Where(x => x.ComponentsContainer.Repository.ContainsKey(typeof(T))).ToList();

    public IReadOnlyList<GameObject> FindAllGameObjects<T>() => _gameObjects.FindAll();


    public void Destroy()
    {
        _canvas.Destroy();

        for (int i = 0; i < _gameObjects.Container.Count; i++)
            _gameObjects.Container[i]?.Destroy();

        _gameObjects.Cleanup();
    }

    public void SetActive(bool value)
    {
        _canvas.SetActive(value);

        for (int i = 0; i < _gameObjects.Container.Count; i++)
            _gameObjects.Container[i]?.SetActive(value);

        Active = value;
    }
    public void SetStarted(bool value) => Started = value;

}