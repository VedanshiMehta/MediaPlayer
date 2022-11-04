using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioMediaPlayer
{
    public class AudioAdapter : RecyclerView.Adapter
    {
        private MainActivity mainActivity;
        private MyMusicClass musicClass;

        public AudioAdapter(MainActivity mainActivity, MyMusicClass musicClass)
        {
            this.mainActivity = mainActivity;
            this.musicClass = musicClass;
        }

        public event EventHandler<AudioAdapterEventArgs> OnItemSelected;
        public override int ItemCount => musicClass.NumbMusics;

        public MyMusic GetItem(int position)
        {
            return musicClass[position];
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AudioViewHolder audioViewHolder = holder as AudioViewHolder;
            audioViewHolder.BindData(musicClass,position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
           View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.audioplayerrowitemdata,parent,false);
            return new AudioViewHolder(view, OnClick);
        }

        public void OnClick(AudioAdapterEventArgs args) => OnItemSelected?.Invoke(this, args);
    }
    public class AudioViewHolder : RecyclerView.ViewHolder
    {
        private TextView _textViewMusicName;
        private TextView _textViewMusicSinger;
        private ImageView _imageViewMusic;
        public AudioViewHolder(View itemView, Action<AudioAdapterEventArgs> clicklistener) : base(itemView)
        {
            _textViewMusicName = itemView.FindViewById<TextView>(Resource.Id.textViewMusicName);
            _textViewMusicSinger = itemView.FindViewById<TextView>(Resource.Id.textViewSinger);
            _imageViewMusic = itemView.FindViewById<ImageView>(Resource.Id.imageViewAudioImage);
            itemView.Click += (s, e) => clicklistener(new AudioAdapterEventArgs { View = itemView, Position = AdapterPosition });
        }

        public void BindData(MyMusicClass musicClass, int position)
        {
            _textViewMusicName.Text = musicClass[position].MusicName;
            _textViewMusicSinger.Text = musicClass[position].MusicSinger;
            _imageViewMusic.SetImageResource(musicClass[position].MusicImageId);
        }
    }
    public class AudioAdapterEventArgs:EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
        
}