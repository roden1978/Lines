using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class TileFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;
    private readonly IIdentifierService _identifierService;
    private SpriteOrder _tiles;
    public TileFactory(IContentProvider contentProvider, IIdentifierService identifierService)
    {
        _contentProvider = contentProvider;
        _identifierService = identifierService;
    }
    public GameObject Create()
    {
        if (_tiles is null)
            GenerateTilesSpriteOrder();

        GameObject tile = new($"Tile {_identifierService.Next()}");

        Sprite sprite = _tiles.GetSpriteByIndex(0);
        var selectableButton = new SelectableButton(sprite, Color.White, Color.LightBlue, Color.LightBlue, Color.LightBlue)
        {
            Interactable = false
        };
        selectableButton.SetNotInteractableTransparent(1f);
        
        tile
            .AddComponent(new CanvasHandler())
            .AddComponent(selectableButton)
            .AddComponent(new TileDataBehaviour()
            {
                SpriteIndex = 0
            });
        return tile;
    }

    private void GenerateTilesSpriteOrder()
    {
        Texture2D texture = _contentProvider.GetTextureByType(TextureTypes.Tiles);
        _tiles = new SpriteOrder(texture, texture.Width / Settings.TilesWidth, Settings.TilesHeight);
    }
}
