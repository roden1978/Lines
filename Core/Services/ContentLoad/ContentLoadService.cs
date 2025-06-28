using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Lines;
using Microsoft.Xna.Framework.Graphics;

public class ContentLoadService : IContentLoadService
{
    private const byte BytesForPixel = 4;
    private readonly List<int> _namesLength = [];
    private readonly List<TextureTypes> _textureTypes = [];
    private readonly List<AccessTypes> _accessTypes = [];
    private readonly List<int> _widths = [];
    private readonly List<int> _heigths = [];
    private readonly List<int> _stringsLenght = [];
    private IGraphicsDeviceProvider _graphicsDeviceProvider;

    public ContentLoadService(IGraphicsDeviceProvider graphicsDeviceProvider)
    {
        _graphicsDeviceProvider = graphicsDeviceProvider;
    }

    public Texture2D LoadTexture(string path)
    {
        Texture2D texture2D = default;

        using (var stream = new FileStream(path, FileMode.Open))
        {
            texture2D = Texture2D.FromStream(_graphicsDeviceProvider.GraphicsDevice, stream);
        }

        return texture2D;
    }

    public ImmutableDictionary<TextureTypes, Texture2D> LoadConvertedTextures()
    {
        Dictionary<TextureTypes, Texture2D> textures = [];

        if (File.Exists("content.dat"))
        {
            using (var stream = File.Open("content.dat", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.ASCII, false))
                {
                    var count = reader.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        var lenght = reader.ReadInt32();
                        _namesLength.Add(lenght);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        string[] name = new string[count];
                        name[i] = reader.ReadString();
                        _textureTypes.Add((TextureTypes)Enum.Parse(typeof(TextureTypes), name[i]));
                    }

                    for (int i = 0; i < count; i++)
                    {
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        _widths.Add(width);
                        _heigths.Add(height);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        byte[] tex = reader.ReadBytes(_widths[i] * _heigths[i] * BytesForPixel);

                        Texture2D newTexture = new(_graphicsDeviceProvider.GraphicsDevice, _widths[i], _heigths[i], false, SurfaceFormat.Color);
                        newTexture.SetData(tex);

                        textures.Add(_textureTypes[i], newTexture);
                    }
                }
            }
        }

        return textures.ToImmutableDictionary();
    }

    public ImmutableDictionary<AccessTypes, string> LoadConvertedAccessDBStrings()
    {
        Dictionary<AccessTypes, string> accessStrings = [];


        if (File.Exists("connection.dat"))
        {
            using var stream = File.Open("connection.dat", FileMode.Open);
            using var reader = new BinaryReader(stream, Encoding.ASCII, false);
            var count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                var lenght = reader.ReadInt32();
                _namesLength.Add(lenght);
            }

            for (int i = 0; i < count; i++)
            {
                string[] name = new string[count];
                name[i] = reader.ReadString();
                _accessTypes.Add((AccessTypes)Enum.Parse(typeof(AccessTypes), name[i]));
            }

            for (int i = 0; i < count; i++)
            {
                int lenght = reader.ReadInt32();
                _stringsLenght.Add(lenght);
            }
            
            for (int i = 0; i < count; i++)
            {
                int[] result = [];
                for (int j = 0; j < _stringsLenght[i]; j++)
                {
                    int value = reader.ReadInt32();
                    result = [.. result, value];
                }
                byte[] bytes = result.Select(x => Convert.ToByte(~x)).ToArray();
                var str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                accessStrings.Add(_accessTypes[i], str);
            }
        }

        return accessStrings.ToImmutableDictionary();
    }

    public Texture2D CreateDefaultTexture2D()
    {
        byte[] tex = new byte[32 * 32 * BytesForPixel].Select(x => x = 255).ToArray();
        Texture2D newTexture = new(_graphicsDeviceProvider.GraphicsDevice, 32, 32, false, SurfaceFormat.Color);
        newTexture.SetData(tex);
        
        return newTexture;
    }

    public Sprite CreateDefaultSprite()
    {
        return new(CreateDefaultTexture2D());
    }
}