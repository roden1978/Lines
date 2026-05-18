using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class UIImage : Component, ICanvasComponent, IDraw
{
    public Sprite Sprite { get => _sprite; set => _sprite = value; }
    public Color Color { get; set; }
    private Sprite _sprite;
    private readonly float _transparent;

    public UIImage(Sprite sprite, float transparent = 1f)
    {
        _sprite = sprite;
        _transparent = transparent;
        Color = Color.White;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Active & gameObject.Active)
        {
            Vector2 origin = new (_sprite.Rect.Width / 2, _sprite.Rect.Height / 2);
            spriteBatch.Draw(_sprite.Image, gameObject.DrawPosition, _sprite.Rect,
                Color * _transparent, gameObject.DrawRotation, origin, gameObject.DrawScale, SpriteEffects.None, 1);
        }
    }
}
