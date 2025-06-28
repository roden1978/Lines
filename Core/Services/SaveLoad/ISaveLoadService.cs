public interface ISaveLoadService
{
    PersistentData Load();
    void Save(PersistentData data);
}
