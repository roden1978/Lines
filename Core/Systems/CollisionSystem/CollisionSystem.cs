using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class CollisionSystem : IUpdate
{
    public bool Active { get; set; } = true;
    public bool Debug => _debug;
    private readonly List<BoxCollider2D> _colliders = [];
    private readonly Dictionary<BoxCollider2D, List<BoxCollider2D>> _collisionRepository = [];
    private readonly bool _debug;
    private readonly Scene _scene;
    private BoxCollider2D[] _copy = new BoxCollider2D[128];
    private int _count;

    public CollisionSystem(Scene scene, bool debug = false)
    {
        _debug = debug;
        _scene = scene;
        _scene.Adding += OnAdding;
    }
    public void Register(Scene scene)
    {
        foreach (var go in scene.FindGameObjectsWithComponent<BoxCollider2D>())
        {
            BoxCollider2D collider = go.GetComponent<BoxCollider2D>();
            collider.IsDraw = _debug;

            _colliders.Add(collider);
        }

        Array.Copy(_colliders.ToArray(), _copy, _colliders.Count);

        _count = _colliders.Count;
    }
    public void Update(GameTime gameTime)
    {
        if (_count != _colliders.Count)
        {
            _count = _colliders.Count;
            _copy = [.. _colliders];
        }

        for (int i = 0; i < _count; i++)
        {
            for (int j = 0; j < _count; j++)
            {
                if (!_copy[i].Active || !_copy[j].Active) continue;
                if (i != j)
                {
                    Intersect(_copy[i], _copy[j]);
                }
            }
        }
    }

    public void SetActive(bool value)
    {
        Active = value;
    }

    private void OnAdding(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<BoxCollider2D>(out var boxCollider))
        {
            //System.Console.WriteLine($"Add go {gameObject.Name}");
            _colliders.Add(boxCollider);
        }
    }

    public void Intersect(BoxCollider2D collider1, BoxCollider2D collider2)
    {
        var result = collider1.Box.Intersects(collider2.Box);
        if (result)
        {
            if (_collisionRepository.ContainsKey(collider1))
            {
                if (_collisionRepository[collider1].Contains(collider2))
                {
                    foreach (var obj in collider1.gameObject.CollisionBehaviourContainer.Repository)
                    {
                        obj.Value.OnCollisionStay(collider2);
                    }

                    if (CheckUpDownCollision(collider1, collider2, out int depth) & !collider2.IsTrigger)
                    {
                        //Console.WriteLine($"Collider {collider1.gameObject.Name} "
                        //+ $"top {collider1.Box.Top} bottom {collider1.Box.Bottom} "
                        //+ $"Collider {collider2.gameObject.Name} "
                        //+ $"top {collider2.Box.Top} bottom {collider2.Box.Bottom} X {collider2.Box.X} Y {collider2.Box.Y} depth: {depth}");

                        collider1.gameObject.Transform.Position = new Vector2(
                           collider1.gameObject.Transform.Position.X,
                        collider1.gameObject.Transform.Position.Y + depth - 1);

                    }
                }
                else
                {
                    _collisionRepository[collider1].Add(collider2);
                    foreach (var obj in collider1.gameObject.CollisionBehaviourContainer.Repository)
                    {
                        obj.Value.OnCollisionEnter(collider2);
                    }


                }

            }
            else
            {
                _collisionRepository.Add(collider1, new List<BoxCollider2D>());

                _collisionRepository[collider1].Add(collider2);

                foreach (KeyValuePair<Type, CollisionBehaviour> obj in collider1.gameObject.CollisionBehaviourContainer.Repository)
                {
                    obj.Value.OnCollisionEnter(collider2);
                }


            }
        }
        else
        {
            if (_collisionRepository.ContainsKey(collider1))
            {
                if (_collisionRepository[collider1].Contains(collider2))
                {
                    foreach (var obj in collider1.gameObject.CollisionBehaviourContainer.Repository)
                    {
                        obj.Value.OnCollisionExit(collider2);
                    }
                    _collisionRepository[collider1].Remove(collider2);
                }

            }
        }

        if (_collisionRepository.ContainsKey(collider1) && _collisionRepository[collider1].Count == 0)
            _collisionRepository.Remove(collider1);

    }

    private bool CheckUpDownCollision(BoxCollider2D collider1, BoxCollider2D collider2, out int depth)
    {
        if (collider1.BodyType == BodyTypes.Static)
        {
            depth = 0;
            return false;
        }

        var top = collider1.Box.Top;
        var bottom = collider2.Box.Bottom;
        if (top < bottom)
        {
            depth = Math.Abs(bottom - top);
            return true;
        }

        depth = 0;
        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_debug)
        {
            spriteBatch.Begin();
            for (int i = 0; i < _copy.Length; i++)
            {
                _copy[i]?.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

    }

    public void CleanUp()
    {
        _scene.Adding -= OnAdding;
        _colliders.Clear();
        _copy = new BoxCollider2D[128];
        _collisionRepository.Clear();
    }
}