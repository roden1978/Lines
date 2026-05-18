using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class StateMachine
{
    public IState CurrentState => _currentState;
    private readonly Dictionary<Type, List<Transition>> _transitions;
    private readonly List<Transition> _anyTransitions;
    private IState _currentState;
    private List<Transition> _currentTransitions;
    private static List<Transition> EmptyTransitions;
    private bool _isActive;

    public StateMachine()
    {
        _transitions = [];
        _currentTransitions = [];
        _anyTransitions = [];
        EmptyTransitions = [];
    }

    public void Update(GameTime gameTime)
    {
        if (_isActive == false) return;

        Transition transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        _currentState?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_isActive == false) return;

        _currentState?.Draw(spriteBatch);
    }

    public void SetState(IState state)
    {
        if (state == _currentState) return;
        if (_transitions.Count == 0)
            return;
        //Console.WriteLine($"Add transitions before");
        _currentState?.Exit();

        _currentState = state;
        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);

        _currentTransitions ??= EmptyTransitions;

        _currentState.Enter();
    }

    public StateMachine AddTransition(IState from, IState to, Func<bool> condition)
    {
        bool result = _transitions.TryGetValue(from.GetType(), out List<Transition> value);
        if (result == false)
        {
            value = [];

            Type type = from.GetType();
            _transitions[type] = value;
        }

        value.Add(new Transition(to, condition));
        return this;
    }

    public StateMachine AddAnyTransition(IState to, Func<bool> condition)
    {
        _anyTransitions.Add(new Transition(to, condition));
        return this;
    }

    private Transition GetTransition()
    {
        foreach (Transition transition in _anyTransitions)
            if (transition.Condition.Invoke())
                return transition;

        foreach (Transition transition in _currentTransitions)
            if (transition.Condition.Invoke())
                return transition;

        return null;
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }
}