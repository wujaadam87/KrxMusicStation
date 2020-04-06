using System;

namespace KrxMusicStation.Logic
{
    public static class Extensions
    {
        public static string ToDurationString(this TimeSpan duration)
        {
            if (duration.Duration().Hours == 0)
                return duration.ToString(@"%m\:ss");
            else
                return duration.ToString(@"%h\:mm\:ss");
        }
    }
}
