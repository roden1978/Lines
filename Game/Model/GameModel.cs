public class GameModel
{
    private readonly LinesModel _lines;

    public GameModel(LinesModel lines)
    {
        _lines = lines;
    }

    public void StartGame() => _lines.Start();

    public void RestartGame() => _lines.RestartGame();
}