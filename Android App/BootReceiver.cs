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

namespace EnvironmentINdb
{
    [BroadcastReceiver(Exported = true, Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, "android.net.conn.CONNECTIVITY_CHANGE", "EnvironmendINdb.RestartService.FromMainActivity" }, Priority = (int)IntentFilterPriority.HighPriority)]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //Toast.MakeText(Application.Context, intent.Action + "|" + Global.ServiceTimer.Enabled.ToString(), ToastLength.Short).Show();
            var a = new Intent(Application.Context, typeof(DemoService));
            //a.SetFlags(ActivityFlags.)    
            //context.StartService(a);
        }
    }
}