using UnityEngine;

namespace TimelineControl
{
    public static class TimelinePlayerUtil
    {
        public static string TimelineColor(this string str, string color)
        {
            return $"<color={color}>{str}</color>";
        }

        public static string TimelineColor(this string str, Color color)
        {
            return TimelineColor(str, ColorUtility
                .ToHtmlStringRGB(color));
        }
    }
}
