public class EffectControllerFactory : IFactory<GameObject>
{
    public string Name => "EffectControllerFactory";
    private readonly EffectController _effectController;

    public EffectControllerFactory(EffectController effectController)
    {
        _effectController = effectController;
    }
    
    public GameObject Create()
    {
        GameObject effectController = new("EffectController");

        effectController.AddComponent(_effectController);

        return effectController;
    }
}
