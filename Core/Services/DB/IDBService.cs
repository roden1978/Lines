using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDBService
{
    IReadOnlyList<Result> Results {get;} 
    Task<bool> Save(Result result, bool insert);
    Task<bool> Get();
}
