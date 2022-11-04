using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.Graphics.Drawable;
using AudioMediaPlayer.Interface;
using Javax.Crypto.Spec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioMediaPlayer.Services
{
    [Service]
    public class MusicPlayingService : Service
    {
        public IBinder Binder { get; private set; }
        private MediaPlayer _mediaPlayer;
        private MyMusicClass _musicClass;
        private IMusicPlaying musicPlaying;
        private IMiniMusicControls miniMusicControls;
        public string channelId1 = "channel1";
        public string channelId2 = "channel2";
        public string actionPrevious = "actionprevious";
        public string actionNext = "actionnext";
        public string actionPlay = "actionplay";
        public string actionMiniNext = "actionMiniNext";
        public string actionMiniPlay = "actionminiPlay";
        public string actionMiniPrevious = "actionminiPrevious";
        public int musicImage;
        public string musicName;
        private Bitmap bitmap;
        private MediaSessionCompat _mediaSession;
        public string _musiclastPlayed = "LastPlayed";
        public string _musicFile = "StoredMusic";
        public int musicPostion;

        public override IBinder OnBind(Intent intent)
        {
           this.Binder = new MusicBinder(this);
           return this.Binder;
        }
        public override void OnCreate()
        {
            base.OnCreate();
       
            _mediaPlayer = new MediaPlayer();
            CreateNotificationChannel();
            _mediaSession = new MediaSessionCompat(BaseContext, "MyMusicPlayer");

        }
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var notificationchannel1 = new NotificationChannel(channelId1, channelId1, NotificationImportance.High);
            var notificationchannel2 = new NotificationChannel(channelId2, channelId2, NotificationImportance.High);
            notificationManager.CreateNotificationChannel(notificationchannel1);
            notificationManager.CreateNotificationChannel(notificationchannel2);
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {

            
            
            int musicPostion = intent.GetIntExtra("servicePostion", -1);
            string actionName = intent.GetStringExtra("ActionName");
           
           
            if (musicPostion != -1)
            {
                PlayMedia(musicPostion);
            }
            if(actionName != null)
            {
              
                switch (actionName)
                {
                    case "PlayPause":
                       
                        PlayPause();
                        break;

                    case "Next":
                         NextMusic();
                         break;

                    case "Previous":
                        
                        PreviousMusic();
                        
                        break;
              
                   
                }
              
            }
      

            return StartCommandResult.Sticky;
        }

    

        public void PlayMedia(int StartPostion)
        {
            _musicClass = new MyMusicClass();
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Release();
                if(_musicClass != null)
                {
                    Create(StartPostion);
                    _mediaPlayer.Start();
                }
            }
            else
            {
                Create(StartPostion);
                _mediaPlayer.Start();
            }
        }

        public void Start()
        {
            _mediaPlayer.Start();
        }

        public bool isPlaying()
        {
            return _mediaPlayer.IsPlaying;
        }

        public void Stop()
        {
           _mediaPlayer.Stop();
        }
        public void Pause()
        {
            _mediaPlayer.Pause();
        }
        public void Release()
        {
            _mediaPlayer.Release();
        }

        public void SetVolume(float leftVolume, float rightVolume)
        {
            _mediaPlayer.SetVolume(leftVolume, rightVolume);
        }
        public int GetDuration()
        {
            return _mediaPlayer.Duration;
        }
        public void SeekTo(int postion)
        {
            _mediaPlayer.SeekTo(postion);
        }
        public void Create(int resid)
        {
            
            _mediaPlayer = MediaPlayer.Create(BaseContext,resid);
           

        }
        
        public int CurrentPostion()
        {
            return _mediaPlayer.CurrentPosition;
        }

        public void SetCallBackAction(IMusicPlaying musicPlaying)
        {
            this.musicPlaying = musicPlaying;
        }
        public void SetCallBackMiniAction(IMiniMusicControls miniMusicControls)
        {
            this.miniMusicControls = miniMusicControls;
        }
        public void ShowNotification(int postion ,int playpausebutton)
        {
            _musicClass = new MyMusicClass();
            musicPostion = postion;
            musicImage = _musicClass[musicPostion].MusicImageId;
            musicName = _musicClass[musicPostion].MusicName;
            ISharedPreferencesEditor sharedPreferencesEditor = GetSharedPreferences(_musiclastPlayed, FileCreationMode.Private)
                                                               .Edit();
            sharedPreferencesEditor.PutInt(_musicFile, musicPostion);
            sharedPreferencesEditor.Apply();

            Intent currentMusic = new Intent(this, (typeof(MainActivity)));
            PendingIntent pendingintent = PendingIntent.GetActivity(this, 0, currentMusic, 0 | PendingIntentFlags.Immutable);

            Intent previousMusic = new Intent(this, (typeof(MusicNotificationClass)))
            .SetAction(actionPrevious);
            PendingIntent previouspendingintent = PendingIntent.GetBroadcast(this, 0, previousMusic, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            Intent pauseMusic = new Intent(this, (typeof(MusicNotificationClass)))
            .SetAction(actionPlay);
            PendingIntent pausependingintent = PendingIntent.GetBroadcast(this, 0, pauseMusic, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            Intent nextMusic = new Intent(this, (typeof(MusicNotificationClass)))
            .SetAction(actionNext);
            PendingIntent nextpendingintent = PendingIntent.GetBroadcast(this, 0, nextMusic, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            if (musicImage != -1)
            {
                bitmap = GetBitmapFromVectorDrawable(this, musicImage);

            }
            else
            {
                bitmap = GetBitmapFromVectorDrawable(this, Resource.Drawable.playmusicimage);
            }



            Notification builder = new NotificationCompat.Builder(this, channelId2)
                 .SetSmallIcon(playpausebutton)
                 .SetLargeIcon(bitmap)
                 .SetContentTitle(musicName)
                 .AddAction(Resource.Drawable.ic_previous, "Previous" , previouspendingintent)
                 .AddAction(playpausebutton, "PlayPause", pausependingintent)
                 .AddAction(Resource.Drawable.ic_next, "Next", nextpendingintent)
                 .SetStyle(new AndroidX.Media.App.NotificationCompat.MediaStyle().SetMediaSession(_mediaSession.SessionToken))
                 .SetPriority(NotificationCompat.PriorityHigh)
                 .SetDefaults((int)NotificationDefaults.All)
                 .SetVisibility(NotificationCompat.VisibilityPublic)
                 .SetOnlyAlertOnce(true)
                 .Build();


            StartForeground(1, builder);

         
        }
        public static Bitmap GetBitmapFromVectorDrawable(Context context, int drawableId)
        {
            Drawable drawable = ContextCompat.GetDrawable(context, drawableId);
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                drawable = (DrawableCompat.Wrap(drawable)).Mutate();
            }

            Bitmap bitmap = Bitmap.CreateBitmap(drawable.IntrinsicWidth,
                    drawable.IntrinsicHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);

            return bitmap;
        }

        public void NextMusic()
        {
            if (musicPlaying != null)
            {
                musicPlaying.LoadNextMusic();
            }
        }
        public void PreviousMusic()
        {
            if (musicPlaying != null)
            {
                musicPlaying.LoadPreviousMusic();
            }
        }
        public void PlayPause()
        {
            if (musicPlaying != null)
            {
                musicPlaying.PlayPauseMusic();
            }
        }

        private void MiniNext()
        {
            if (miniMusicControls != null)
            {
                miniMusicControls.MiniNext();
            }
        }

        private void MiniPlay()
        {
            if (miniMusicControls != null)
            {
                miniMusicControls.MiniPlayPause();
            }
        }

        private void MiniPrevious()
        {
            if (miniMusicControls != null)
            {
                miniMusicControls.MiniPrevious();
            }
        }
    }
}