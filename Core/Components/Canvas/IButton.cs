using System;
using Microsoft.Xna.Framework;
public interface IButton : IInteractable, ICanvasComponent
{
    Color NormalColor { get; }
    Color HighlightedColor { get; }
    Color PressedColor { get; }
    Action<UIEvent> OnClick {get;}
    Action<UIEvent> OnEnter {get;}
    Action<UIEvent> OnExit {get;}
}
