using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.Fragment.App;
using AudioMediaPlayer.Interface;
using AudioMediaPlayer.Services;
using Google.Android.Material.Card;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioMediaPlayer
{
    public class MusicBotttomFragment : Fragment,IServiceConnection,IMiniMusicControls
    {
        private ImageView _imageViewMusicLogo;
        private TextView _textViewMusicName;
        private TextView _textViewMusicSinger;
        private FloatingActionButton _floatingActionButtonPause;
        private ImageView _imageViewNext;
        private ImageView _imageViewPrevious;
        private View view;
        private bool isBottomPlayer;
        private int pathFragment;
        private MyMusicClass _myMusicClass;
        private MusicPlayingService _musicPlayingService;
        private MusicBinder _musicBinder;
        public string _musiclastPlayed = "LastPlayed";
        public string _musicFile = "StoredMusic";
        private CardView _cardView;
        int value;
        public bool isFragment;
        public MusicBotttomFragment()
        {

            isFragment = true;

        }

        public MusicBotttomFragment(bool isBottomPlayer, int pathFragment)
        {
            this.isBottomPlayer = isBottomPlayer;
            this.pathFragment = pathFragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            view = inflater.Inflate(Resource.Layout.musicfragmentlayout, container, false);
            _imageViewMusicLogo = view.FindViewById<ImageView>(Resource.Id.imageViewLogo);
            _textViewMusicName = view.FindViewById<TextView>(Resource.Id.textViewMusicName);
            _textViewMusicSinger = view.FindViewById<TextView>(Resource.Id.textViewSinger);
            _floatingActionButtonPause = view.FindViewById<FloatingActionButton>(Resource.Id.floatingActionButtonPause);
            _imageViewNext = view.FindViewById<ImageView>(Resource.Id.imageViewNext);
            _imageViewPrevious = view.FindViewById<ImageView>(Resource.Id.imageViewPrevious);

            _cardView = view.FindViewById<CardView>(Resource.Id.cardView);
            _myMusicClass = new MyMusicClass();
            _musicPlayingService = new MusicPlayingService();


            _floatingActionButtonPause.Click += _floatingActionButtonPause_Click;
            _imageViewNext.Click += _imageViewNext_Click;
            _imageViewPrevious.Click += _imageViewPrevious_Click;

            return view;
        }

        private void _imageViewPrevious_Click(object sender, EventArgs e)
        {
            MiniPrevious();
        }

        private void _floatingActionButtonPause_Click(object sender, EventArgs e)
        {
            MiniPlayPause();
        }

        private void _imageViewNext_Click(object sender, EventArgs e)
        {
            MiniNext();

        }

        public override void OnResume()
        {
            base.OnResume();
            
            GetMusic();
            


        }

        private void GetMusic()
        {
            
            if (isBottomPlayer)
            {
                if (pathFragment != -1)
                {   
                    var musicData = _myMusicClass[pathFragment];
                    int musicImage = musicData.MusicImageId;
                    string musicName = musicData.MusicName;
                    string musicSinger = musicData.MusicSinger;

                    _textViewMusicName.Text = musicName;
                    _textViewMusicSinger.Text = musicSinger;
                    if (musicImage != 0)
                    {
                        _imageViewMusicLogo.SetImageResource(musicImage);
                    }
                    else
                    {
                        _imageViewMusicLogo.SetImageResource(Resource.Drawable.playmusicimage);
                    }
                    isFragment = true;
                    Intent intent = new Intent(Activity, typeof(MusicPlayingService));
                    if (Activity != null)
                    {
                        Activity.BindService(intent, this, Bind.AutoCreate);
                    }
                   

                }

            }
         
            
        }

        public override void OnPause()
        {
            base.OnPause();
            if (isBottomPlayer)
            {
                if (Activity != null)
                {
                    Activity.UnbindService(this);
                }
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            _musicBinder = service as MusicBinder;
            _musicPlayingService = _musicBinder.Service;
            if (_musicPlayingService.isPlaying())
            {
                _floatingActionButtonPause.Enabled = true;
                _imageViewNext.Enabled = true;
                _imageViewPrevious.Enabled = true;
                _imageViewNext.SetImageResource(Resource.Drawable.ic_next);
                _imageViewPrevious.SetImageResource(Resource.Drawable.ic_previous);
                _cardView.Visibility = ViewStates.Visible;
            }
            else
            {
                _floatingActionButtonPause.Enabled = false;
                _imageViewNext.Enabled = false;
                _imageViewPrevious.Enabled = false;
                _imageViewNext.SetImageResource(Resource.Drawable.ic_next_untapped);
                _imageViewPrevious.SetImageResource(Resource.Drawable.ic_previous_untapped);
                _cardView.Visibility= ViewStates.Gone;
            }

        }

        public void OnServiceDisconnected(ComponentName name)
        {
            _musicPlayingService = null;
        }

        public void MiniPlayPause()
        {
            _musicPlayingService.PlayPause();

           
            if (!_musicPlayingService.isPlaying())
            {
                _floatingActionButtonPause.SetImageResource(Resource.Drawable.ic_play);
            }
            else
            {
                _floatingActionButtonPause.SetImageResource(Resource.Drawable.ic_pause);
            }
        }

        public void MiniNext()
        {
            _musicPlayingService.NextMusic();

            ISharedPreferencesEditor sharedPreferencesEditor = Activity.GetSharedPreferences(_musiclastPlayed, FileCreationMode.Private)
                                                            .Edit();
            sharedPreferencesEditor.PutInt(_musicFile, _musicPlayingService.musicPostion);
            sharedPreferencesEditor.Apply();
            ISharedPreferences sharedPreferences = Activity.GetSharedPreferences(_musiclastPlayed, FileCreationMode.Private);
            value = sharedPreferences.GetInt(_musicFile, -1);
            GetMusicDetails();

           
           
        }

        private void GetMusicDetails()
        {
            if (value != -1)
            {
                isBottomPlayer = true;
                pathFragment = value;

            }
            else
            {
                isBottomPlayer = false;
                pathFragment = -1;
            }
            if (isBottomPlayer)
            {
                if (pathFragment != -1)
                {
                    var musicData = _myMusicClass[pathFragment];
                    int musicImage = musicData.MusicImageId;
                    string musicName = musicData.MusicName;
                    string musicSinger = musicData.MusicSinger;

                    _textViewMusicName.Text = musicName;
                    _textViewMusicSinger.Text = musicSinger;
                    if (musicImage != 0)
                    {
                        _imageViewMusicLogo.SetImageResource(musicImage);
                    }
                    else
                    {
                        _imageViewMusicLogo.SetImageResource(Resource.Drawable.playmusicimage);
                    }
                }
            }
        }

        public void MiniPrevious()
        {
            _musicPlayingService.PreviousMusic();
            
            ISharedPreferencesEditor sharedPreferencesEditor = Activity.GetSharedPreferences(_musiclastPlayed, FileCreationMode.Private)
                                                            .Edit();
            sharedPreferencesEditor.PutInt(_musicFile, _musicPlayingService.musicPostion);
            sharedPreferencesEditor.Apply();
            ISharedPreferences sharedPreferences = Activity.GetSharedPreferences(_musiclastPlayed, FileCreationMode.Private);
            value = sharedPreferences.GetInt(_musicFile, -1);
            GetMusicDetails();

        }
    }
}