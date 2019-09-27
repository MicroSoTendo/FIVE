namespace FIVE.EventSystem
{
    public class OnLoadingGameMode : IEventType
    {

    }

    public class OnLoadingMultiPlayer : OnLoadingGameMode { }
    public class OnLoadingSinglePlayer : OnLoadingGameMode { }
}