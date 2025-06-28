using Microsoft.Xna.Framework;

public class SelectableButton : Button
{
    public Color SelectedColor { get; set; }

    public SelectableButton(Sprite sprite, Color normalColor, Color highlightedColor, Color pressedColor, Color selectedColor, float transparent = 1)
    : base(sprite, normalColor, highlightedColor, pressedColor, transparent)
    {
        SelectedColor = selectedColor;
    }

    public SelectableButton(Sprite sprite) : base(sprite)
    {
        NormalColor = Color.White;
        HighlightedColor = Color.Gray;
        PressedColor = Color.White;
        SelectedColor = Color.Green;
        _currentColor = NormalColor;
        _highlightedColor = Color.Gray;
        _pressedColor = Color.White;
        _transparent = 1.0f;
    }

    public override void OnPointerClickHandler(UIEventInfo sender, MouseEventArgs e)
    {
        if(false == Selected)
            UnSelectedClick(sender, e);
        else
            SelectedClick(sender, e);        
    }

    private void SelectedClick(UIEventInfo sender, MouseEventArgs e)
    {
        RestoreColor();
        OnClick?.Invoke(sender);
    }

    private void UnSelectedClick(UIEventInfo sender, MouseEventArgs e)
    {
        _currentColor = PressedColor;
        Selected = true;

        OnClick?.Invoke(sender);
    }

    public override void OnPointerEnterHandler(UIEventInfo sender, MouseEventArgs e)
    {
        Pointing = true;
        _currentColor = HighlightedColor;
        
        OnEnter?.Invoke(sender);
    }

    public override void OnPointerExitHandler(UIEventInfo sender, MouseEventArgs e)
    {
        Pointing = false;
        _currentColor = Selected ? SelectedColor : NormalColor;

        OnExit?.Invoke(sender);
    }

    public void RestoreColor()
    {
        _currentColor = NormalColor;
        Selected = false;
    }

    public void SetCurrentColor(Color color)
    {
        _currentColor = color;
    }
    public void RestoreHighlightedColor()
    {
        HighlightedColor = _highlightedColor;
    }
}
