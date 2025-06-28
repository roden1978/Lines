using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class GameObject : IUpdate, IStart, IDestory
{
    public event Action<int> ChildsCountHasChanged;
    public bool Active { get; private set; } = true;
    public bool Started { get; private set; } = false;
    public Transform2D Transform => _transform;
    public string Name { get; set; } = string.Empty;
    public Tags Tag { get; set; }
    public int Layer { get; set; }
    public int ComponentCount => ComponentsContainer.Count;
    public Container<CollisionBehaviour> CollisionBehaviourContainer { get; } = new();
    public Container<Component> ComponentsContainer { get; } = new();
    public Scene Scene { get; set; }
    public Canvas Canvas {get; set;}
    public int ChildsCount => _childsCount;
    private readonly List<IUpdate> _updateableComponents = [];
    private readonly List<IDraw> _drawableComponents = [];
    private readonly Transform2D _transform = new();
    private int _childsCount;

    public Vector2 DrawPosition
    {
        get => Transform.Parent != null ? Transform.Position + Transform.Parent.AbsolutePosition : Transform.Position;
    }
    public float DrawRotation
    {
        get => Transform.Parent != null ? Transform.Rotation + Transform.Parent.Rotation : Transform.Rotation;
    }

    public Vector2 DrawScale
    {
        get => Transform.Parent != null ? Transform.Scale + Transform.Parent.Scale - Vector2.One : Transform.Scale;
    }

    public GameObject()
    {
        Name = "GameObject";
        _transform.Gameobject = this;
    }
    public GameObject(string name)
    {
        Name = name;
        _transform.Gameobject = this;
    }

    public GameObject(string name, Transform2D transform)
    {
        Name = name;
        _transform = transform;
        _transform.Gameobject = this;
    }

    public GameObject(string name, Transform2D transform, Transform2D parent)
    {
        Name = name;
        _transform = transform;
        _transform.Parent = parent;
        _transform.Gameobject = this;
    }
    public GameObject(string name, Vector2 position, float rotation, Vector2 scale)
    {
        _transform.Position = position;
        _transform.Rotation = rotation;
        _transform.Scale = scale;
        _transform.Gameobject = this;
        Name = name;
    }
    public void Start()
    {
        StartComponents();
        StartBehaviour();

        SetStarted(true);
    }

    private void StartBehaviour()
    {
        foreach (var component in CollisionBehaviourContainer.Repository.Where(x => x.Value.Started == false))
            component.Value.Start();
    }

    private void StartComponents()
    {
        foreach (var component in ComponentsContainer.Repository.Where(x => x.Value.Started == false))
            component.Value.Start();
    }

    public void Update(GameTime gameTime)
    {
        if (false == Active) return;

        for (int i = 0; i < _updateableComponents.Count; i++)
            _updateableComponents[i]?.Update(gameTime);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        if (false == Active) return;

        for (int i = 0; i < _drawableComponents.Count; i++)
            _drawableComponents[i].Draw(spriteBatch);
    }
    public GameObject AddComponent<T>(T component) where T : Component
    {
        if (component is IUpdate)
            _updateableComponents.Add(component as IUpdate);

        if (component is IDraw)
            _drawableComponents.Add(component as IDraw);

        if (component is CollisionBehaviour)
            CollisionBehaviourContainer.Register(component as CollisionBehaviour);

        component.gameObject = this;

        ComponentsContainer.Register(component);
        
        if(Started)
            component.Start();

        return this;
    }
    public void Unregister<T>(T component) where T : Component =>
        ComponentsContainer.Unregister(component);

    public T GetComponent<T>() where T : Component
    {
        if (ComponentsContainer.TryGetComponent<T>(out var component))
            return (T)component;

        return null;
    }
    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = default;

        if (ComponentsContainer.TryGetComponent<T>(out var c))
        {
            component = c;
            return true;
        }

        return false;
    }

    public override string ToString() => Name;

    public void SetActive(bool value)
    {
        foreach (var children in Transform.Childrens)
            children.Gameobject.SetActive(value);

        foreach (IUpdate component in _updateableComponents)
            component.SetActive(value);

        foreach (IDraw component in _drawableComponents)
            component.SetActive(value);

        if(false == Started) Start();

        Active = value;
    }

    public void AddChild()
    {
        _childsCount++;
        ChildsCountHasChanged?.Invoke(_childsCount);
    }

    public void RemoveChild()
    {
        if (_childsCount > 0)
        {
            _childsCount--;
            ChildsCountHasChanged?.Invoke(_childsCount);
        }
    }

    public void SetStarted(bool value) => Started = value;

    public void Destroy() => Cleanup();

    private void Cleanup()
    {
        CleanupUpdatables();
        CleanupDrawables();
        CleanupComponents();
        CleanupBehaviours();
    }

    private void CleanupBehaviours()
    {
        foreach (var behaviour in CollisionBehaviourContainer.Repository.Values)
            behaviour.Destroy();

        CollisionBehaviourContainer.Cleanup();
    }

    private void CleanupComponents()
    {
        foreach (Component component in ComponentsContainer.Repository.Values)
            component.Destroy();
        ComponentsContainer.Cleanup();
    }

    private void CleanupDrawables()
    {
        for (int i = 0; i < _drawableComponents.Count; i++)
            _drawableComponents[i] = null;

        _drawableComponents.Clear();
    }

    private void CleanupUpdatables()
    {
        for (int i = 0; i < _updateableComponents.Count; i++)
            _updateableComponents[i] = null;

        _updateableComponents.Clear();
    }
}