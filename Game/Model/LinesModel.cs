using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
public class LinesModel : IScoreProvider
{
    public StringSource StringSource => _stringSource;
    public event Action<bool> GameStarted;
    public event Action<IEnumerable<Transform2D>> SpalshEffectPositions;
    public event Action GameOver;
    public IEnumerable<TileData> TilesData => _tilesWithCoords.Keys;
    private int _scores;

    private readonly TileFactory _tileFactory;
    private readonly IContentProvider _contentProvider;
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly Random _random;
    private readonly ImmutableDictionary<TileData, SelectableButton>.Builder _builder;
    private ImmutableDictionary<TileData, SelectableButton> _tilesWithCoords;
    private int _fromX, _fromY;
    private SelectableButton _selectedButton;
    private readonly ImmutableDictionary<TileData, UIImage>.Builder _listBuilder;
    private ImmutableDictionary<TileData, UIImage> _nextRandomTiles;
    private StringSource _stringSource;
    public LinesModel(TileFactory tileFactory, IContentProvider contentProvider, IPersistentProgressService persistentProgressService)
    {
        _tileFactory = tileFactory;
        _contentProvider = contentProvider;
        _persistentProgressService = persistentProgressService;
        _builder = ImmutableDictionary.CreateBuilder<TileData, SelectableButton>();
        _listBuilder = ImmutableDictionary.CreateBuilder<TileData, UIImage>();
        _random = new Random();
        _stringSource = new()
        {
            Value = _scores.ToString()
        };
    }

    public void CreateTileSheet(Action<UIEvent> onClick, Action<UIEvent> onEnter, Action<UIEvent> onExit, Transform2D parent)
    {
        for (int i = 0; i < Settings.TilesCount * Settings.TilesCount; i++)
        {
            GameObject tile = _tileFactory.Create();
            tile.Transform.Parent = parent;

            int x = i % Settings.TilesCount;
            int y = i / Settings.TilesCount;

            SelectableButton button = tile.GetComponent<SelectableButton>();
            button.OnClick = onClick;
            button.OnEnter = onEnter;
            button.OnExit = onExit;

            TileData data = new()
            {
                SpriteIndex = 0,
                X = x,
                Y = y
            };
            _builder.Add(data, button);
        }
        _tilesWithCoords = _builder.ToImmutableDictionary();
    }

    public SelectableButton GetTileByCoords(int x, int y)
    {
        if (x >= 0 & x < Settings.TilesCount & y >= 0 & y < Settings.TilesCount)
            return _tilesWithCoords.FirstOrDefault(t => t.Key.X == x & t.Key.Y == y).Value;

        return null;
    }

    public TileData GetTileDataByCoords(int x, int y)
    {
        if (x >= 0 & x < Settings.TilesCount & y >= 0 & y < Settings.TilesCount)
            return _tilesWithCoords.Keys.FirstOrDefault(t => t.X == x & t.Y == y);

        return null;
    }

    public TileData GetTileDataByButton(SelectableButton button) => 
        _tilesWithCoords.FirstOrDefault(x => x.Value.GetHashCode() == button.GetHashCode()).Key;

    public void Start()
    {
        GameStarted?.Invoke(true);

        ClearMap();
        SetInteractableForSelectableButtons(true);
        GenerateNextRandomTilesObjects();
        AddRandomBallsToMap();
        GenerateNextRandomTilesObjects();
        ResetScores();
    }

    private void ResetScores()
    {
        _scores = 0;
        _stringSource.Value = _scores.ToString();
    }

    public void RestartGame()
    {
        GameStarted?.Invoke(false);

        SetInteractableForSelectableButtons(false);
        ClearMap();
        ClearNextRandomTiles();
        ClearUsed();
        ResetScores();
    }

    private void SetInteractableForSelectableButtons(bool value) =>
        _tilesWithCoords.Values.ToList().ForEach(x => x.Interactable = value);

    public void GenerateNextTilesViews(Transform2D parent)
    {
        for (int i = 0; i < Settings.RandomTilesCount; i++)
        {
            GameObject tileImage = new($"New Tile {i}");
            Sprite sprite = _contentProvider.GetTileSpriteByIndex(0);
            UIImage uIImage = new(sprite);
            tileImage
                .AddComponent(new CanvasHandler())
                .AddComponent(uIImage);

            tileImage.Transform.Parent = parent;

            _listBuilder.Add(new(), uIImage);
        }

        _nextRandomTiles = _listBuilder.ToImmutableDictionary();
    }

    public void GenerateNextRandomTilesObjects()
    {
        for (int i = 0; i < Settings.RandomTilesCount; i++)
        {
            int randomIndexTile = _contentProvider.GetRandomTileIndex();
            Sprite sprite = _contentProvider.GetTileSpriteByIndex(randomIndexTile);
            _nextRandomTiles.Values.ElementAt(i).Sprite = sprite;
            _nextRandomTiles.Keys.ElementAt(i).SpriteIndex = randomIndexTile;
        }
    }
    private void ClearNextRandomTiles()
    {
        for (int i = 0; i < Settings.RandomTilesCount; i++)
        {
            Sprite sprite = _contentProvider.GetTileSpriteByIndex(0);
            _nextRandomTiles.Values.ElementAt(i).Sprite = sprite;
        }
    }
    public void Click(int x, int y)
    {
        TileData tileData = GetTileDataByCoords(x, y);
        SelectableButton button = GetTileByCoords(x, y);
        
        if (tileData.SpriteIndex > 0)
        {
            if (_selectedButton == null)
                SelectTile(x, y);
            else
            {
                int selectedX = tileData.X;
                int selectedY = tileData.Y;

                if (selectedX == x & selectedY == y)
                    UnSelectTile();
                else
                {
                    UnSelectTile();
                    SelectTile(x, y);
                }
            }
        }
        else
        {
            MoveTileTo(x, y);
            button.RestoreColor();

            if (_selectedButton != null)
                button.SetCurrentColor(button.HighlightedColor);
        }
    }

    private void UnSelectTile()
    {
        _selectedButton?.RestoreColor();
        _selectedButton = null;
    }

    private void SelectTile(int x, int y)
    {
        SelectableButton button = GetTileByCoords(x, y);
        TileData tileData = GetTileDataByCoords(x, y);
        if (tileData.SpriteIndex == 0)
        {
            button.RestoreColor();
            return;
        }

        _fromX = x;
        _fromY = y;
        _selectedButton = button;
    }
    private void MoveTileTo(int x, int y)
    {
        if (_selectedButton == null) return;

        if (!CanMove(x, y)) return;

        ClearUsed();

        int index = GetTileDataByCoords(_fromX, _fromY).SpriteIndex;

        SetMap(x, y, index);

        SetMap(_fromX, _fromY, 0);

        _selectedButton.RestoreColor();
        _selectedButton = null;

        if (false == CutLines())
        {
            AddRandomBallsToMap();
            CutLines();
            GenerateNextRandomTilesObjects();
        }

        if (IsGameOver())
        {
            _persistentProgressService.Save(new()
            {
                PlayerName = Settings.PlayerName,
                Value = _scores,
                Game = Settings.GameName
            });
            GameOver?.Invoke();
        }
    }

    private bool CutLines()
    {
        int count = 0;

        foreach (TileData item in _tilesWithCoords.Keys)
        {
            count += CutLine(item.X, item.Y, 1, 0);
            count += CutLine(item.X, item.Y, 0, 1);
            count += CutLine(item.X, item.Y, 1, 1);
            count += CutLine(item.X, item.Y, -1, 1);
        }

        if (count > 0)
        {
            IEnumerable<TileData> cleared = _tilesWithCoords.Keys.Where(x => x.Clear == true);

            SpalshEffectPositions?.Invoke(cleared.Select(x => GetTileByCoords(x.X, x.Y).gameObject.Transform));

            _scores += cleared.Count();
            _stringSource.Value = _scores.ToString();

            foreach (TileData tile in cleared)
            {
                tile.Clear = false;
                SetMap(tile.X, tile.Y, 0);
            }
            return true;
        }
        return false;
    }

    private int CutLine(int x0, int y0, int sx, int sy)
    {
        int spriteIndex = GetTileDataByCoords(x0, y0).SpriteIndex;

        if (spriteIndex == 0) return 0;

        int count = 0;

        for (int x = x0, y = y0; 
                                GetTileDataByCoords(x, y)?.SpriteIndex == spriteIndex;
                                                                                        x += sx, y += sy)
        {
            count++;
        }

        if (count < Settings.WinCount) return 0;

        for (int x = x0, y = y0;
                                GetTileDataByCoords(x, y)?.SpriteIndex == spriteIndex;
                                                                                        x += sx, y += sy)
        {
            GetTileDataByCoords(x, y).Clear = true;
        }

        return count;
    }

    public bool CanMove(int toX, int toY)
    {
        Walk(_fromX, _fromY, true);
        return GetTileDataByCoords(toX, toY).Used;
    }

    private void Walk(int x, int y, bool start = false)
    {
        if (false == start)
        {
            if (GetTileByCoords(x, y) == null) return;

            int spriteIndex = GetTileDataByCoords(x, y).SpriteIndex;
            if (spriteIndex > 0) return;

            bool used = GetTileDataByCoords(x, y).Used;
            if (used) return;
        }

        GetTileDataByCoords(x, y).Used = true;

        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
    }

    public void ClearUsed()
    {
        foreach (TileData tile in _tilesWithCoords.Keys)
            tile.Used = false;
    }

    public void SetMap(int x, int y, int index)
    {
        SelectableButton button = GetTileByCoords(x, y);
        button.Sprite = _contentProvider.GetTileSpriteByIndex(index);
        GetTileDataByCoords(x, y).SpriteIndex = index;
    }

    public void ClearMap()
    {
        foreach ((TileData tile, SelectableButton button) in _tilesWithCoords)
        {
            button.Sprite = _contentProvider.GetTileSpriteByIndex(0);
            button.RestoreColor();
            tile.SpriteIndex = 0;
        }
    }

    private void AddRandomBallsToMap()
    {
        for (int i = 0; i < Settings.RandomTilesCount; i++)
            AddRandomTileToMap(i);
    }

    private void AddRandomTileToMap(int index)
    {
        int x, y;
        int loop = Settings.TilesCount * Settings.TilesCount;
        do
        {
            x = _random.Next(Settings.TilesCount);
            y = _random.Next(Settings.TilesCount);
            if (--loop <= 0) return;
        } while (GetTileDataByCoords(x, y).SpriteIndex > 0);

        int spriteIndex = _nextRandomTiles.Keys.ElementAt(index).SpriteIndex;
        SetMap(x, y, spriteIndex);
    }

    public SelectableButton GetSelected() => _selectedButton;

    private bool IsGameOver() => 
        false == _tilesWithCoords.Keys.Any(x => x.SpriteIndex == 0);
}