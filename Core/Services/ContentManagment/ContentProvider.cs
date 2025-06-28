using System;
using System.Collections.Immutable;
using Autofac;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IContentProvider : IStartable
{
    void LoadAll();
    Texture2D GetTextureByType(TextureTypes type);
    Sprite GetTileSpriteByIndex(int index);
    string GetAccessStringByType(AccessTypes type);
    Sequence FontSequence {get;}
    int TilesCount {get;}
    int GetRandomTileIndex();
    int GetRandomIndex();
    Sprite CreateDefaultSprite();
}

public class ContentProvider : IContentProvider
{
    public Sequence FontSequence => _fontSequence;
    public int TilesCount => _tiles.Length;
    private readonly IContentLoadService _contentLoadService;
    private SpriteOrder _tiles;
    private Sequence _fontSequence;
    private ImmutableDictionary<TextureTypes, Texture2D> _textures;
    private ImmutableDictionary<AccessTypes, string> _accessStrings;
    private Random _random;

    public ContentProvider(IContentLoadService contentLoadService)
    {
        _contentLoadService = contentLoadService;
        _random = new();
    }

    public void LoadAll()
    {
        _textures = _contentLoadService.LoadConvertedTextures();
        _accessStrings = _contentLoadService.LoadConvertedAccessDBStrings();
        _tiles = new SpriteOrder(_textures[TextureTypes.Tiles], _textures[TextureTypes.Tiles].Width / Settings.TilesWidth, Settings.TilesHeight);
        _fontSequence = new Sequence(new SpriteOrder(_textures[TextureTypes.Font], Point.Zero, 10, 9, new Rectangle(0, 0, 480, 414))); 
    }


    public Texture2D GetTextureByType(TextureTypes type)
    {
        if (_textures.TryGetValue(type, out Texture2D value))
            return value;

        throw new ArgumentException($"Texture type {type} is not exist!");
    }

    public string GetAccessStringByType(AccessTypes type)
    {
        if (_accessStrings.TryGetValue(type, out string value))
            return value;

        throw new ArgumentException($"Texture type {type} is not exist!");
    }

    public Sprite GetTileSpriteByIndex(int index)
    {
        return _tiles.GetSpriteByIndex(index);
    }

    public int GetRandomTileIndex()
    {
        return _random.Next(1, TilesCount);
    }

    public int GetRandomIndex()
    {
        return _random.Next(0, Settings.TilesCount * Settings.TilesCount);
    }
    
    public void Start()
    {
        LoadAll();
    }

    public Sprite CreateDefaultSprite()
    {
        return _contentLoadService.CreateDefaultSprite();
    }
}
