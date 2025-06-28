using System;
using System.Collections.Generic;
using System.Linq;

public class Container<T> : IContainer<T>
{
    public int Count => Repository.Count;
    public Dictionary<Type, T> Repository { get; } = [];

    public K GetComponent<K>() where K : T
    {
        if (!Repository.TryGetValue(typeof(K), out T value))
            throw new Exception($"Component with type {typeof(K)} not found");

        return (K)value;
    }

    public bool TryGetComponent<K>(out K component) where K : T
    {
        if (!Repository.TryGetValue(typeof(K), out T value))
        {
            component = default;
            return false;
        }

        component = (K)value;

        return true;
    }

    public K Register<K>(K component) where K : T
    {
        var type = component.GetType();
        if (Repository.ContainsKey(type))
            throw new Exception("Type is exist");

        Repository[type] = component;
        return component;
    }

    public void Unregister<K>(K component) where K : T => 
        Repository.Remove(component.GetType());

    public bool HasAnyDrawableComponent() => 
        Repository.Values.Any(x => x is IDraw);

    public bool HasAnyIUIComponent() => 
        Repository.Values.Any(x => x is ICanvasComponent);

    public IReadOnlyList<IDraw> GetDraws()
    {
        List<IDraw> list = [];
        foreach (T item in Repository.Values)
        {
            if (item is IDraw draw)
                list.Add(draw);
        }
        return list;
    }

    public IInteractable GetInteractable() => 
        Repository.Values.FirstOrDefault(x => x is IInteractable) as IInteractable;

    public void Cleanup() =>
        Repository.Clear();
}