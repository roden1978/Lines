using Microsoft.Xna.Framework.Graphics;

namespace Lines;

public interface IGraphicsDeviceProvider
{
    GraphicsDevice GraphicsDevice { get; }
}
