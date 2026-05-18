using System;

public interface ITransition
{
    Func<bool> Condition { get; }
}

public interface IStateTransition : ITransition
{
    IState To { get; }
}
public interface IAnimationTransition : ITransition
{
    string To { get; }
}

public interface IFromStateTransition : IStateTransition
{
    IState From { get; }
}