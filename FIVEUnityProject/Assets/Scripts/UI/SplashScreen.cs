using System;
using System.Collections;
using UnityEngine;

namespace FIVE.UI
{
    public abstract class SplashScreen
    {
        public abstract IEnumerator OnTransitioning();
    }
}
