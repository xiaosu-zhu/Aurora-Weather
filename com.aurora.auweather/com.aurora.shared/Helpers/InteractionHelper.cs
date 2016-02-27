using Windows.Devices.Input;

namespace Com.Aurora.Shared.Helpers
{
    public static class InteractionHelper
    {
        public static bool HaveTouchCapabilities()
        {
            var touch = new TouchCapabilities();
            return touch.TouchPresent > 0;
        }
    }
}
