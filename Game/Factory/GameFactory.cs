using System.Collections.Generic;

public class GameFactory : IFactory<IReadOnlyList<GameObject>>
{
    public string Name => GetType().Name;
    private readonly GamePanelFactory _gamePanelFactory;
    private readonly InfoPanelFactory _infoPanelFactory;
    private readonly EffectControllerFactory _effectControllerFactory;

    public GameFactory(GamePanelFactory gamePanelFactory, InfoPanelFactory infoPanelFactory, EffectControllerFactory effectControllerFactory)
    {
        _gamePanelFactory = gamePanelFactory;
        _infoPanelFactory = infoPanelFactory;
        _effectControllerFactory = effectControllerFactory;
    }
    public IReadOnlyList<GameObject> Create()
    {
        GameObject infoPanel = _infoPanelFactory.Create();
        GameObject gamePanel = _gamePanelFactory.Create();
        GameObject effectController = _effectControllerFactory.Create();
        
        return [infoPanel, gamePanel, effectController];
    }
}
