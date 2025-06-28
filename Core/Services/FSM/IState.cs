using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IState
{
    public void Enter();
    public void Update(GameTime gameTime);
    public void Draw(SpriteBatch spriteBatch);
    public void Exit();
}

