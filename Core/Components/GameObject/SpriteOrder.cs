using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteOrder : IDestroy
{
    public Sprite[] Sprites => _spriteOrder; 
    private Texture2D _texture;
    private readonly int _row;
    private readonly int _column;
    private readonly Point _start;
    private Sprite[] _spriteOrder;
    private readonly Rectangle _rect;

    public int Length
    {
        get
        {
            int row = ValidateRow();
            int column = ValidateColumn();
            return row * column;
        }
    }

    public SpriteOrder(Texture2D texture, int row, int column)
    {
        _texture = texture;
        _row = row;
        _column = column;
        _start = new Point(0, 0);
        _spriteOrder = Create();
        
    }
    public SpriteOrder(Texture2D texture, Point start, int row, int column)
    {
        _texture = texture;
        _row = row;
        _column = column;
        _start = start;
        _spriteOrder = Create();
    }

    public SpriteOrder(Texture2D texture, Point start, int row, int column, Rectangle rect)
    {
        _texture = texture;
        _row = row;
        _column = column;
        _start = start;
        _rect = rect;
        _spriteOrder = Create();
    }

    private Sprite[] Create()
    {
        Sprite[] spriteOrder = new Sprite[_row * _column];

        int row = ValidateRow();
        int column = ValidateColumn();
        int count = 0;

        int width = _rect == Rectangle.Empty ? _texture.Width : _rect.Width;
        int height = _rect == Rectangle.Empty ? _texture.Height : _rect.Height;

        for (int i = _start.X; i < column; i++)
            for (int j = _start.Y; j < row; j++)
            {
                spriteOrder[count] = new Sprite(_texture,
                                    j * width / _row,
                                    i * height / _column,
                                    width / _row,
                                    height / _column);
                count++;
            }


        return spriteOrder;
    }

    private int ValidateRow()
    {
        return _row <= 0 ? 1 : _row;
    }

    private int ValidateColumn()
    {
        return _column <= 0 ? 1 : _column;
    }

    public Sprite GetSpriteByIndex(int index)
    {
        return _spriteOrder[index];
    }

    public void Destroy()
    {
        _texture = null;
        _spriteOrder = null;
    }
}
