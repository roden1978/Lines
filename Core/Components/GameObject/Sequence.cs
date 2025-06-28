
public class Sequence : IDestory
{
    public int Length => _spriteOrder.Length;
    private SpriteOrder _spriteOrder;
    private Sprite[] _sequence;
    public Sequence(SpriteOrder spriteOrder)
    {
        _spriteOrder = spriteOrder;
    }
    public Sprite this[int index]
    {
        get
        {
            if (_sequence == null)
                _sequence = _spriteOrder.Sprites;

            return _sequence[index];
        }
    }

    public void Destroy()
    {
        if (_spriteOrder != null)
        {
            _spriteOrder.Destroy();
            _spriteOrder = null;
        }

        _sequence = null;
    }
}