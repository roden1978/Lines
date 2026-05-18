using System.Collections.Generic;

public interface IReadOnlyRepository<out T> 
{
        IReadOnlyList<T> FindAll();
}
