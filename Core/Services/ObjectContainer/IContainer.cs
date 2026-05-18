using System;
using System.Collections.Generic;

public interface IContainer<T>
{
    public int Count { get; }
    public Dictionary<Type, T> Repository { get; }
    TP Register<TP>(TP component) where TP : T;
    void Unregister<TP>(TP component) where TP : T;
    TP GetComponent<TP>() where TP : T;
}