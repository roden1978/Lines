using System;
using System.Collections.Generic;
using System.Linq;

public static class LayerMask
{
    public static object GetLayerName(int layer) =>
     Enum.ToObject(typeof(CollisionLayers), layer);

    public static int GetLayerId(string layerName) =>
     (int)Enum.Parse(typeof(CollisionLayers), layerName);

    public static int GetLayerId(CollisionLayers layer) =>
     (int)Enum.Parse(typeof(CollisionLayers), Enum.GetName(typeof(CollisionLayers), layer));
}

public static class IEnumerableExtensions {
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)       
       => self.Select((item, index) => (item, index));
}
