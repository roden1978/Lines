using Microsoft.Xna.Framework;
using System;

public class Animator : Component, IUpdate
{
    public event Action NoLoopAnimationEnd;
    public Sprite CurrentSprite => _currentSprite;
    public bool _play;
    private readonly IDraw _drawComponent;
    private AnimationController _animationController;
    private int _index = 0;
    private int _prevIndex = 0;
    private double _timeStep = 0;
    private Sprite _currentSprite;
    private Animation _prevAnimation;
    private Animation _currentAnimation;
    public Animator(IDraw drawComponent, AnimationController animationController, bool playOnAwake = true)
    {
        _drawComponent = drawComponent;
        _animationController = animationController;
        _play = playOnAwake;
        _currentAnimation = _animationController.CurrentAnimation;
        _currentSprite = _currentAnimation.Sequence[0];
    }
   
    public void Update(GameTime gameTime)
    {
        if(!Active & !gameObject.Active) return;

        _animationController.Update(gameTime);

        if (_currentAnimation != _animationController.CurrentAnimation)
            _currentAnimation = _animationController.CurrentAnimation;

        if (_prevAnimation != _currentAnimation)
        {
            _index = 0;
            SetPlay(true);
        }

        if (_play & gameObject.Active & Active)
        {
            PlayAnimation(gameTime);
        }

        _prevAnimation = _currentAnimation;
    }

    private void PlayAnimation(GameTime gameTime)
    {
        if (_prevIndex != _index)
        {
            _currentSprite = _currentAnimation.Sequence[_index];
            _drawComponent.Sprite = _currentSprite;
        }

        if (_timeStep >= _currentAnimation.TimeStepMilliseconds)
        {
            _prevIndex = _index;
            _index++;
            _timeStep = 0;
        }

        _timeStep += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (_index == _currentAnimation.Sequence.Length & _currentAnimation.Loop)
            Reset();

        if (_index == _currentAnimation.Sequence.Length & _currentAnimation.Loop == false)
            {
                SetPlay(false);
                Reset();
                NoLoopAnimationEnd?.Invoke();
            }
    }

    public override void SetActive(bool value)
    {
        SetPlay(value);
        SetActive(value);
    }

    private void SetPlay(bool value) => _play = value;

    public void Reset() => _index = 0;

    public override void Destroy()
    {
        if (_animationController != null)
        {
            _animationController.Destroy();
            _animationController = null;
        }
        if (_drawComponent != null)
        {
            _drawComponent.Sprite.Destroy();
        }
        if (_prevAnimation != null)
        {
            _prevAnimation.Destroy();
            _prevAnimation = null;
        }
        if (_currentAnimation != null)
        {
            _currentAnimation.Destroy();
            _currentAnimation = null;
        }
        _currentSprite = null;
    }
}
