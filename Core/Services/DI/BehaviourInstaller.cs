using Autofac;

public class BehaviourInstaller : Module
{
    protected override void Load(ContainerBuilder container)
    {
        RegisterBehaviour(container);
    }

    private void RegisterBehaviour(ContainerBuilder container)
    {
        container.RegisterType<StartButtonBehaviour>().AsSelf().SingleInstance();
        container.RegisterType<RestartButtonBehaviour>().AsSelf().SingleInstance();
        container.RegisterType<RandomTileAnimationBehavoiur>().AsSelf().SingleInstance();
        container.RegisterType<GameBehaviour>().AsSelf().SingleInstance();
        container.RegisterType<GameOverLabelBehaviour>().AsSelf().SingleInstance();
        container.RegisterType<InfoPanelBehaviour>().AsSelf().SingleInstance();
        container.RegisterType<NewTilesHolderBehaviour>().AsSelf().SingleInstance();
        container.RegisterType<EffectController>().AsSelf().SingleInstance();
        container.RegisterType<Leaderboard>().AsSelf().SingleInstance();
    }
}
