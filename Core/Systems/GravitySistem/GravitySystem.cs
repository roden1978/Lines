using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class GravitySystem : IUpdate
{
    private readonly float _gravity;
    public bool Active { get; private set; } = true;
    private List<Rigidbody> _bodies = new();

    public GravitySystem(float gravity = -9.81f)
    {
        _gravity = gravity;
    }


    public void SetActive(bool value)
    {
        Active = value;
    }

    public void Update(GameTime gameTime)
    {
        if (Active == false) return;

        foreach (var rigidbody in _bodies)
        {
            if (rigidbody.UseGravity == false && rigidbody.Velocity != 0)
            {
                rigidbody.Velocity = 0.0f;
            }

            if (rigidbody.UseGravity)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                rigidbody.Velocity += _gravity * deltaTime * 40;
                rigidbody.gameObject.Transform.Position += new Vector2(0, rigidbody.Velocity * deltaTime);
            }
        }
    }

    public void Register(Scene scene)
    {
        foreach (var go in scene.FindGameObjectsWithComponent<Rigidbody>())
        {
            _bodies.Add(go.GetComponent<Rigidbody>());
        }
    }
    public void CleanUp()
    {
        _bodies.Clear();
    }
}