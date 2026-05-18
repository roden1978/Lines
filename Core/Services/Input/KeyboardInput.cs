using Microsoft.Xna.Framework.Input;

public class KeyboardInput : IInputService
{
    private bool _isJump;
    public float Jump()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.W) && !_isJump)
        {
            _isJump = true;
            return -1;
        }

        if (Keyboard.GetState().IsKeyUp(Keys.W) && _isJump)
        {
            _isJump = false;
        }

        return 0;
    }

    public float Move()
    {
        if (Keyboard.GetState().IsKeyDown(Keys.A))
            return -1;
        else if (Keyboard.GetState().IsKeyDown(Keys.D))
            return 1;

        return 0;
    }

}
