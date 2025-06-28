using System.Collections.Immutable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TextDrawer : Component, IDraw, IUpdate, ICanvasComponent
{
    public string Text = string.Empty;
    public Sequence Sequence;
    private readonly StringSource _stringSource;
    public Color TextColor = Color.White;
    public Sprite Sprite { get => Sequence[0]; set => Sprite = Sequence[0]; }
    private readonly char[] _chars = ['A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                            '0','1','2','3','4','5','6','7','8','9',
                                            '.',',',';',':','?','!','-','#','"','_','&','(',')',
                                            '[',']','`','\\','/',' ','Б','В','+','=','*','$','<','>','%'];
    private ImmutableDictionary<char, int> _characters;
    private Sprite[] _sprites = [];
    private string _prevText = string.Empty;

    public TextDrawer(Sequence sequence, StringSource stringSource = null)
    {
        Sequence = sequence;
        _stringSource = stringSource;
    }

    public override void Start() => Initialize();

    private void Initialize()
    {
        CreateCharacters();
        CreateSpriteString();
    }

    private void CreateCharacters()
    {
        ImmutableDictionary<char, int>.Builder builder = ImmutableDictionary.CreateBuilder<char, int>();

        for (int i = 0; i < _chars.Length; i++)
        {
            builder.Add(_chars[i], i);
        }

        _characters = builder.ToImmutableDictionary();
    }

    private void CreateSpriteString()
    {
        if (_stringSource != null)
        {
            if (Text.Equals(_stringSource.Str)) return;
            else
            if(false == _stringSource.Str.Equals(string.Empty))
                Text = _stringSource.Str;
        }

        if (Text.Equals(_prevText)) return;

        char[] textChars = Text.ToCharArray();
        int[] indexes = new int[textChars.Length];
        _sprites = new Sprite[textChars.Length];

        for (int i = 0; i < textChars.Length; i++)
        {
            indexes[i] = _characters[textChars[i]];
            _sprites[i] = Sequence[indexes[i]];
        }
        _prevText = Text;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        int allSpritesWidth = 0;
        for (int i = 0; i < _sprites.Length; i++)
        {
            allSpritesWidth += _sprites[i].Width;
        }
        for (int i = 0; i < _sprites.Length; i++)
        {
            float startPosX = gameObject.DrawPosition.X - allSpritesWidth / 2 * gameObject.DrawScale.X;
            Vector2 nextPosition = new(startPosX + i * _sprites[i].Width * gameObject.DrawScale.X, gameObject.DrawPosition.Y);
            spriteBatch.Draw(_sprites[i].Image, nextPosition, _sprites[i]?.Rect, TextColor,
                                 gameObject.DrawRotation, Vector2.Zero, gameObject.DrawScale, SpriteEffects.None, 0f);
        }
    }

    public void Update(GameTime gameTime) => CreateSpriteString();

    public override void Destroy() => Sequence.Destroy();
}
