using Microsoft.Xna.Framework;

public interface IUpdate
{
    bool Active { get; }
    void SetActive(bool value);
    void Update(GameTime gameTime);
}