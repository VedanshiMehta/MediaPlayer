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

namespace AudioMediaPlayer.Services
{
    public class MusicBinder : Binder
    {
        public MusicBinder(MusicPlayingService musicPlayingService)
        {
            this.Service = musicPlayingService;
        }
        public MusicPlayingService Service { get; private set; }
    }
}