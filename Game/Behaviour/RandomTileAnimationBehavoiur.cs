using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Xna.Framework;

public class RandomTileAnimationBehavoiur : Component, IStart, IUpdate, ICanvasComponent
{
    private readonly LinesModel _lines;
    private readonly IContentProvider _contentProvider;
    private bool _isGameStarted;
    
    public RandomTileAnimationBehavoiur(LinesModel lines, IContentProvider contentProvider)
    {
        _lines = lines;
        _contentProvider = contentProvider;
    }

    public override void Start()
    {
        _lines.GameStarted += OnGameStart;
        _lines.GameOver += OnGameOver;
    }

    private void OnGameOver()
    {
        OnGameStart(false);
    }

    public void Update(GameTime gameTime)
    {
        SetRandomColor();
    }
    private void OnGameStart(bool value)
    {
        _isGameStarted = value;
    }
    private void SetRandomColor()
    {
        if (_isGameStarted) return;
        int index = _contentProvider.GetRandomIndex();
        int x = index % Settings.TilesCount;
        int y = index / Settings.TilesCount;

        TileData tile = _lines.GetTileDataByCoords(x, y);
        tile.SpriteIndex = _contentProvider.GetRandomTileIndex(all: true);
        SelectableButton button = _lines.GetTileByCoords(x, y);
        button.Sprite = _contentProvider.GetTileSpriteByIndex(tile.SpriteIndex);
    }
    

    public override void Destroy()
    {
        _lines.GameStarted -= OnGameStart;
        _lines.GameOver -= OnGameOver;
    }
}
