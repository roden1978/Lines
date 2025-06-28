using System;

[Serializable]
public class Result
{
    public string PlayerName = string.Empty;
    public int Value = 0;
    public string Game = string.Empty;

    public override string ToString() => $"{PlayerName} {Value} {Game}";
}