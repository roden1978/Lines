public interface IFactory<out T>
{
    string Name {get;}
    T Create();
}
