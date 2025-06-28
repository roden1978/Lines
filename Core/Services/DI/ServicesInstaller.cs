using Autofac;

public class ServicesInstaller : Module
{
    protected override void Load(ContainerBuilder container)
    {
        RegisterServices(container);
    }
    private void RegisterServices(ContainerBuilder container)
    {
        container.RegisterType<ContentLoadService>().As<IContentLoadService>().SingleInstance();
        container.RegisterType<ContentProvider>().As<IContentProvider>().As<IStartable>().SingleInstance();
        container.RegisterType<SaveLoadService>().As<ISaveLoadService>().SingleInstance();
        container.RegisterType<DBService>().As<IDBService>().SingleInstance();
        container.RegisterType<PersistentProgressService>().As<IPersistentProgressService>().SingleInstance(); //.As<IStartable>()
        container.RegisterType<LeaderboardService>().As<ILeaderboardService>().As<IStartable>().SingleInstance(); 
        container.RegisterType<IdentifierService>().As<IIdentifierService>().SingleInstance();
        
        //Uncomment for use
        //container.RegisterType<KeyboardInput>().As<IInputService>().SingleInstance();
        
        //Uncomment for use
        //container.RegisterType<PoolService>().AsSelf().SingleInstance();
        
        //Uncomment for use
        //container.RegisterType<CollisionSystem>().AsSelf().SingleInstance();
        
        //Uncomment for use
        //container.RegisterType<GravitySystem>().AutoActivate();
    }
}
