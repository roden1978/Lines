using Autofac;

public class FactoryIstaller : Module
{
    protected override void Load(ContainerBuilder container)
    {
        RegisterFactory(container);
    }

    private void RegisterFactory(ContainerBuilder container)
    {
        container.RegisterType<TileFactory>().AsSelf().SingleInstance();
        container.RegisterType<GamePanelFactory>().AsSelf().SingleInstance();
        
        container.RegisterType<TitleFactory>().AsSelf().SingleInstance();
        container.RegisterType<NextLabelFactory>().AsSelf().SingleInstance();
        container.RegisterType<ScoreLabelFactory>().AsSelf().SingleInstance();
        container.RegisterType<ScoreTextFactory>().AsSelf().SingleInstance();
        container.RegisterType<StartButtonFactory>().AsSelf().SingleInstance();
        container.RegisterType<RestartButtonFactory>().AsSelf().SingleInstance();
        container.RegisterType<GameOverLabelFactory>().AsSelf().SingleInstance();
        container.RegisterType<LeaderboardFactory>().AsSelf().SingleInstance();
        container.RegisterType<InfoPanelFactory>().AsSelf().SingleInstance();

        container.RegisterType<SplashFactory>().AsSelf().SingleInstance();
        container.RegisterType<EffectControllerFactory>().AsSelf().SingleInstance();

        container.RegisterType<GameFactory>().AsSelf().SingleInstance();
        //container.RegisterType<GameFactory>().AsSelf().SingleInstance();
    }
}
