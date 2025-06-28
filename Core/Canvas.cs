using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Canvas
{
    public event Action UIObjectsCountHasChanged;
    public Rectangle CanvasRect { get; private set; }
    public string Name { get; private set; }
    public bool Active { get; private set; }
    public bool Started { get; private set; }
    private readonly List<GameObject> _gameObjects = [];
    private readonly List<IUpdate> _updateableComponents = [];
    private readonly List<IDraw> _drawableComponents = [];
    private readonly MouseEventSystem _mouseEventSystem;
    public Canvas(string name, int width, int height)
    {
        CanvasRect = new Rectangle(0, 0, width, height);
        Name = name;
        _mouseEventSystem = new MouseEventSystem();
        _mouseEventSystem.PositionUpdate += OnPositionUpdate;
        _mouseEventSystem.ClickUpdate += OnClickUpdate;
    }

    public Canvas(string name, int x, int y, int width, int height)
    {
        CanvasRect = new Rectangle(x, y, width, height);
        Name = name;
        _mouseEventSystem = new MouseEventSystem();
        _mouseEventSystem.PositionUpdate += OnPositionUpdate;
        _mouseEventSystem.ClickUpdate += OnClickUpdate;
    }

    public Canvas Register(GameObject gameObject)
    {
        Add(gameObject);
        return this;
    }

    public bool Contains(GameObject gameObject)
    {
        return _gameObjects.Contains(gameObject);
    }

    public void Remove(GameObject gameObject)
    {
        _drawableComponents.Remove(gameObject as IDraw);
        _updateableComponents.Remove(gameObject);
        _gameObjects.Remove(gameObject);
    }

    private void Add(GameObject gameObject)
    {
        gameObject.Canvas = this;
        _gameObjects.Add(gameObject);
        
        if (gameObject.TryGetComponent(out CanvasHandler canvasHandler) == false)
            throw new ArgumentNullException($"UI game object {gameObject.Name} must have CanvasHandler component!");

        _updateableComponents.Add(gameObject);

        if (gameObject.ComponentsContainer.HasAnyDrawableComponent())
        {
            IEnumerable<IDraw> draws = gameObject.ComponentsContainer.GetDraws();
            //Console.WriteLine($"Game object {gameObject.Name} Draws {draws.Count()}");
            foreach (IDraw component in draws)
            {
                canvasHandler.Width = component.Sprite.Width;
                canvasHandler.Height = component.Sprite.Height;
                //Console.WriteLine ($"Canvas handler width {canvasHandler.Width} height {canvasHandler.Height}");
                _drawableComponents.Add(component);
            }
        }

        IInteractable interactable = gameObject.ComponentsContainer.GetInteractable();
        if (interactable != null)
            canvasHandler.InteractableComponent = interactable;

        if (false == gameObject.Started)
            gameObject.Start();

        UIObjectsCountHasChanged?.Invoke();

        //Console.WriteLine ($"Component count {gameObject.ComponentCount} name {gameObject.Name}");
    }
    private void OnClickUpdate(object sender, MouseEventArgs e)
    {
        for(int i = 0; i < _gameObjects.Count; i++)
        {
            if (_gameObjects[i].Active & _gameObjects[i].TryGetComponent(out CanvasHandler canvasHandler))
                canvasHandler.OnClickUpdate(sender, e);
        }
    }
    private void OnPositionUpdate(object sender, MouseEventArgs e)
    {
        for(int i = 0; i < _gameObjects.Count; i++)
        {
            if (_gameObjects[i].Active & _gameObjects[i].TryGetComponent(out CanvasHandler canvasHandler))
                canvasHandler.OnPositionUpdate(sender, e);
        }
    }
    public void Update(GameTime gameTime)
    {
        _mouseEventSystem.Update(gameTime);

        for(int i = 0; i < _updateableComponents.Count; i++)
            if (_updateableComponents[i].Active)
                _updateableComponents[i].Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for(int i = 0; i < _gameObjects.Count; i++)
            if (_gameObjects[i].Active)
                _gameObjects[i].Draw(spriteBatch);
    }

    public void Start()
    {
        foreach (GameObject gameObject in _gameObjects)
            if (gameObject.Active)
            {
                gameObject.Start();
                SetStarted(true);
            }
    }

    public void SetActive(bool value)
    {
        foreach (GameObject gameObject in _gameObjects)
            gameObject.SetActive(value);

        Active = value;
    }

    public void SetStarted(bool value) => Started = value;

    public GameObject FindUiObjectWithTag(Tags tag) =>
        _gameObjects.FirstOrDefault(x => x.Tag == tag);

    public GameObject FindUiObjectWithComponent<T>() =>
        _gameObjects.FirstOrDefault(x => x.ComponentsContainer.Repository.ContainsKey(typeof(T)));

    public IReadOnlyList<GameObject> FindUiObjectsWithComponent<T>() =>
        _gameObjects.Where(x => x.ComponentsContainer.Repository.ContainsKey(typeof(T))).ToList();

    public IReadOnlyList<GameObject> FindAllUiObjects<T>() => _gameObjects;

    public void Destroy()
    {
        _mouseEventSystem.PositionUpdate -= OnPositionUpdate;
        _mouseEventSystem.ClickUpdate -= OnClickUpdate;

        for (int i = 0; i < _gameObjects.Count; i++)
            _gameObjects[i]?.Destroy();

    }
}