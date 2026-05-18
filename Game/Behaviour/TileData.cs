public class TileData
{
    public int X;
    public int Y;
    public int SpriteIndex;
    public bool Clear = false;
    public bool Used = false;

    public override string ToString() => $"X: {X}, Y: {Y}, Sprite index: {SpriteIndex}, clear: {Clear}, used: {Used}";
}
