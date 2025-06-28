using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class MouseEventSystem : IUpdate
{
    public bool Active { get; private set; } = true;
    public event EventHandler<MouseEventArgs> PositionUpdate;
    public event EventHandler<MouseEventArgs> ClickUpdate;
    public event EventHandler<MouseEventArgs> StateUpdate;
    private Point _position;
    private Point _prevPosition;
    private Rectangle _mouseRectangle;
    private ButtonState _leftButtonState;
    private ButtonState _prevLeftButtonState;
    private MouseEventArgs _mouseEventArgs;
    public MouseEventSystem()
    {
        _mouseRectangle = new Rectangle(_position.X, _position.Y, 1, 1);
        _mouseEventArgs = new MouseEventArgs
        {
            MouseCurrentPosition = Point.Zero,
            State = ButtonState.Released
        };
    }
    private void UpdatePosition()
    {
        _position.X = Mouse.GetState().X;
        _position.Y = Mouse.GetState().Y;

        _mouseRectangle.X = _position.X;
        _mouseRectangle.Y = _position.Y;

        if (_prevPosition != _position)
        {
            _mouseEventArgs.MouseCurrentPosition = _position;
            PositionUpdate?.Invoke(this, _mouseEventArgs);
        }

        _prevPosition = _position;
    }
    public void Update(GameTime gameTime)
    {
        UpdatePosition();
        UpdateClick();
    }
    private void UpdateClick()
    {
        if (Active)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _leftButtonState = ButtonState.Pressed;
                _prevLeftButtonState = _leftButtonState;
                _mouseEventArgs.State = ButtonState.Pressed;
                StateUpdate?.Invoke(this, _mouseEventArgs);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released &
                    _prevLeftButtonState == ButtonState.Pressed)
            {
                _leftButtonState = ButtonState.Released;

                _mouseEventArgs.MouseCurrentPosition = _position;
                ClickUpdate?.Invoke(this, _mouseEventArgs);

                _mouseEventArgs.State = ButtonState.Released;
                StateUpdate?.Invoke(this, _mouseEventArgs);

                _prevLeftButtonState = _leftButtonState;
            }
        }
    }

    public void SetActive(bool value)
    {
        Active = value;
    }
}