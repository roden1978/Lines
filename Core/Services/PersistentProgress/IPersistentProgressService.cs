using System.Collections.Generic;

public interface IPersistentProgressService
{
    void Load();
    void Save(Result result);
}
