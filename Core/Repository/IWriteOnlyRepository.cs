public interface IWriteOnlyRepository<in T> 
{
    void Add(T newItem);
    void Remove(T item);
    bool Contains(T item);
    void Cleanup();
}
