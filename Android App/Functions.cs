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
using System.Timers;
using System.Net;
using Android.Media;
using Android.Text;
using Android.Graphics.Drawables;
using System.IO;
using Android.Support.V4.Content;
//using Java.Lang;

namespace EnvironmentINdb
{
    public class Functions
    {
        public static void Notify(Context context, string message,string title = "")
        {
            Notification.Builder builder2 = new Notification.Builder(context)
                        .SetContentTitle(title)
                        .SetContentText(message) 
                        .SetLights(0xff9900, 1000, 500)
                        .SetVibrate(new long[] { 0, 150, 50, 500 })
                        .SetSmallIcon(Resource.Drawable.mess);
            Notification notification2 = builder2.Build();
            NotificationManager notificationManager2 = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager2.Notify(0, notification2);
        }
        public static void OpenLiveWallPaperChooser(Context context)
        {
            Intent i = new Intent();
            if (Build.VERSION.PreviewSdkInt >= 16)
            {
                i.SetAction(WallpaperManager.ActionChangeLiveWallpaper);
                i.PutExtra(WallpaperManager.ExtraLiveWallpaperComponent, new ComponentName(Application.Context, Java.Lang.Class.FromType(typeof(DBWallpaper))));
            }
            else
            {
                i.SetAction(WallpaperManager.ActionLiveWallpaperChooser);
            }
            context.StartActivity(i);
        }
        public static void FunctionNotImplemented(Context context)
        {
            var dialog = new AlertDialog.Builder(context);
            dialog.SetTitle("Nicht verfügbar!");
            dialog.SetMessage("Diese Funktion ist erst in kommenden Versionen verfügbar!");
            dialog.SetPositiveButton("OK", delegate { });            
            dialog.SetIcon(context.Resources.GetDrawable(Resource.Drawable.no));
            dialog.Show();
        }
        public static void Install(Context context, string path)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(Android.Net.Uri.Parse(path), "application/vnd.android.package-archive");
                intent.SetFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            catch(System.Exception ex) {

            }            
        }
        public static Android.Text.ISpanned FormatTitle(string title)
        {
            return Android.Text.Html.FromHtml("<font color='#FF8000'>" + title + "</font>");
        }        
        public static void CheckDirectory()
        {
            var a = new Java.IO.File(Global.HomeDirectory);
            if (a.Exists() == false)
            {
                a.Mkdir();
            }
        }
        public static void Share(Context context)
        {
            var sharingIntent = new Intent();
            sharingIntent.SetAction(Intent.ActionSend);
            sharingIntent.SetType("text/*");
            sharingIntent.PutExtra(Android.Content.Intent.ExtraSubject, "Nutze jetzt EnvironmentINdb!");
            sharingIntent.PutExtra(Intent.ExtraText, Global.ShareContent);
            context.StartActivity(Intent.CreateChooser(sharingIntent, "Link zur Anwendung teilen"));
        }
        public static void ShowError(Activity activity, System.Exception ex, string ErrorMessage)
        {
            SaveError(ex, "In [" + activity.LocalClassName + "](Error was displayed): " + ErrorMessage);
            activity.RunOnUiThread(delegate {
                var dialog = new AlertDialog.Builder(activity);
                dialog.SetTitle("Fehler");
                dialog.SetMessage(ErrorMessage + System.Environment.NewLine + "[" + ex.Message + "]");
                dialog.SetIcon(activity.Resources.GetDrawable(Resource.Drawable.Error));
                dialog.SetPositiveButton("OK", delegate { });                
                dialog.Show();
            });
        }
        public static void SaveError(System.Exception ex, string Kommentar = "unknowen")
        {
            //Android.Util.Log.Error(Kommentar, ex.Message + " {" + ex.ToString() + "}");            
            var settings = Application.Context.GetSharedPreferences(Application.Context.PackageName + ".DeveloperTool", FileCreationMode.MultiProcess);
            var editor = settings.Edit();
            var oldvalue = settings.GetString("ErrorLog", "");
            var newvalue = "[" + Global.MyDateTime + "]: Exception @ " + Kommentar + ": {" + ex.Message + "}";
            if (oldvalue != "") { newvalue += System.Environment.NewLine; }
            newvalue += oldvalue;
            editor.PutString("ErrorLog", newvalue);
            editor.Apply();
        }
        public static void RunOnNewThread(System.Threading.ParameterizedThreadStart del)
        {
            var thread = new System.Threading.Thread(del);
            thread.IsBackground = true;
            thread.Start();
        }
    }
    [Service(Enabled = true, Label = "DemoService")] //IsolatedProcess = false
    public class DemoService : IntentService
    {
        //[return: GeneratedEnum]
        //public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        //{
        //    return StartCommandResult.StickyCompatibility; // base.OnStartCommand(intent, flags, startId);
        //}
        protected override void OnHandleIntent(Intent intent)
        {
            //var cont = new WebClient().DownloadString("http://kurzweb.de/Tools/uploadfileex.txt");
            Functions.Notify(Application.Context, "dsfd", "Service");
            //System.Diagnostics.Debug.Print(" _____________________ ");
            //System.Diagnostics.Debug.Print("|                     |");
            //System.Diagnostics.Debug.Print("| DemoService started!|");
            //System.Diagnostics.Debug.Print("|_____________________|");
            #region "Media recorder"
            var mediaRecorder = new MediaRecorder();
            mediaRecorder.SetAudioSource(
                    AudioSource.Mic);
            mediaRecorder.SetOutputFormat(
                    OutputFormat.ThreeGpp);
            mediaRecorder.SetAudioEncoder(
                    AudioEncoder.AmrNb);
            mediaRecorder.SetOutputFile("/dev/null");
            mediaRecorder.Prepare();
            mediaRecorder.Start();
            
            #endregion
            if(Global.ServiceTimer != null)
            {
                Global.ServiceTimer.Enabled = false;
                Global.ServiceTimer.Stop();
                Global.ServiceTimer.Dispose();
            }
            Global.ServiceTimer = new Timer();
            Global.ServiceTimer.Interval = 1000;
            Global.ServiceTimer.Enabled = true;
            Global.ServiceTimer.Elapsed += delegate
            {
                double db = 10 * System.Math.Log(mediaRecorder.MaxAmplitude / 10);//20 * System.Math.Log10(mediaRecorder.MaxAmplitude) - 10 ;//0.00002 2700.0 32767 //-20 * System.Math.Log10(mediaRecorder.MaxAmplitude / 51805.5336 / 0.05)
                var lastLevel = System.Math.Round(db, 0);
                //System.Diagnostics.Debug.Print(" _____________________ ");
                //System.Diagnostics.Debug.Print("|                     |");
                //System.Diagnostics.Debug.Print("|       " + lastLevel.ToString() + "       |");
                //System.Diagnostics.Debug.Print("|_____________________|");
                Functions.Notify(Application.Context, "dsfdf", "Timer");
            };
            Global.ServiceTimer.Start();            
        }
        public override void OnDestroy()
        {
            var a = new Intent();
            a.SetAction("EnvironmendINdb.RestartService.FromMainActivity");
            SendBroadcast(a);
            base.OnDestroy();
        }
    }
    public class SettingItem
    {
        public bool HasCheckBox { get; set; }
        public bool HasImage { get; private set; }
        public bool Checked { get; set; }
        public bool IsClickable { get; set; }
        public bool Enabled { get; set; } = true;
        public string Text { get; set; }
        public string Description { get; set; } = "";
        public Android.Graphics.Drawables.Drawable Image { get; private set; }
        public void SetBitmap(Android.Graphics.Drawables.Drawable _Image)
        {
            Image = _Image;
            HasImage = true;
        }
        public void SetBitmap(int ResourceID)
        {
            Image = Application.Context.Resources.GetDrawable(ResourceID);
            HasImage = true;
        }
        public void ClearImage()
        {
            HasImage = false;
            Image = null;
        }
        public Action ClickEvent;
        public View View { get; set; }
    }
    public class CusotmListAdapter : BaseAdapter<SettingItem>
    {
        Activity context;
        List<SettingItem> list;
        public List<SettingItem> ItemList {
            get
            {
                return list;
            }
        }
        public CusotmListAdapter(Activity _context, List<SettingItem> _list)
            : base()
        {
            this.context = _context;
            this.list = _list;
        }
        public override int Count
        {
            get { return list.Count; }
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override SettingItem this[int index]
        {
            get { return list[index]; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            // re-use an existing view, if one is available
            // otherwise create a new one
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.SettingsListViewItem, parent, false);

            SettingItem item = this[position];            
            if (item.HasCheckBox)
            {
                view.FindViewById<Switch>(Resource.Id.switch1).Text = item.Text;
                view.FindViewById<Switch>(Resource.Id.switch1).Visibility = ViewStates.Visible;
                view.FindViewById<Switch>(Resource.Id.switch1).Checked = item.Checked;
                view.FindViewById<Switch>(Resource.Id.switch1).CheckedChange += delegate {
                    try
                    {
                        item.Checked = view.FindViewById<Switch>(Resource.Id.switch1).Checked;
                        item.ClickEvent.Invoke();                        
                    }
                    catch { }
                };
                view.FindViewById<TextView>(Resource.Id.textView1).Visibility = ViewStates.Gone;
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.textView1).Text = item.Text;
                view.FindViewById<TextView>(Resource.Id.textView1).Visibility = ViewStates.Visible;
                view.FindViewById<Switch>(Resource.Id.switch1).Visibility = ViewStates.Gone;
            }
            var imageView1 = view.FindViewById<ImageView>(Resource.Id.imageView1);
            if (item.HasImage)
            {
                imageView1.Visibility = ViewStates.Visible;
                var bmp = ((BitmapDrawable)item.Image).Bitmap;
                imageView1.SetImageBitmap(Android.Graphics.Bitmap.CreateScaledBitmap(bmp, 80, 80, false));
            }
            else
            {
                imageView1.Visibility = ViewStates.Gone;
            }
            var textView2 = view.FindViewById<TextView>(Resource.Id.textView2);
            if (item.Description == "")
            {
                textView2.Visibility = ViewStates.Gone;                
            }
            else
            {                
                textView2.TextFormatted = Html.FromHtml(item.Description);
                if (item.HasCheckBox)
                {
                    textView2.Clickable = true;
                    textView2.Click += delegate {
                        try
                        {
                            var a = view.FindViewById<Switch>(Resource.Id.switch1);
                            if (a.Checked)
                            {
                                a.Checked = false;
                            }else
                            {
                                a.Checked = true;
                            }
                            item.ClickEvent.Invoke();
                        }
                        catch { }
                    };
                }
            }
            item.View = view;
            return view;
        }
    }
    public class CustomProgressDialog : ProgressDialog
    {
        private Activity BaseActivity;
        public CustomProgressDialog(Activity activity) :base(activity)
        {
            BaseActivity = activity;
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            BaseActivity.OnBackPressed();
            this.Cancel();
        }
        public override void Show()
        {
            base.Show();
        }
    }
    public class AsynkProgressDialog : AsyncTask
    {
        CustomProgressDialog dialog;
        public AsynkProgressDialog(CustomProgressDialog _dialog) :base()
        {
            dialog = _dialog;
        }
        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] @params)
        {
            dialog.Show();
            return null;
        }
    }
}
#region OLD
//toolbar1 = FindViewById<Toolbar>(Resource.Id.toolbar1);
//            var lp1 = new LinearLayout.LayoutParams(200, 100);
//lp1.Gravity = GravityFlags.CenterHorizontal;
//            var test = new ImageButton(this);
//test.SetImageResource(Resource.Drawable.Location);            
//            test.SetBackgroundResource(Android.Resource.Color.HoloOrangeLight);
//            test.SetScaleType(ImageView.ScaleType.FitCenter);            
//            test.LayoutParameters = lp1;            
//            test.Click += delegate
//            {
//                StartActivity(new Intent(this, typeof(MapActivity)));
//            };
//            toolbar1.AddView(test);
//            var test2 = new ImageButton(this);
//test2.SetImageResource(Resource.Drawable.Info);
//            test2.SetBackgroundResource(Android.Resource.Color.HoloOrangeLight);
//            test2.SetScaleType(ImageView.ScaleType.FitCenter);
//            test2.LayoutParameters = lp1;
//            test2.Click += delegate
//            {
//                var i = new Intent();
//i.SetAction(Intent.ActionView);
//                i.SetData(Android.Net.Uri.Parse("http://umwelt.kurzweb.de"));
//                StartActivity(i);
//            };
//            toolbar1.AddView(test2);            
//            var test3 = new ImageButton(this);
//test3.SetImageResource(Resource.Drawable.Upload); // Android.Resource.Drawable.IcMenuUpload);
//            test3.SetBackgroundResource(Android.Resource.Color.HoloOrangeLight);
//            test3.SetScaleType(ImageView.ScaleType.FitCenter);
//            test3.LayoutParameters = lp1;
//            test3.Click += delegate
//            {
//                RunOnNewThread(delegate { 
//                    #region Request
//                    var req = WebRequest.Create("https://umwelt.wasweisich.com/apply/");
//req.Credentials = new NetworkCredential("jonjon0815", "Toaster1144");
//req.Method = "POST";
//                    req.ContentType = "application/x-www-form-urlencoded";
//                    string a = "0", b = "0";
//                    if (MyLocation != null)
//                    {                
//                        a = MyLocation.Latitude.ToString().Replace(',', '.');
//b = MyLocation.Longitude.ToString().Replace(',', '.');
//                    }else
//                    {
//                        RunOnUiThread(delegate {
//                            //Toast.MakeText(this, "Keine Position verfügbar!", ToastLength.Long).Show();
//                            var dialog = new AlertDialog.Builder(this);
//dialog.SetTitle("Keine Position verfügbar!");
//                            dialog.SetMessage("Ohne GPS können Sie keine Daten hochladen."+System.Environment.NewLine + "Bitte warten Sie, bis Ihr Standort verfügbar ist.");
//                            dialog.SetPositiveButton("OK", delegate { });
//                            dialog.SetIcon(Resources.GetDrawable(Resource.Drawable.Warning));
//                            dialog.Show();
//                        });
//                        return;
//                    }
//                    var data = System.Text.Encoding.UTF8.GetBytes("Latitude=" + a + "&Longitude=" + b + "&DB=" + _db.ToString());
//req.ContentLength = data.Length;
//                    var stream = req.GetRequestStream();
//stream.Write(data, 0, data.Length);
//                    stream.Flush();
//                    stream.Close();
//                    stream.Dispose();
//                    var res = req.GetResponse();
//var str2 = res.GetResponseStream();
//var respst = new StreamReader(str2).ReadToEnd();
//                    RunOnUiThread(delegate { Toast.MakeText(this, respst, ToastLength.Long).Show(); });
//                    str2.Close();
//                    str2.Dispose();
//                    res.Dispose();
//                    req.Abort();
//                    #endregion
//                });
//            };
//            toolbar1.AddView(test3);
//            var test4 = new ImageButton(this);
//test4.SetImageResource(Resource.Drawable.Settings);
//            test4.SetBackgroundResource(Android.Resource.Color.HoloOrangeLight);
//            test4.SetScaleType(ImageView.ScaleType.FitCenter);
//            test4.LayoutParameters = lp1;
//            test4.Click += delegate
//            {
//                StartActivity(new Intent(this, typeof(SettingsActivity)));
//            };
//            //toolbar1.AddView(test4);
#endregion