namespace FIVE.EventSystem
{
    public interface IEventType
    {
        
    }

    public interface IEventType<T> : IEventType
    {
        T TypeHolder { get; }
    }
}