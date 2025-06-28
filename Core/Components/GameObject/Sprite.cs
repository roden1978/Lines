using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Sprite : IDestroy
{
    public Texture2D Image { get; private set;}
    public Rectangle Rect { get; }

    public string Name { get; set; }

    public Point StartPoint { get; }

    public int Width { get; }

    public int Height { get; }

    public Sprite(Texture2D texture)
    {
        Image = texture;
        Rect = new Rectangle(0, 0, Image.Width, Image.Height);
        StartPoint = new Point(0, 0);
        Width = Image.Width;
        Height = Image.Height;
    }
    public Sprite(Texture2D texture, Point startPoint, Point size) : this(texture)
    {
        Rect = new Rectangle(startPoint, size);
        StartPoint = startPoint;
        Width = size.X;
        Height = size.Y;
    }


    public Sprite(Texture2D texture, Rectangle rectangle) : this(texture)
    {
        Rect = rectangle;
        StartPoint = new Point(rectangle.X, rectangle.Y);
        Width = rectangle.Width;
        Height = rectangle.Height;
    }

    public Sprite(Texture2D texture, int x, int y, int width, int height) : this(texture)
    {
        Rect = new Rectangle(x, y, width, height);
        StartPoint = new Point(x, y);
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return Name;
    }

    public void Destroy()
    {
        Image  = null;
    }
}
