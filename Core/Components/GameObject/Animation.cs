public class Animation : IDestory
{
    public bool Loop { get; set; }
    public string Name { get; set; }
    public Sequence Sequence { get; private set; }
    public double TimeStepMilliseconds { get; }
    public Animation(Sequence sequence, double timeStepMilliseconds, string name, bool loop = true)
    {
        Sequence = sequence;
        TimeStepMilliseconds = timeStepMilliseconds;
        Name = name;
        Loop = loop;
    }

    public void Destroy()
    {
        Sequence.Destroy();
    }
}