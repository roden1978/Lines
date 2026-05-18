using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class BoxCollider2D : Component, IUpdate, IDraw
{
    public Rectangle Box => _box;
    public Point Offset { get; set; }
    public bool IsTrigger { get; set; }
    public bool IsDraw { get; set; }
    public BodyTypes BodyType => _bodyType;
    public Sprite Sprite {get; set;}
    private Rectangle _box;

    private readonly BodyTypes _bodyType;
    public BoxCollider2D(int width, int height, BodyTypes bodyType)
    {
        _box = new Rectangle(0, 0, width, height);
        _bodyType = bodyType;
    }

    public BoxCollider2D(int x, int y, int width, int height, BodyTypes bodyType)
    {
        _box = new Rectangle(x, y, width, height);
        _bodyType = bodyType;
    }

    public override void Start()
    {
        UpdatePosition(gameObject.Transform.AbsolutePosition);
    }

    private void UpdatePosition(Vector2 position)
    {
        _box.X = (int)(position.X - _box.Width / 2 + Offset.X);
        _box.Y = (int)(position.Y - _box.Height / 2 + Offset.Y);
    }

    public void Update(GameTime gameTime)
    {
        if(Active == false) return;

        UpdatePosition(gameObject.Transform.AbsolutePosition);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if(Active == false || IsDraw == false) return;

        if(Sprite.Image == null)
            CreateDebugTexture(spriteBatch);
        
        spriteBatch.Draw(Sprite.Image, new Vector2(_box.X, _box.Y), _box, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
    }

    private void CreateDebugTexture(SpriteBatch spriteBatch)
    {
        Color[] colors = new Color[_box.Width * _box.Height];
        
        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(Color.Red, .5f);
        }
        
        Texture2D newTexture2D = new (spriteBatch.GraphicsDevice, _box.Width, _box.Height);
        newTexture2D.SetData(0, 0, null, colors, 0, colors.Length);

        Sprite = new Sprite(newTexture2D);
    }


}

public enum BodyTypes
{
    Static = 0,
    Dynamic = 1,
}

