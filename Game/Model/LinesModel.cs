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
    public IEnumerable<TileDataBehaviour> TilesData => _tilesWithCoords.Keys;
    private int _scores;
    
    private readonly TileFactory _tileFactory;
    private readonly IContentProvider _contentProvider;
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly Random _random;
    private readonly ImmutableDictionary<TileDataBehaviour, SelectableButton>.Builder _builder;
    private ImmutableDictionary<TileDataBehaviour, SelectableButton> _tilesWithCoords;
    private int _fromX, _fromY;
    private SelectableButton _selectedButton;
    private readonly ImmutableList<TileDataBehaviour>.Builder _listBuilder;
    private ImmutableList<TileDataBehaviour> _nextRandomTiles;
    private StringSource _stringSource;
    public LinesModel(TileFactory tileFactory, IContentProvider contentProvider, IPersistentProgressService persistentProgressService)
    {
        _tileFactory = tileFactory;
        _contentProvider = contentProvider;
        _persistentProgressService = persistentProgressService;
        _builder = ImmutableDictionary.CreateBuilder<TileDataBehaviour, SelectableButton>();
        _listBuilder = ImmutableList.CreateBuilder<TileDataBehaviour>();
        _random = new Random();
        _stringSource = new();
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

            TileDataBehaviour data = tile.GetComponent<TileDataBehaviour>();
            data.X = x;
            data.Y = y;

            _builder.Add(data, button);
        }

        _tilesWithCoords = _builder.ToImmutableDictionary();
    }

    public SelectableButton GetTileByCoords(int x, int y)
    {
        KeyValuePair<TileDataBehaviour, SelectableButton> value = default;

        if (x >= 0 & x < Settings.TilesCount & y >= 0 & y < Settings.TilesCount)
        {
            value = _tilesWithCoords.FirstOrDefault(t => t.Key.X == x & t.Key.Y == y);
            return _tilesWithCoords.GetValueOrDefault(value.Key);
        }

        return null;
    }

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
        _stringSource.Str = "0";
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
            var tileImage = new GameObject($"New Tile {i}");
            Sprite sprite = _contentProvider.GetTileSpriteByIndex(0);
            var tileData = new TileDataBehaviour()
            {
                SpriteIndex = 0
            };
            tileImage
            .AddComponent(new CanvasHandler())
            .AddComponent(new UIImage(sprite))
            .AddComponent(tileData);

            tileImage.Transform.Parent = parent;

            _listBuilder.Add(tileData);
        }

        _nextRandomTiles = _listBuilder.ToImmutableList();
    }

    public void GenerateNextRandomTilesObjects()
    {
        for (int i = 0; i < Settings.RandomTilesCount; i++)
        {
            int randomIndexTile = _contentProvider.GetRandomTileIndex();
            Sprite sprite = _contentProvider.GetTileSpriteByIndex(randomIndexTile);
            _nextRandomTiles[i].SpriteIndex = randomIndexTile;
            _nextRandomTiles[i].gameObject.GetComponent<UIImage>().Sprite = sprite;
        }
    }
    private void ClearNextRandomTiles()
    {
        for (int i = 0; i < Settings.RandomTilesCount; i++)
        {
            Sprite sprite = _contentProvider.GetTileSpriteByIndex(0);
            _nextRandomTiles[i].SpriteIndex = 0;
            _nextRandomTiles[i].gameObject.GetComponent<UIImage>().Sprite = sprite;
        }
    }
    public void Click(int x, int y)
    {
        SelectableButton button = GetTileByCoords(x, y);
        int index = button.gameObject.GetComponent<TileDataBehaviour>().SpriteIndex;

        if (index > 0)
        {
            if (_selectedButton == null)
                SelectTile(x, y, button);
            else
            {
                int selectedX = _selectedButton.gameObject.GetComponent<TileDataBehaviour>().X;
                int selectedY = _selectedButton.gameObject.GetComponent<TileDataBehaviour>().Y;

                if (selectedX == x & selectedY == y)
                    UnSelectTile();
                else
                {
                    UnSelectTile();
                    SelectTile(x, y, button);
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

    private void SelectTile(int x, int y, SelectableButton button)
    {
        if (button.gameObject.GetComponent<TileDataBehaviour>().SpriteIndex == 0)
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
        
        int index = _selectedButton.gameObject.GetComponent<TileDataBehaviour>().SpriteIndex;
        
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

        if(IsGameOver())
        {
          _persistentProgressService.CurrentResult.Value = _scores;
          
          GameOver?.Invoke();  
        } 
    }

    private bool CutLines()
    {
        int count = 0;

        foreach (TileDataBehaviour item in _tilesWithCoords.Keys)
        {
            count += CutLine(item.X, item.Y, 1, 0);
            count += CutLine(item.X, item.Y, 0, 1);
            count += CutLine(item.X, item.Y, 1, 1);
            count += CutLine(item.X, item.Y, -1, 1);
        }

        if (count > 0)
        {
            IEnumerable<TileDataBehaviour> cleared = _tilesWithCoords.Keys.Where(x => x.Clear == true);
            
            SpalshEffectPositions?.Invoke(cleared.Select(x => x.gameObject.Transform));
            
            _scores += cleared.Count();
            _stringSource.Str = _scores.ToString();

            foreach (TileDataBehaviour tile in cleared)
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
        int spriteIndex = GetTileByCoords(x0, y0).gameObject.GetComponent<TileDataBehaviour>().SpriteIndex;

        if (spriteIndex == 0) return 0;

        int count = 0;

        for (int x = x0, y = y0;
        GetTileByCoords(x, y)?.gameObject.GetComponent<TileDataBehaviour>().SpriteIndex == spriteIndex;
        x += sx, y += sy)
        {
            count++;
        }

        if (count < Settings.WinCount) return 0;

        for (int x = x0, y = y0;
                GetTileByCoords(x, y)?.gameObject.GetComponent<TileDataBehaviour>().SpriteIndex == spriteIndex;
                x += sx, y += sy)
        {
            GetTileByCoords(x, y).gameObject.GetComponent<TileDataBehaviour>().Clear = true;
        }

        return count;
    }

    public bool CanMove(int toX, int toY)
    {
        Walk(_fromX, _fromY, true);
        return GetTileByCoords(toX, toY).gameObject.GetComponent<TileDataBehaviour>().Used;
    }

    private void Walk(int x, int y, bool start = false)
    {
        if (false == start)
        {
            if (GetTileByCoords(x, y) == null) return;

            int spriteIndex = GetTileByCoords(x, y).gameObject.GetComponent<TileDataBehaviour>().SpriteIndex;
            if (spriteIndex > 0) return;

            bool used = GetTileByCoords(x, y).gameObject.GetComponent<TileDataBehaviour>().Used;
            if (used) return;
        }

        GetTileByCoords(x, y).gameObject.GetComponent<TileDataBehaviour>().Used = true;

        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
    }

    public void ClearUsed()
    {
        foreach (TileDataBehaviour tile in _tilesWithCoords.Keys)
            tile.Used = false;
    }

    public void SetMap(int x, int y, int index)
    {
        SelectableButton button = GetTileByCoords(x, y);
        button.Sprite = _contentProvider.GetTileSpriteByIndex(index);
        button.gameObject.GetComponent<TileDataBehaviour>().SpriteIndex = index;
    }

    public void ClearMap()
    {
        
        foreach ((TileDataBehaviour tile, SelectableButton button) in _tilesWithCoords)
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
        } while (GetTileByCoords(x, y).gameObject.GetComponent<TileDataBehaviour>().SpriteIndex > 0);

        int spriteIndex = _nextRandomTiles[index].SpriteIndex;
        SetMap(x, y, spriteIndex);
    }

    public SelectableButton GetSelected()
    {
        return _selectedButton;
    }

    private bool IsGameOver()
    {
        return false == _tilesWithCoords.Keys.Any(x => x.SpriteIndex == 0);
    }
}