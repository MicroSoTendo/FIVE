using System.Collections;

namespace FIVE.UI.SplashScreens
{
    public abstract class SplashScreen
    {
        public abstract IEnumerator OnTransitioning();
    }
}
