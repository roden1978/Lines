using System.Collections.Generic;

public class Pool
{
    public string Name;
    public int Count => _repository.Count;
    public int Capacity { get; }
    public int Index;
    private Queue<GameObject> _repository { get; } = new Queue<GameObject>();

    public Pool(int capacity)
    {
        Capacity = capacity;
    }

    public GameObject GetPooledObject()
    {
        return _repository.Dequeue();
    }
    public GameObject GetFirst()
    {
        return _repository.Peek();
    }

    public void SetPooledObject(GameObject pooledObject)
    {
        _repository.Enqueue(pooledObject);
    }
}