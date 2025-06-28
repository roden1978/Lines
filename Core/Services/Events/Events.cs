using Microsoft.Xna.Framework;

public abstract class GameEvent { }
public interface UIEvent { }
internal class PlayerObstacleCollision : GameEvent {}


public struct UIEventInfo : UIEvent
{
    public GameObject GameObject;
    public int x;
    public int y;
}