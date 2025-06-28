using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public class PoolService
{
    private readonly List<IFactory<GameObject>> _factories = [];
    private readonly Dictionary<string, Pool> _pools = [];

    public void Initialize()
    {
        foreach (var factory in _factories)
        {
            Pool pool = new(3);
            
            for (int i = 0; i < pool.Capacity; i++)
            {
                GameObject pooledObject = factory.Create();
                pooledObject.Name = $"{factory.Name}({i})";
                pooledObject.Transform.Position = ResetPosition();
                pooledObject.SetActive(false);
                pool.SetPooledObject(pooledObject);
                pool.Index = i;
            }
            _pools.Add(pool.Name = factory.Name, pool);

        }
    }

    public GameObject GetPooledObject(string name)
    {
        GameObject pooledObject = default;

        if (_pools.TryGetValue(name, out Pool pool))
        {

            pooledObject = pool.GetFirst();

            if (pooledObject.Active)
            {
                int index = pool.Count - 1;
                var additional = _factories.FirstOrDefault(x => x.Name == name).Create();
                additional.Name = $"{additional.Name}({++index})";
                additional.Transform.Position = ResetPosition();
                pool.SetPooledObject(additional);
                //System.Console.WriteLine($"Name: {additional.Name}");
                return additional;
            }

            pooledObject = pool.GetPooledObject();
            pooledObject.SetActive(true);
            pool.SetPooledObject(pooledObject);
        }
        else
        {
            throw new KeyNotFoundException($"Pool with name {name} not found");
        }

        return pooledObject;
    }

    public void ReturnToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.Transform.Position = ResetPosition();
        gameObject.Transform.Parent = null;
    }

    public PoolService Add(IFactory<GameObject> factory)
    {
        _factories.Add(factory);
        return this;
    }

    private Vector2 ResetPosition()
    {
        return new Vector2(999, 999);
    }

    public void CleanUp()
    {
        _factories.Clear();
        _pools.Clear();
    }
}