using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioMediaPlayer
{
    public static class MusicTime
    {
        public static string GetTime(int duration)
        {
            int min = duration / 1000 / 60;
            int sec = duration / 1000 % 60;
            string time = null;
            if (min < 10)
            {
                time += "0" + min.ToString() + ":";
            }
            else
            {
                time += min.ToString() + ":";
            }
            if (sec < 10)
            {
                time += "0" + sec.ToString();
            }
            else
            {
                time += sec.ToString();
            }
            return time;
        }
    }
}