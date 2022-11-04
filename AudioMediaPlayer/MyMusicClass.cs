using Android.App;
using Android.Content;
using Android.Content.Res;
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
    public class MyMusic
    {
      
        public MyMusic(int musicId,int musicImageId,string musicName, string musicSinger)
        {
            MusicId = musicId;
            MusicImageId = musicImageId;
            MusicName = musicName;
            MusicSinger = musicSinger;
        }

        public int MusicId { get; set; }
        public int MusicImageId { get; set; }
        public string MusicName { get; set; }
        public string MusicSinger { get; set; }
    }
    public class MyMusicClass
    {
        static MyMusic[] myMusics =
            {
               new MyMusic(Resource.Raw.cheapthrill,Resource.Drawable.Cheap,"Cheap Thrills","Sia ft. Paul"),
               new MyMusic(Resource.Raw.closer,Resource.Drawable.closers,"Closer","The Chainsmoker ft. Hasely"),
               new MyMusic(Resource.Raw.perfect,Resource.Drawable.perfects,"Perfect","Ed Sheeran"),
               new MyMusic(Resource.Raw.perfectinstrumental,Resource.Drawable.perfectinstrumentals,"Perfect Instrumental","Ed Sheeran"),
               new MyMusic(Resource.Raw.pyarlafzonmeinkahan,Resource.Drawable.pyaarlafzonmeinkaha,"Pyaar Lafzon meinkahan","Not Applicable"),
               new MyMusic(Resource.Raw.sameoldlove,Resource.Drawable.sameold,"Same Old Love","Salena Gomez"),
               new MyMusic(Resource.Raw.shapeofyou,Resource.Drawable.shape,"Shape Of You","Ed Sheeran"),
               new MyMusic(Resource.Raw.cheapthrill,Resource.Drawable.Cheap,"Cheap Thrills","Sia ft. Paul"),
               new MyMusic(Resource.Raw.closer,Resource.Drawable.closers,"Closer","The Chainsmoker ft. Hasely"),
               new MyMusic(Resource.Raw.perfect,Resource.Drawable.perfects,"Perfect","Ed Sheeran"),
               new MyMusic(Resource.Raw.perfectinstrumental,Resource.Drawable.perfectinstrumentals,"Perfect Instrumental","Ed Sheeran"),
               new MyMusic(Resource.Raw.pyarlafzonmeinkahan,Resource.Drawable.pyaarlafzonmeinkaha,"Pyaar Lafzon meinkahan","Not Applicable"),
               new MyMusic(Resource.Raw.sameoldlove,Resource.Drawable.sameold,"Same Old Love","Salena Gomez"),
               new MyMusic(Resource.Raw.shapeofyou,Resource.Drawable.shape,"Shape Of You","Ed Sheeran"),
               new MyMusic(Resource.Raw.cheapthrill,Resource.Drawable.Cheap,"Cheap Thrills","Sia ft. Paul"),
               new MyMusic(Resource.Raw.closer,Resource.Drawable.closers,"Closer","The Chainsmoker ft. Hasely"),
               new MyMusic(Resource.Raw.perfect,Resource.Drawable.perfects,"Perfect","Ed Sheeran"),
               new MyMusic(Resource.Raw.perfectinstrumental,Resource.Drawable.perfectinstrumentals,"Perfect Instrumental","Ed Sheeran"),
               new MyMusic(Resource.Raw.pyarlafzonmeinkahan,Resource.Drawable.pyaarlafzonmeinkaha,"Pyaar Lafzon meinkahan","Not Applicable"),
               new MyMusic(Resource.Raw.sameoldlove,Resource.Drawable.sameold,"Same Old Love","Salena Gomez"),
               new MyMusic(Resource.Raw.shapeofyou,Resource.Drawable.shape,"Shape Of You","Ed Sheeran")
            };

        MyMusic[] myMusic;

        public MyMusicClass()
        {
            myMusic = myMusics;
        }

        public int NumbMusics
        {
            get { return myMusic.Length; }
        }

        public MyMusic this[int i]
        { get { return myMusic[i]; } }
    }
}