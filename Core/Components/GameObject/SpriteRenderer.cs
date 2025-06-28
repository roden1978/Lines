using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteRenderer : Component, IDraw
{
    public Sprite Sprite { get; set; }
    public Color Color { get; set; }

    public bool Flip;
    private readonly float _depth;


    public SpriteRenderer(Sprite sprite = null)
    {
        Sprite = sprite;
        Color = Color.White;
    }

    public SpriteRenderer(float depth = 0, Sprite sprite = null)
    {
        _depth = depth;
        Sprite = sprite;
        Color = Color.White;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Active && gameObject.Active)
        {
            SpriteEffects flip = Flip ? SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally : SpriteEffects.FlipVertically; 

            Vector2 position = gameObject.Transform.AbsolutePosition;
            Vector2 origin = new Vector2(gameObject.Transform.Origin.X + Sprite.Width / 2, gameObject.Transform.Origin.Y + Sprite.Height / 2);
            float rotation = gameObject.Transform.AbsoluteRotation;

            spriteBatch.Draw(Sprite.Image, new Vector2(position.X, position.Y), Sprite?.Rect, Color,
                         rotation, origin, gameObject.Transform.Scale, flip, _depth);
        }
    }
}
