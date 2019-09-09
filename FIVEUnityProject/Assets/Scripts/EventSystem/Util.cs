using System;
using System.Threading.Tasks;

namespace FIVE.EventSystem
{
    public static class Util
    {
        public static void RaiseEvent<T>(this object sender, EventArgs args)
        {
            EventManager.RaiseEvent<T>(sender, args);
        }
        public static async Task RaiseEventAsync<T>(this object sender, EventArgs args)
        {
            await EventManager.RaiseEventAsync<T>(sender, args);
        }
    }
}