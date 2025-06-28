public class Component : IStart, IDestroy
{
    public bool Active { get; private set; } = true;
    public GameObject gameObject { get; set; }
    public bool Started { get; private set; }

    public virtual void SetActive(bool value) => Active = value;

    public virtual void SetStarted(bool value) => Started = value;
    
    public virtual void Start() => Started = true;

    public virtual void Destroy(){ }

}