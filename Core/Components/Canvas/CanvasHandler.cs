using Microsoft.Xna.Framework;

public class CanvasHandler : Component, ICanvasComponent
{
    public IInteractable InteractableComponent;
    public int Width { get; set; }
    public int Height { get; set; }
    public CanvasHandler()
    {

    }
    public void OnClickUpdate(object sender, MouseEventArgs e)
    {
        if(InteractableComponent == null) return;
        bool contains = ContainsPointer(InteractableComponent, e.MouseCurrentPosition);
        if (contains & InteractableComponent?.Interactable == true & Active)
        {
            InteractableComponent?.OnPointerClickHandler(new UIEventInfo()
            {
                GameObject = gameObject,
                x = e.MouseCurrentPosition.X,
                y = e.MouseCurrentPosition.Y,
            }, e);
        }
    }

    public void OnPositionUpdate(object sender, MouseEventArgs e)
    {
            if(InteractableComponent == null) return;
            
            bool contains = ContainsPointer(InteractableComponent, e.MouseCurrentPosition);
            if (contains & InteractableComponent?.Pointing == false & InteractableComponent?.Interactable == true & Active)
                InteractableComponent.OnPointerEnterHandler(new UIEventInfo()
                {
                    GameObject = gameObject,
                    x = e.MouseCurrentPosition.X,
                    y = e.MouseCurrentPosition.Y,
                }, e);

            if (contains == false & InteractableComponent.Pointing)
                InteractableComponent?.OnPointerExitHandler(new UIEventInfo()
                {
                    GameObject = gameObject,
                    x = e.MouseCurrentPosition.X,
                    y = e.MouseCurrentPosition.Y,
                }, e);
    }

    public static bool ContainsPointer(IInteractable component, Point pointer) =>
        component.InteractableArea.Contains(pointer);
}
