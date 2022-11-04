using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Media.Session;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Core.Motion.Utils;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.Core.Graphics.Drawable;
using AudioMediaPlayer.Interface;
using AudioMediaPlayer.Services;
using Google.Android.Material.Card;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Threading;
using static Android.Media.AudioManager;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
namespace AudioMediaPlayer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class PlayMusicActivity : AppCompatActivity,SeekBar.IOnSeekBarChangeListener,IMusicPlaying,IServiceConnection,IOnAudioFocusChangeListener
    {
        private Toolbar _toolbar;
        private ImageView _imageView;
        private FloatingActionButton _floatingActionButton;
        private FloatingActionButton _floatingActionButtonPrevious;
        private FloatingActionButton _floatingActionButtonNext;
        private SeekBar _seekBarMusic;
        int musicId;
        string musicname;
        int musicimage;
        int positionAdapter;
        private int nextPostion;
        private int previousPosition;
        private Timer _timer;
        private MyMusicClass _musicClass;
        private TextView _textViewMusicPlayTime;
        private TextView _textViewMusicDuration;
        string musicDuration;
        private MusicPlayingService _musicPlayingService;
        private MusicBinder _binder;
        private bool isCurrentPostion;
        private bool isNextPostion;
        private int mNextPostion;
        private int mPreviousPosition;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.playmusiclayout);
            UIReferences();
            UIClickEvents();
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
           
            _musicClass = new MyMusicClass();
            _musicPlayingService = new MusicPlayingService();
           

            LoadCurrentMusic();
        }

     

        protected override void OnResume()
        {
            base.OnResume();

            Intent i = new Intent(this, typeof(MusicPlayingService));
            BindService(i, this, Bind.AutoCreate);
        }
        protected override void OnPause()
        {
            base.OnPause();
            UnbindService(this);
        }

        public void LoadCurrentMusic()
        {
            GetIntentMethod(); 
        }

        private void GetIntentMethod()
        {
          
            if (Intent.Extras != null)
            {
                isCurrentPostion = true;
                positionAdapter = Intent.GetIntExtra("postions", -1);

                nextPostion = positionAdapter;
                previousPosition = positionAdapter;

                musicId = Intent.GetIntExtra("musicid", -1);
                musicname = Intent.GetStringExtra("musicName");
                musicimage = Intent.GetIntExtra("musicImage", -1);
                if (musicname != null && musicId != -1 && musicimage != -1)
                {
                    SupportActionBar.Title = musicname;

                    _imageView.SetImageResource(musicimage);

                }

                RequestAudioFocus();

                Intent i = new Intent(this, typeof(MusicPlayingService));
                i.PutExtra("servicePostion", musicId);
                StartService(i);

            }
        }

        [Obsolete]
        private bool RequestAudioFocus()
        {

            AudioManager audioManager = (AudioManager)GetSystemService(AudioService);
            AudioFocusRequest audioFocusRequest;
            if(Build.VERSION.SdkInt > BuildVersionCodes.O)
            {
                audioFocusRequest = audioManager.RequestAudioFocus(new AudioFocusRequestClass.Builder(AudioFocus.Gain)
                    .SetAudioAttributes( new AudioAttributes.Builder().SetLegacyStreamType(Stream.Music).Build())
                    .SetOnAudioFocusChangeListener(this)
                    .Build());
            }
            else
            {
                audioFocusRequest = audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
            }

            if (audioFocusRequest == AudioFocusRequest.Granted)
            {
                return true;
            }
            return false;
        }

        public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
        {
            switch (focusChange)  
            {  
              case AudioFocus.Gain:

                    //Gain when other Music Player app releases the audio service
                    _musicPlayingService.SetVolume(1f, 1f);
                    _musicPlayingService.Start();
                    break; 
                    
              case AudioFocus.Loss:  
                  //We have lost focus stop!  
                    _musicPlayingService.Stop();
                    break; 
                    
              case AudioFocus.LossTransient:  
                  //We have lost focus for a short time, but likely to resume so pause  
                   _musicPlayingService.Pause();
                   break;  

              case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume 
                   _musicPlayingService.SetVolume(0.2f, 0.2f);
                   break;  
            } 
        }

        private void CallBack(object state = null)
        {

          
            RunOnUiThread(() =>
                {
                    _seekBarMusic.Progress = _musicPlayingService.CurrentPostion();
                    _textViewMusicPlayTime.Text = MusicTime.GetTime(_musicPlayingService.CurrentPostion());
                    if (_textViewMusicPlayTime.Text == _textViewMusicDuration.Text)
                    {
                        
                        _timer.Dispose();
                        _seekBarMusic.Progress = 0;
                        _textViewMusicPlayTime.Text = "00:00";
                        LoadNextMusic();

                    }

                

                });
           
        }


        private void UIClickEvents()
        { 
            _toolbar.NavigationClick += _toolbar_NavigationClick;
            _floatingActionButton.Click += _floatingActionButton_Click;
            _floatingActionButtonPrevious.Click += _floatingActionButtonPrevious_Click;
            _floatingActionButtonNext.Click += _floatingActionButtonNext_Click;
            
        }

        private void _floatingActionButtonNext_Click(object sender, EventArgs e)
        {
            LoadNextMusic();
           
        }

        public void LoadNextMusic()
        {
            _musicPlayingService.Stop();
            _musicPlayingService.Release();
            _timer.Dispose();
            isCurrentPostion = false;
            isNextPostion = true;
            if (nextPostion == _musicClass.NumbMusics - 1)
            {
                nextPostion = 0;
            }
            else
            {
                nextPostion++;
            }
            previousPosition = nextPostion;
            mNextPostion = nextPostion;
            int musicid = _musicClass[nextPostion].MusicId;
            int musicimageid = _musicClass[nextPostion].MusicImageId;
            string musicname = _musicClass[nextPostion].MusicName;
            _musicPlayingService.ShowNotification(nextPostion, Resource.Drawable.ic_pause);
            LoadMusic(musicname,musicid,musicimageid);
          
           


        }

        private void LoadMusic(string musicName, int musicId, int musicImage)
        {
            SupportActionBar.Title = musicName;
            _imageView.SetImageResource(musicImage);

            _musicPlayingService.Create(musicId);
            _musicPlayingService.Start();

            musicDuration = _musicPlayingService.GetDuration().ToString();
            _textViewMusicDuration.Text = MusicTime.GetTime(int.Parse(musicDuration));
            _textViewMusicPlayTime.Text = "00:00";

            _seekBarMusic.SetOnSeekBarChangeListener(this);
            _seekBarMusic.Max = _musicPlayingService.GetDuration();

            _timer = new Timer(CallBack, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void _floatingActionButtonPrevious_Click(object sender, EventArgs e)
        {
            LoadPreviousMusic();
   
        }

        public void LoadPreviousMusic()
        {
            _musicPlayingService.Stop();
            _musicPlayingService.Release();
            _timer.Dispose();
            isCurrentPostion = false;
            isNextPostion = false;
            if (previousPosition == 0)
            {
                previousPosition = _musicClass.NumbMusics - 1;
            }
            else
            {
                previousPosition--;
            }
            nextPostion = previousPosition;
            mPreviousPosition = previousPosition;
          
            int musicid = _musicClass[previousPosition].MusicId;
            int musicimageid = _musicClass[previousPosition].MusicImageId;
            string musicname = _musicClass[previousPosition].MusicName;
            
            _musicPlayingService.ShowNotification(previousPosition, Resource.Drawable.ic_pause);
            LoadMusic(musicname, musicid, musicimageid);

        }

        private void _floatingActionButton_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
          
        }

        public void PlayPauseMusic()
        {
            int musicPosition = 0;
            if (_musicPlayingService.isPlaying())
            {
                _musicPlayingService.Pause();
                _floatingActionButton.SetImageResource(Resource.Drawable.ic_play);
                if(isCurrentPostion)
                {
                    musicPosition = positionAdapter;
                }
                else if(isNextPostion && !isCurrentPostion)
                {
                    musicPosition = mNextPostion;
                }
                else if(!isCurrentPostion && !isNextPostion)
                {
                    musicPosition = mPreviousPosition;
                }
                _musicPlayingService.ShowNotification(musicPosition, Resource.Drawable.ic_play);

            }
            else
            {

                _musicPlayingService.Start();
                _floatingActionButton.SetImageResource(Resource.Drawable.ic_pause);
                if (isCurrentPostion)
                {
                    musicPosition = positionAdapter;
                }
                else if (isNextPostion && !isCurrentPostion)
                {
                    musicPosition = mNextPostion;
                }
                else if (!isCurrentPostion && !isNextPostion)
                {
                    musicPosition = mPreviousPosition;
                }
                _musicPlayingService.ShowNotification(musicPosition, Resource.Drawable.ic_pause);
            }
        }

        private void _toolbar_NavigationClick(object sender, Toolbar.NavigationClickEventArgs e)
        {
  
            Finish();
        }
        public override bool OnSupportNavigateUp()
        {

            OnBackPressed();
            return base.OnSupportNavigateUp();
        }
      

        private void UIReferences()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbarMusic);
            _imageView = FindViewById<ImageView>(Resource.Id.imageViewMusicImage);
            _floatingActionButton = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonPause);
            _floatingActionButtonPrevious = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonPrevious);
            _floatingActionButtonNext = FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonNext);
            _seekBarMusic = FindViewById<SeekBar>(Resource.Id.seekBarMusic);
            _textViewMusicDuration = FindViewById<TextView>(Resource.Id.textViewMusicDuration);
            _textViewMusicPlayTime = FindViewById<TextView>(Resource.Id.textViewMusicPlayTime);
        }

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            if (fromUser)
            {
                _musicPlayingService.SeekTo(progress);
            }
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
          
        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
           
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {

           
            _binder = service as MusicBinder;
           _musicPlayingService = _binder.Service;

            _musicPlayingService.ShowNotification(positionAdapter,Resource.Drawable.ic_pause);

            _musicPlayingService.SetCallBackAction(this);
            
            musicDuration = _musicPlayingService.GetDuration().ToString();
           _textViewMusicDuration.Text = MusicTime.GetTime(int.Parse(musicDuration));
           _textViewMusicPlayTime.Text = "00:00";

            _seekBarMusic.SetOnSeekBarChangeListener(this);
            _seekBarMusic.Max = _musicPlayingService.GetDuration();

            _timer = new Timer(CallBack, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

        }

        public void OnServiceDisconnected(ComponentName name)
        {
            _musicPlayingService = null;
        }

    }
}