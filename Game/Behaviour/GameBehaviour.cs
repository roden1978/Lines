using Microsoft.Xna.Framework;

public class GameBehaviour : Component, IStart, ICanvasComponent
{
    private readonly LinesModel _lines;
    private Color _warningColor = Color.Red;
    
    public GameBehaviour(LinesModel lines)
    {
        _lines = lines;
    }

    public override void Start()
    {
        _lines.CreateTileSheet(UIObjectClick, UIObjectEnter, UIObjectExit, gameObject.Transform);
    }
    
    private void UIObjectClick(UIEvent tileClick)
    {
        GameObject clicked = ((UIEventInfo)tileClick).GameObject;
        TileDataBehaviour data = clicked.GetComponent<TileDataBehaviour>();

        _lines.Click(data.X, data.Y);
    }

    private void UIObjectEnter(UIEvent tileEnter)
    {
        GameObject entered = ((UIEventInfo)tileEnter).GameObject;
        TileDataBehaviour data = entered.GetComponent<TileDataBehaviour>();

        if (data == null) return;

        if (_lines.CanMove(data.X, data.Y) == false & data.SpriteIndex == 0 & _lines.GetSelected() != null)
        {
            SelectableButton highlighted = entered.GetComponent<SelectableButton>();
            highlighted.SetCurrentColor(_warningColor);
            highlighted.HighlightedColor = _warningColor;
        }
    }

    private void UIObjectExit(UIEvent tileExit)
    {
        _lines.ClearUsed();

        GameObject exited = ((UIEventInfo)tileExit).GameObject;
        SelectableButton selectableButton = exited.GetComponent<SelectableButton>();

        if (selectableButton != null & selectableButton.Selected == false)
        {
            selectableButton.RestoreColor();
            selectableButton.RestoreHighlightedColor();
        }
    }
}