using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace VideoDroid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var startActivityIntent = new Intent(context, typeof(MainActivity));
            startActivityIntent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(startActivityIntent);
        }
    }
}