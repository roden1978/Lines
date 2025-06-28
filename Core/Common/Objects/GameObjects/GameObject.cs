using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class GameObject : IUpdate, IStart, IDestroy
{
    public event Action<int> ChildrensCountHasChanged;
    public bool Active { get; private set; } = true;
    public bool Started { get; private set; }
    public Transform2D Transform => _transform;
    public string Name { get; set; }
    public Tags Tag { get; set; }
    public int Layer { get; set; }
    public int ComponentCount => ComponentsContainer.Count;
    public Container<CollisionBehaviour> CollisionBehaviourContainer { get; } = new();
    public Container<Component> ComponentsContainer { get; } = new();
    public Scene Scene { get; set; }
    public Canvas Canvas {get; set;}
    public int ChildrensCount => _childrensCount;
    private readonly List<IUpdate> _updateableComponents = [];
    private readonly List<IDraw> _drawableComponents = [];
    private readonly Transform2D _transform = new();
    private int _childrensCount;

    public Vector2 DrawPosition => Transform.Parent != null ? Transform.Position + Transform.Parent.AbsolutePosition : Transform.Position;

    public float DrawRotation => Transform.Parent != null ? Transform.Rotation + Transform.Parent.Rotation : Transform.Rotation;

    public Vector2 DrawScale => Transform.Parent != null ? Transform.Scale * Transform.Parent.Scale : Transform.Scale;

    public GameObject()
    {
        Name = "GameObject";
        _transform.Gameobject = this;
    }
    public GameObject(string name) : this() => Name = name;
    public GameObject(string name, Transform2D transform) : this(name)
    {
        _transform = transform;
        _transform.Gameobject = this;
    }
    public GameObject(string name, Transform2D transform, Transform2D parent) : this(name, transform) => _transform.Parent = parent;
    public GameObject(string name, Vector2 position, float rotation, Vector2 scale) : this(name)
    {
        _transform.Position = position;
        _transform.Rotation = rotation;
        _transform.Scale = scale;
    }
    public void Start()
    {
        StartComponents();
        StartBehaviour();

        SetStarted(true);
    }

    private void StartBehaviour()
    {
        foreach (KeyValuePair<Type, CollisionBehaviour> component in CollisionBehaviourContainer.Repository.Where(x =>
                     x.Value.Started == false))
            component.Value.Start();
    }

    private void StartComponents()
    {
        foreach (KeyValuePair<Type, Component> component in ComponentsContainer.Repository.Where(x =>
                     x.Value.Started == false))
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
            _drawableComponents[i]?.Draw(spriteBatch);
    }
    public GameObject AddComponent<T>(T component) where T : Component
    {
        if (component is IUpdate update)
            _updateableComponents.Add(update);

        if (component is IDraw draw)
            _drawableComponents.Add(draw);
        
        if (component is CollisionBehaviour collisionBehaviour) 
            CollisionBehaviourContainer.Register(collisionBehaviour);

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
        if (ComponentsContainer.TryGetComponent(out T component))
            return component;

        return null;
    }
    public bool TryGetComponent<T>(out T component) where T : Component
    {
        component = null;

        if (!ComponentsContainer.TryGetComponent(out T c)) return false;
        component = c;
        return true;

    }

    public override string ToString() => Name;

    public void SetActive(bool value)
    {
        foreach (Transform2D children in Transform.Childrens)
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
        _childrensCount++;
        ChildrensCountHasChanged?.Invoke(_childrensCount);
    }

    public void RemoveChild()
    {
        if (_childrensCount <= 0) return;
        _childrensCount--;
        ChildrensCountHasChanged?.Invoke(_childrensCount);
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
        foreach (CollisionBehaviour behaviour in CollisionBehaviourContainer.Repository.Values)
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