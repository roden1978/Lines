using Microsoft.Xna.Framework;

public interface IInteractable : IDraw
{
    bool Interactable { get; set;}
    Rectangle InteractableArea { get;}
    bool Pointing { get;}
    bool Selected {get;}
    void OnPointerClickHandler(UIEventInfo sender, MouseEventArgs mouseEventArgs);
    void OnPointerEnterHandler(UIEventInfo sender, MouseEventArgs mouseEventArgs);
    void OnPointerExitHandler(UIEventInfo sender, MouseEventArgs mouseEventArgs);
}