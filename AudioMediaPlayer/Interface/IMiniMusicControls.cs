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

namespace AudioMediaPlayer.Interface
{
    public interface IMiniMusicControls
    {
        void MiniPlayPause();
        void MiniNext();
        void MiniPrevious();

    }
}