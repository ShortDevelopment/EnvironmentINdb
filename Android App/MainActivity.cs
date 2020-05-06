using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using Android.Locations;
using Android.Runtime;
using System;
using System.Collections.Generic;
using Android.Webkit;
using Android.Content;
using System.Net;
using System.IO;
using Android.Content.PM;
using Android.Views;
using Android.Text;
using Android.Graphics.Drawables;

namespace EnvironmentINdb
{
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionWebSearch }, 
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault }, 
        DataScheme = "http", 
        DataHost = "kurzweb.de", 
        DataPathPrefix = "/umwelt/", 
        AutoVerify = true, 
        Priority = (int)IntentFilterPriority.HighPriority)]
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionWebSearch },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataScheme = "http",
        DataHost = "umwelt.kurzweb.de",        
        AutoVerify = true,
        Priority = (int)IntentFilterPriority.HighPriority)]
    [Activity(Label = "EnvironmentINdb", Theme = "@style/MyTheme", MainLauncher = true, Icon = "@drawable/mess", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : Activity, ILocationListener
    {
        #region Variablen
        private MediaRecorder mediaRecorder;
        Switch ActivateButton;
        TextView LocationTextView;
        LocationManager locationManager;
        string locationProvider;
        Location MyLocation;
        WebView webview1;
        Toolbar toolbar1;
        TextView SpeedTextView;
        TextView StatusTextView;
        ImageView SpeedTypeImageView;
        ImageView UploadTypeImageView;
        ISharedPreferences Settings;
        ISharedPreferencesEditor SettingsEditor;
        System.Timers.Timer timer1;
        System.Timers.Timer timer2;
        TrafficType MyTrafficType = TrafficType.ZuFuß;
        int _db;
        double LevelBefore;
        #endregion
        bool NewVersion = false;
        bool splashscreen = false;
        string NewVersionName;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            splashscreen = true;
            RunOnUiThread(delegate { SetContentView(Resource.Layout.SplashScreen); });
            this.Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);
            ActionBar.Hide();
            
            RunOnNewThread(delegate {
                try { 
                    bool a = true;
                    foreach(string b in Global.Permissions)
                    {
                        if(CheckSelfPermission(b) != Permission.Granted){
                            a = false;
                        }
                    }
                    if(!a)
                    {
                        StartActivity(new Intent(this, typeof(SetupActivity)));
                        this.Finish();
                        return;
                    }
                    var settings = this.GetSharedPreferences(this.PackageName + Global.ApplicationInfo.VersionName, FileCreationMode.MultiProcess);
                    if(settings.GetBoolean("setup", true))
                    {
                        StartActivity(new Intent(this, typeof(SetupActivity)));
                        this.Finish();
                        return;
                    }
                }
                catch { }
                try
                {
                    NewVersionName = new WebClient().DownloadString("http://kurzweb.de/umwelt/info.txt");
                    var data = NewVersionName.Split(new[] { "." }, StringSplitOptions.None);
                    var bcd = Global.ApplicationInfo.VersionName.Split(new[] { "." }, StringSplitOptions.None);                    
                    if (int.Parse(data[0]) > int.Parse(bcd[0])){
                        NewVersion = true;
                    }else if(int.Parse(data[0]) == int.Parse(bcd[0]) && int.Parse(data[1]) > int.Parse(bcd[1]))
                    {
                        NewVersion = true;
                    }
                    else if (int.Parse(data[0]) == int.Parse(bcd[0]) && int.Parse(data[1]) == int.Parse(bcd[1]) && int.Parse(data[2]) > int.Parse(bcd[2]))
                    {
                        NewVersion = true;
                    }                    
                }
                catch (Exception ex){
                    //Functions.ShowError(this, ex, "Es konnten keine Daten vom Server geladen werden");
                }
                System.Threading.Thread.Sleep(1000);
                RunOnUiThread(delegate { init(); });
            });            
        }        
        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                if (ActivateButton.Checked)
                {
                    locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                }
            }
            catch { }
        }
        protected override void OnRestart()
        {
            base.OnRestart();
            //if (!splashscreen) { CheckLocationManager(); } 
            try
            {
                if (ActivateButton.Checked)
                {
                    locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                }                
            }
            catch { }            
            CheckAutoUpdate();         
        }        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            var a = new Intent();
            a.SetAction("EnvironmendINdb.RestartService.FromMainActivity");
            SendBroadcast(a);
            try
            {
                mediaRecorder.Stop();
                mediaRecorder.Release();
                mediaRecorder.Dispose();
                mediaRecorder = null;
            }
            catch { }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_navigation, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_settings:
                    StartActivity(new Intent(this, typeof(SettingsActivity)));
                    return true;
                case Resource.Id.menu_share:
                    Functions.Share(this);
                    return true;
                case Resource.Id.menu_help:
                    StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://kurzweb.de/umwelt/help.php?id=01")));
                    return true;
            }
            return base.OnOptionsItemSelected(item);            
        }
        #region Functions
        void init(){
            #region Start
            SetContentView(Resource.Layout.Home);
            try
            {
                this.Window.ClearFlags(Android.Views.WindowManagerFlags.Fullscreen);
                ActionBar.Show();
            }catch { }            
            Global.SavedMainActivity = this;
            //StartService(new Intent(this, typeof(DemoService)));
            Settings = this.GetSharedPreferences(this.PackageName + Global.ApplicationInfo.VersionName + ".Settings", FileCreationMode.MultiProcess);
            SettingsEditor = Settings.Edit();
            ActivateButton = FindViewById<Switch>(Resource.Id.switch1);
            SpeedTextView = FindViewById<TextView>(Resource.Id.textView4);
            StatusTextView = FindViewById<TextView>(Resource.Id.textView5);
            SpeedTypeImageView = FindViewById<ImageView>(Resource.Id.imageView2);
            UploadTypeImageView = FindViewById<ImageView>(Resource.Id.imageView3);
            LocationTextView = FindViewById<TextView>(Resource.Id.textView3);
            LocationTextView.TextFormatted = Android.Text.Html.FromHtml("<i>Connecting...</i>");
            #endregion
            #region "Toolbar"             
            ActionBar.SetHomeButtonEnabled(false);
            ActionBar.SetDisplayShowHomeEnabled(false);
            ActionBar.SetDisplayUseLogoEnabled(true);            
            ActionBar.SetIcon(Resources.GetDrawable(Resource.Drawable.favicon));
            ActionBar.SetLogo(Resources.GetDrawable(Resource.Drawable.favicon));
            ActionBar.TitleFormatted = Android.Text.Html.FromHtml("<font color='#FF8000'>" + this.Title + "</font>");
            ActionBar.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.ParseColor("#1f669b")));
            FindViewById<ImageButton>(Resource.Id.imageButton3).Click += delegate
            {
                StartActivity(new Intent(this, typeof(MapActivity)));
            };            
            FindViewById<ImageButton>(Resource.Id.imageButton4).Click += delegate
            {
                var i = new Intent();
                i.SetAction(Intent.ActionView);
                i.SetData(Android.Net.Uri.Parse("http://umwelt.kurzweb.de"));
                StartActivity(i);
            };
            FindViewById<ImageButton>(Resource.Id.imageButton5).Click += delegate
            {
                Upload();
            };            
            #endregion
            #region Init
            InitializeLocationManager();
            InizializeMediaRecorder();
            #endregion
            #region Main
            try
            {
                locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
            }
            catch { }

            double lastLevel;
            timer1 = new System.Timers.Timer();
            timer1.Interval = 100;
            timer1.Enabled = true;
            timer1.Elapsed += delegate { 
                var thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(delegate {
                    try
                    {
                        if(mediaRecorder == null) { InizializeMediaRecorder(); };
                        var b = FindViewById<TextView>(Resource.Id.textView1);                        
                        var a = FindViewById<ProgressBar>(Resource.Id.progressBar1);
                        var max = mediaRecorder.MaxAmplitude;
                        long db = (long)(10 * System.Math.Round(System.Math.Log(max / 10), 1));//20 * System.Math.Log10(mediaRecorder.MaxAmplitude) - 10 ;//0.00002 2700.0 32767 //-20 * System.Math.Log10(mediaRecorder.MaxAmplitude / 51805.5336 / 0.05)                        
                        //double db = 20 * System.Math.Log10(mediaRecorder.MaxAmplitude / 32768.0);                        
                        lastLevel = db;
                        System.Diagnostics.Debug.Print("[----]" + max.ToString() + "=>" + lastLevel.ToString());
                        if (double.IsInfinity(lastLevel))
                        {
                            lastLevel = LevelBefore;
                        }
                        else { LevelBefore = lastLevel; }
                        if (ActivateButton.Checked) {
                            RunOnUiThread(delegate
                                {                                    
                                    if (LocationTextView.Text == "Do not track" || LocationTextView.Text == "Connecting...")
                                    {
                                        LocationTextView.TextFormatted = Android.Text.Html.FromHtml("<i>Connecting...</i>");
                                        try
                                        {
                                            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                                        }
                                        catch { }                                        
                                    }            
                                    b.Text = lastLevel.ToString() + "db";
                                    _db = (int)lastLevel;
                                    a.Max = 150;
                                    a.Enabled = false;
                                    a.Progress = (int)lastLevel;
                                    if ((int)lastLevel <= 75)
                                    {
                                        a.ProgressDrawable.SetColorFilter(Android.Graphics.Color.LightGreen, Android.Graphics.PorterDuff.Mode.Multiply);
                                    }
                                    else if((int)lastLevel <= 90)
                                    {
                                        a.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                                    }
                                    else if ((int)lastLevel <= 100)
                                    {
                                        a.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Orange, Android.Graphics.PorterDuff.Mode.Multiply);
                                    }
                                    else
                                    {
                                        a.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                                    }                                                                    
                                });
                        }
                    }
                    catch (Java.Lang.Exception e)
                    {
                        RunOnUiThread(delegate { Toast.MakeText(this, "Fehler: " + e.Message, ToastLength.Short).Show(); });
                    }
                }));
                thread.IsBackground = true;
                thread.Start();
            };
            timer1.Start();
            timer2 = new System.Timers.Timer();
            timer2.Interval = Settings.GetInt("Hochlade_Interval", 10) * 1000;            
            timer2.Elapsed += delegate {
                var thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(delegate {
                    if (Settings.GetBoolean("Automatisch_Hochladen", false) == false) { timer2.Stop(); timer2.Enabled = false; return; }
                    Upload(false);
                }));
                thread.IsBackground = true;
                thread.Start();
            };
            CheckAutoUpdate();
            ActivateButton.CheckedChange += delegate
            {
                if (ActivateButton.Checked){
                    CheckLocationManager();
                    timer1.Start();
                    timer1.Enabled = true;
                    FindViewById<LinearLayout>(Resource.Id.linearLayout1).Visibility = ViewStates.Visible;
                    var ll = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                    ll.AddRule(LayoutRules.Below, Resource.Id.linearLayout1);                    
                    FindViewById<LinearLayout>(Resource.Id.linearLayout2).LayoutParameters = ll;
                    CheckAutoUpdate();
                }
                else
                {
                    timer1.Stop();
                    timer1.Enabled = false;
                    FindViewById<LinearLayout>(Resource.Id.linearLayout1).Visibility = ViewStates.Gone;                    
                    var ll = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                    ll.AddRule(LayoutRules.Below, Resource.Id.LL1);
                    //ll.SetMargins(0, -100, 0, 0);
                    FindViewById<LinearLayout>(Resource.Id.linearLayout2).LayoutParameters = ll;                    
                    LocationTextView.TextFormatted = Android.Text.Html.FromHtml("<i>Do not track</i>");
                    var a = FindViewById<ProgressBar>(Resource.Id.progressBar1);
                    var b = FindViewById<TextView>(Resource.Id.textView1);
                    StatusTextView.TextFormatted = Android.Text.Html.FromHtml("<i>Messen deaktiviert</i>");
                    UploadTypeImageView.SetImageResource(Resource.Drawable.pause);
                    a.Progress = 0;
                    b.Text = "--";
                    a.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Gray, Android.Graphics.PorterDuff.Mode.Multiply);
                    try
                    {
                        locationManager.RemoveUpdates(this);
                    }catch { }
                }
            };
            splashscreen = false;
            CheckLocationManager();
            #endregion
            #region Update
            if (NewVersion)
            {
                var dialog = new AlertDialog.Builder(this);
                dialog.SetTitle("Neues Update");
                dialog.SetMessage("Wollen Sie jetzt die auf die neue Version updaten?!");
                dialog.SetPositiveButton("Update", delegate {
                    var a = new WebClient();
                    a.DownloadFileCompleted += A_DownloadFileCompleted;
                    Functions.CheckDirectory();
                    try
                    {
                        a.DownloadFileAsync(new Uri("http://kurzweb.de/umwelt/app/" + NewVersionName + ".apk"), Global.HomeDirectory + "Update_" + NewVersionName + ".apk");
                    }
                    catch (Exception ex){
                        Functions.ShowError(this, ex, "Das Update konnte nicht heruntergeladen werden");
                    }                    
                });
                dialog.SetNegativeButton("Später", delegate { });
                dialog.SetIcon(Resources.GetDrawable(Resource.Drawable.update));
                dialog.Show();
            }
            #endregion
        }
        void Upload(bool ShowMessage = true)
        {
            if (!ActivateButton.Checked)
            {
                Toast.MakeText(this, "Das Messen wurde deaktiviert", ToastLength.Long).Show();
                Functions.SaveError(new Exception("User disabled capturing data"), "Upload_Data");
                return;
            }
            RunOnNewThread(delegate {
                #region Request
                try
                {                
                    var req = WebRequest.Create("https://umwelt.wasweisich.com/apply/");
                    req.Credentials = new NetworkCredential("jonjon0815", "Toaster1144");
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    string a = "0", b = "0";
                    if (MyLocation != null)
                    {
                        a = MyLocation.Latitude.ToString().Replace(',', '.');
                        b = MyLocation.Longitude.ToString().Replace(',', '.');
                    }
                    else
                    {
                        RunOnUiThread(delegate {
                            if (ShowMessage)
                            {
                                //Toast.MakeText(this, "Keine Position verfügbar!", ToastLength.Long).Show();
                                var dialog = new AlertDialog.Builder(this);
                                dialog.SetTitle("Keine Position verfügbar!");
                                dialog.SetMessage("Ohne GPS können Sie keine Daten hochladen." + System.Environment.NewLine + "Bitte warten Sie, bis Ihr Standort verfügbar ist.");
                                dialog.SetPositiveButton("OK", delegate { });
                                dialog.SetIcon(Resources.GetDrawable(Resource.Drawable.Warning));
                                dialog.Show();
                            } else
                            {
                                Toast.MakeText(this, "Keine Position", ToastLength.Short).Show();
                            }
                        });
                        Functions.SaveError(new Exception("No Position while Upload"), "Upload_Data");
                        return;
                    }
                    var Kommentar = "";
                    if(MyTrafficType == TrafficType.Auto)
                    {
                        Kommentar += "[In Auto ect gemessen]";
                    }
                    var data = System.Text.Encoding.UTF8.GetBytes("Latitude=" + a + "&Longitude=" + b + "&DB=" + _db.ToString() + "&Kommentar=" + Kommentar);
                    req.ContentLength = data.Length;
                    var stream = req.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();
                    stream.Dispose();
                    var res = req.GetResponse();
                    var str2 = res.GetResponseStream();
                    var respst = new StreamReader(str2).ReadToEnd();
                    RunOnUiThread(delegate { Toast.MakeText(this, respst, ToastLength.Short).Show(); });
                    str2.Close();
                    str2.Dispose();
                    res.Dispose();
                    req.Abort();
                }
                catch (Exception ex) {
                    RunOnUiThread(delegate {
                        Toast.MakeText(this, "Fehler beim Hochladen!", ToastLength.Short).Show();
                    });
                    Functions.SaveError(ex, "Upload_Data"); }
                #endregion
            });
        }
        void CheckAutoUpdate()
        {
            try
            {
                if (ActivateButton.Checked) { 
                    if (Settings.GetBoolean("Automatisch_Hochladen", false) == false)
                    {
                        timer2.Stop(); timer2.Enabled = false;
                        UploadTypeImageView.SetImageResource(Resource.Drawable.timer_off);
                        StatusTextView.Text = "Manuel hochladen";
                    }
                    else
                    {
                        timer2.Start(); timer2.Enabled = true;
                        UploadTypeImageView.SetImageResource(Resource.Drawable.timer);
                        StatusTextView.Text = "Automatisch hochladen";
                    }
                }
            }
            catch { }
        }
        private void A_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {

            }catch(Exception ex) { }            
            Java.IO.File toInstall = new Java.IO.File(Global.HomeDirectory + "Update_" + NewVersionName + ".apk");
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.N)
            {
                Android.Net.Uri apkUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.ApplicationContext, "EnvironmentINdb.EnvironmentINdb.lk_drfdf", toInstall);
                Intent intentS = new Intent(Intent.ActionInstallPackage);
                intentS.SetData(apkUri);
                intentS.SetFlags(ActivityFlags.GrantReadUriPermission);
                intentS.AddFlags(ActivityFlags.NewTask);
                StartActivity(intentS);
                this.Finish();
            }
            else
            {
                Android.Net.Uri apkUri = Android.Net.Uri.FromFile(toInstall);
                Intent intentS = new Intent(Intent.ActionView);
                intentS.SetDataAndType(apkUri, "application/vnd.android.package-archive");
                intentS.SetFlags(ActivityFlags.NewTask);
                StartActivity(intentS);
                this.Finish();
            }
        }
        void InizializeMediaRecorder()
        {
            mediaRecorder = new MediaRecorder();
            mediaRecorder.SetAudioSource(AudioSource.Default);
            mediaRecorder.SetOutputFormat(OutputFormat.ThreeGpp);
            mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
            mediaRecorder.SetOutputFile("/dev/null");
            mediaRecorder.Prepare();
            mediaRecorder.Start();
        }
        void CheckLocationManager()
        {
            LocationManager lm = (LocationManager)GetSystemService(LocationService);
            if (lm.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                var dialog = new AlertDialog.Builder(this);
                dialog.SetTitle("Bitte aktivieren Sie GPS");
                dialog.SetMessage("Ohne GPS können Sie keine Daten hochladen");
                dialog.SetCancelable(false);
                dialog.SetPositiveButton("Aktivieren", delegate {
                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                    try
                    {
                        locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                    }
                    catch { }                    
                });
                dialog.SetNegativeButton("Überspringen", delegate {
                    LocationTextView.TextFormatted = Html.FromHtml("<b>GPS Deaktiviert</b>");
                });
                dialog.SetIcon(Resources.GetDrawable(Resource.Drawable.Warning));
                dialog.Show();
            }
        }
        public static void RunOnNewThread(System.Threading.ParameterizedThreadStart del)
        {
            var thread = new System.Threading.Thread(del);
            thread.IsBackground = true;
            thread.Start();
        }
        #endregion
        #region Location
        private void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine                
            };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);
            if (acceptableLocationProviders.Count > 0)
            {
                locationProvider = acceptableLocationProviders[0];
            }
            else
            {
                locationProvider = string.Empty;
            }
            //Toast.MakeText(this, "\"" + locationProvider + "\"", ToastLength.Short).Show(); 
        }        
        bool one = false;
        enum TrafficType{ Auto, Fahrrad, ZuFuß }
        public void OnLocationChanged(Location location)
        {
            MyLocation = location;
            if (ActivateButton.Checked)
            {                 
                LocationTextView.Text = "Latitude: " + System.Math.Round(location.Latitude, 7).ToString() + " | Longitude: " + System.Math.Round(location.Longitude, 7).ToString();
                try
                {
                    locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
                    double speed = location.Speed * 3.6;
                    if (speed >= 30)
                    {
                        MyTrafficType = TrafficType.Auto;
                        SpeedTypeImageView.SetImageResource(Resource.Drawable.car);
                    }
                    else if (speed >= 6)
                    {
                        MyTrafficType = TrafficType.Fahrrad;
                        SpeedTypeImageView.SetImageResource(Resource.Drawable.bike);
                    }
                    else
                    {
                        MyTrafficType = TrafficType.ZuFuß;
                        SpeedTypeImageView.SetImageResource(Resource.Drawable.walk);
                    }
                    SpeedTextView.Text = Math.Round(speed, 1).ToString() + " km/h";
                }
                catch { }
            }
        }
        public void OnProviderDisabled(string provider)
        {
            
        }
        public void OnProviderEnabled(string provider)
        {
            
        }
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            if (status != Availability.Available && ActivateButton.Checked)
            {
                LocationTextView.TextFormatted = Android.Text.Html.FromHtml("<i><b>No Connection</b> "+ status.ToString()+ "</i>");
            }
        }
        #endregion
    }
    
}

