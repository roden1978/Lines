public interface IPersistentProgressService
{
    Result CurrentResult {get;}
    PersistentData Load();
    void Save(Result result);
}
