using Microsoft.Xna.Framework;

public class Splash : CollisionBehaviour, IPositionAdapter
{
    public Vector2 Position
    {
        get => gameObject.Transform.Position;
        set => gameObject.Transform.Position = value;
    }
}