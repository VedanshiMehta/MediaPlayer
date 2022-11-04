using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AudioMediaPlayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AudioMediaPlayer
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[] {"actionplay","actionnext","actionprevious"})]
    public class MusicNotificationClass : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
           
            string actionName = intent.Action;
            Intent serviceIntent = new Intent(context,typeof(MusicPlayingService));
            if (actionName != null)
            {
                
                switch(actionName)
                {
                    case "actionplay":
                        serviceIntent.PutExtra("ActionName", "PlayPause");
                        context.StartService(serviceIntent);
                        break;
                    case "actionnext":
                        serviceIntent.PutExtra("ActionName", "Next");
                        context.StartService(serviceIntent);
                        break;
                    case "actionprevious":
                        serviceIntent.PutExtra("ActionName", "Previous");
                        context.StartService(serviceIntent);
                        break;
                  

                }
       
            }
        }
    }
}