
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Button : Component, IButton
{
    public Sprite Sprite { get; set; }
    public Color NormalColor { get; set; }
    public Color HighlightedColor { get; set; }
    public Color PressedColor { get; set; }
    public bool Interactable { get; set; } = true;

    public Rectangle InteractableArea
    {
        get
        {
            Vector2 position = gameObject.DrawPosition;

            int x = Convert.ToInt32(position.X) - Convert.ToInt32(Sprite.Rect.Width * gameObject.Transform.Scale.X) / 2;
            int y = Convert.ToInt32(position.Y) - Convert.ToInt32(Sprite.Rect.Height * gameObject.Transform.Scale.Y) / 2;
            int width = Convert.ToInt32(Sprite.Rect.Width * gameObject.Transform.Scale.X);
            int height = Convert.ToInt32(Sprite.Rect.Height * gameObject.Transform.Scale.Y);
            _interactableArea = new Rectangle(x, y, width, height);

            return _interactableArea;
        }
        set { _interactableArea = value; }
    }


    public bool Pointing { get; protected set; }
    public bool Selected { get; protected set; }
    public Action<UIEvent> OnClick { get; set; }
    public Action<UIEvent> OnEnter { get; set; }
    public Action<UIEvent> OnExit { get; set; }
    protected Color _currentColor;
    protected Color _highlightedColor;
    protected Color _pressedColor;
    protected float _transparent;
    protected float _notInteractableTransparent = 0;
    private Rectangle _interactableArea;

    public Button(Sprite sprite, Color normalColor, Color highlightedColor, Color pressedColor, float transparent = 1.0f)
    {
        Sprite = sprite;
        NormalColor = normalColor;
        HighlightedColor = highlightedColor;
        PressedColor = pressedColor;
        _currentColor = NormalColor;
        _highlightedColor = highlightedColor;
        _pressedColor = pressedColor;
        _transparent = transparent;
    }

    public Button(Sprite sprite)
    {
        Sprite = sprite;
        NormalColor = Color.White;
        HighlightedColor = Color.Gray;
        PressedColor = Color.White;
        _currentColor = NormalColor;
        _highlightedColor = HighlightedColor;
        _pressedColor = PressedColor;
        _transparent = 1.0f;
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        Vector2 position = gameObject.DrawPosition;
        Vector2 origin = new(Sprite.Rect.Width / 2, Sprite.Rect.Height / 2);

        if (false == Interactable)
            _transparent = _notInteractableTransparent == 0 ? .5f : _notInteractableTransparent;
        else
            _transparent = 1;

        spriteBatch.Draw(Sprite.Image, position, Sprite.Rect,
            _currentColor * _transparent, gameObject.DrawRotation, origin, gameObject.DrawScale, SpriteEffects.None, 0);
    }

    public virtual void OnPointerClickHandler(UIEventInfo sender, MouseEventArgs e)
    {
        _currentColor = PressedColor;
        OnClick?.Invoke(sender);
    }

    public virtual void OnPointerEnterHandler(UIEventInfo sender, MouseEventArgs e)
    {
        Pointing = true;
        _currentColor = HighlightedColor;
    }

    public virtual void OnPointerExitHandler(UIEventInfo sender, MouseEventArgs e)
    {
        Pointing = false;
        _currentColor = NormalColor;
    }

    public void SetNotInteractableTransparent(float value)
    {
        _notInteractableTransparent = value < 0 ? 0 : value > 1 ? 1 : value;
    }
}
