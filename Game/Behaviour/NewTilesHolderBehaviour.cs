public class NewTilesHolderBehaviour : Component, IStart, ICanvasComponent
{
    private readonly LinesModel _lines;

    public NewTilesHolderBehaviour(LinesModel lines)
    {
        _lines = lines;
    }

    public override void Start()
    {
        _lines.GenerateNextTilesViews(gameObject.Transform);
    }
}
