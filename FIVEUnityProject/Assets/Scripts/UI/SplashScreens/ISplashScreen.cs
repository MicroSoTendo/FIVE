using System.Collections;

namespace FIVE.UI.SplashScreens
{
    public interface ISplashScreen
    {
        IEnumerator TransitionRoutine();
    }
}
