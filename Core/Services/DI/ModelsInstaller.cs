using Autofac;

public class ModelsInstaller : Module
{
    protected override void Load(ContainerBuilder container)
    {
        RegisterModels(container);
    }
    private void RegisterModels(ContainerBuilder container)
    {
        container.RegisterType<LinesModel>().As<IScoreProvider>().AsSelf().SingleInstance();
        container.RegisterType<GameModel>().AsSelf().SingleInstance();
    }
}
