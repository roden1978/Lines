using System.Collections.Generic;
using System.Linq;

public class Repository<T> : IRepository<T>
{
    public IReadOnlyList<T> Container => _repository;
    private readonly List<T> _repository = [];
    public void Add(T item)
    {
        _repository.Add(item);
    }

    public void Remove(T item)
    {
        _repository.Remove(item);
    }

    public IReadOnlyList<T> FindAll()
    {
        return _repository;
    }

    public void Cleanup()
    {
        _repository.Clear();
    }

    public bool Contains(T item)
    {
        return _repository.Contains(item);
    }
}
