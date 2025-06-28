using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class MouseEventArgs : EventArgs
{
    public Point MouseCurrentPosition;
    public ButtonState State;
}