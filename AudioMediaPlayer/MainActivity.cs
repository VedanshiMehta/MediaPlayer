using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using AudioMediaPlayer.Services;
using System;
using System.Collections.Generic;
using static AndroidX.ConstraintLayout.Core.Motion.Utils.HyperSpline;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace AudioMediaPlayer
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true,
        ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : AppCompatActivity,IServiceConnection
    {
       
        private RecyclerView _recyclerViewAudio;
        private RecyclerView.LayoutManager _layoutManager;
        private AudioAdapter _audioAdapter;
        private MyMusicClass _musicClass;
        private MusicBinder _musicBinder;
        public string _musiclastPlayed = "LastPlayed";
        public string _musicFile = "StoredMusic";
        public static bool isBottomPlayer;
        private Toolbar _toolbar;
        public MusicBotttomFragment _musicBotttomFragment;
        int pathFragment;
        private MusicPlayingService _musicPlayingService;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            UIReferences();
            _musicPlayingService = new MusicPlayingService();
            GetMusicList();


        }





        private void GetMusicList()
        {
           _layoutManager = new LinearLayoutManager(this);
            _recyclerViewAudio.SetLayoutManager(_layoutManager);
            _musicClass = new MyMusicClass();
            _audioAdapter = new AudioAdapter(this, _musicClass);
            _audioAdapter.OnItemSelected += _audioAdapter_OnItemSelected;
            _recyclerViewAudio.SetAdapter(_audioAdapter);

            Intent i = new Intent(this,typeof(MusicPlayingService));
            StartService(i);
     
        }
        protected override void OnResume()
        {
            base.OnResume();
            GetLastPlayedMusic();
            Intent j = new Intent(this, typeof(MusicPlayingService));
            BindService(j, this, Bind.AutoCreate);

        }
        protected override void OnPause()
        {
            base.OnPause();
            UnbindService(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _musicPlayingService.StopService();
        }
        private void GetLastPlayedMusic()
        {
            ISharedPreferences sharedPreferences = GetSharedPreferences(_musiclastPlayed, FileCreationMode.Private);
            int value = sharedPreferences.GetInt(_musicFile, -1);
            if (value != -1)
            {
                isBottomPlayer = true;
                pathFragment = value;
                _musicBotttomFragment = new MusicBotttomFragment(isBottomPlayer, pathFragment);
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayoutMusic, _musicBotttomFragment).Commit();
            
            }
            else
            {
                isBottomPlayer = false;
                pathFragment = -1;
            }

        }
        
        private void _audioAdapter_OnItemSelected(object sender, AudioAdapterEventArgs e)
        {

            var musicdetails = _musicClass[e.Position];
            int postionAdapter = e.Position;
            int musicId = musicdetails.MusicId;
            string musicName = musicdetails.MusicName;
            int musicImage = musicdetails.MusicImageId;
            StartMusic(postionAdapter,musicId,musicName,musicImage);
          


        }

        private void StartMusic(int postion, int music, string musicname, int musicimage)
        {
            Intent intent = new Intent(this, typeof(PlayMusicActivity));
            intent.PutExtra("postions", postion);
            intent.PutExtra("musicid", music);
            intent.PutExtra("musicName", musicname);
            intent.PutExtra("musicImage", musicimage);
            StartActivity(intent);
        }

        private void UIReferences()
        {
           _recyclerViewAudio = FindViewById<RecyclerView>(Resource.Id.recyclerViewAudio);
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbarMusic);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            _musicBinder = service as MusicBinder;
            _musicPlayingService = _musicBinder.Service;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            _musicPlayingService = null;
        }
    }
}