using System.Collections.Generic;

public class EffectController : Component
{
    private readonly SplashFactory _splashFactory;
    private readonly Scene _scene;
    private readonly LinesModel _lines;
    private Animator _animator;
    private List<GameObject> _effects = [];
    private bool _isPlayEffect;

    public EffectController(LinesModel lines, SplashFactory splashFactory, Scene scene)
    {
        _lines = lines;
        _splashFactory = splashFactory;
        _scene = scene;
    }

    public override void Start() => _lines.SpalshEffectPositions += OnCreateSplashEffect;

    private void OnCreateSplashEffect(IEnumerable<Transform2D> transforms)
    {
        if (_isPlayEffect)
            OnNoLoopAnimationEnd();
        _effects = [];

        foreach (Transform2D transform in transforms)
        {
            GameObject newEffect = _splashFactory.Create();
            _scene.Register(newEffect);
            Splash splash = newEffect.GetComponent<Splash>();
            splash.Position = transform.AbsolutePosition;
            _effects.Add(newEffect);
            //Console.WriteLine($"Splash coords {splash.Position} name {newEffect.Name}");
        }

        _animator = _effects[^1].GetComponent<Animator>();
        _animator.NoLoopAnimationEnd += OnNoLoopAnimationEnd;
        _isPlayEffect = true;
    }


    private void OnNoLoopAnimationEnd()
    {
        _isPlayEffect = false;
        _animator.NoLoopAnimationEnd -= OnNoLoopAnimationEnd;
        foreach (GameObject effect in _effects)
            _scene.Unregister(effect);
    }
    public override void Destroy()
    {
        _lines.SpalshEffectPositions -= OnCreateSplashEffect;
    }
}
