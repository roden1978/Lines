using Microsoft.Xna.Framework.Graphics;

public interface IDraw
{
    Sprite Sprite { get; set;}
    void Draw(SpriteBatch spriteBatch);
    void SetActive(bool value);
}
