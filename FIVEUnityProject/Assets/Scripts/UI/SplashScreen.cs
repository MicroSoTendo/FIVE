using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class SplashScreen
    {
        public abstract IEnumerator OnTransitioning();
    }
}
