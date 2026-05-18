using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lines;

public class Game1 : Game, IGraphicsDeviceProvider
{
    private readonly GraphicsDeviceManager _graphics;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        InitializeGraphicsDevice();

        Window.Title = "Lines";

        Bootstraper bootstraper = new(this);
        Components.Add(bootstraper);

        base.Initialize();
    }

    private void InitializeGraphicsDevice()
    {
        // TODO: Add your initialization logic here

        _graphics.GraphicsProfile = GraphicsProfile.Reach;
        _graphics.PreferredBackBufferWidth = Settings.ScreenWidth;
        _graphics.PreferredBackBufferHeight = Settings.ScreenHeight;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent()
    {
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.AntiqueWhite);

        base.Draw(gameTime);
    }
}
