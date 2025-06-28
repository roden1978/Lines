using System.Collections.Immutable;
using Microsoft.Xna.Framework.Graphics;

public interface IContentLoadService
{
    Texture2D LoadTexture(string path);
    ImmutableDictionary<TextureTypes, Texture2D> LoadConvertedTextures();
    ImmutableDictionary<AccessTypes, string> LoadConvertedAccessDBStrings();
    Texture2D CreateDefaultTexture2D();
    Sprite CreateDefaultSprite();
}