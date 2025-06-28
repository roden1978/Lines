using System;

public class AnimationTransition : IAnimationTransition, IDestroy
{
    public Func<bool> Condition { get; private set;}
    public string To { get; }

    public AnimationTransition(string to, Func<bool> condition)
    {
        To = to;
        Condition = condition;
    }

    public void Destroy()
    {
        Condition = null;
    }
}
