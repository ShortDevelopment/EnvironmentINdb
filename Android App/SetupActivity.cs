using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace EnvironmentINdb
{
    [Activity(Label = "Setup", Theme = "@style/MyTheme", Icon = "@drawable/mess", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class SetupActivity : Activity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            bool a = true;
            if (!CheckPermissions(Global.Permissions))
            {
                Android.Support.V4.App.ActivityCompat.RequestPermissions(this, Global.Permissions, 0);
            }else
            {
                button2.Visibility = ViewStates.Gone;               
                button1.Visibility = ViewStates.Visible;                
            }
        }
        LinearLayout PermissionLayout;
        LinearLayout ToolsLayout;
        Button button2;
        Button button1;
        int PageIndex = 0;
        LinearLayout[] Pages = new LinearLayout[] { };
        public override void OnBackPressed()
        {
            Toast.MakeText(this, "Bitte schlieﬂe erst das Setup ab!", ToastLength.Short).Show();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Setup1);
            this.Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);            
            ActionBar.Hide();            
            this.TitleFormatted = Android.Text.Html.FromHtml("<font color='#FF8000'>" + this.Title + "</font>");
            PermissionLayout = FindViewById<LinearLayout>(Resource.Id.PermissionLayout);
            ToolsLayout = FindViewById<LinearLayout>(Resource.Id.ToolsLayout);
            button1 = FindViewById<Button>(Resource.Id.button1);
            button1.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.HideNavigation;
            button2 = FindViewById<Button>(Resource.Id.button2);
            button1.Click += ShowToolsLayout;
            PermissionLayout.Visibility = ViewStates.Visible;
            ToolsLayout.Visibility = ViewStates.Gone;
            button2.Visibility = ViewStates.Gone;
            button1.Visibility = ViewStates.Visible;
            if (!CheckPermissions(Global.Permissions))
            {
                button2.Visibility = ViewStates.Visible;
                button1.Visibility = ViewStates.Gone;
                button2.Click += ReqPermission;
            }            
        }
        private void ReqPermission(object sender, EventArgs e)
        {
            Android.Support.V4.App.ActivityCompat.RequestPermissions(this, Global.Permissions, 0);
        }
        private void ShowToolsLayout(object sender, EventArgs e)
        {
            PermissionLayout.Visibility = ViewStates.Gone;
            ToolsLayout.Visibility = ViewStates.Visible;
            button2.Visibility = ViewStates.Visible;
            button2.Text = "Jetzt nutzen";
            button2.Click -= ReqPermission;
            button2.Click += delegate {
                Intent i = new Intent();
                if (Build.VERSION.PreviewSdkInt >= 16)
                {
                    i.SetAction(WallpaperManager.ActionChangeLiveWallpaper);
                    i.PutExtra(WallpaperManager.ExtraLiveWallpaperComponent, new ComponentName(this, Java.Lang.Class.FromType(typeof(DBWallpaper))));
                }
                else
                {
                    i.SetAction(WallpaperManager.ActionLiveWallpaperChooser);
                }
                StartActivity(i);
            };
            button1.Visibility = ViewStates.Visible;
            button1.Click -= ShowToolsLayout;
            button1.Text = "Fertig";
            button1.Click += delegate
            {
                var settings = this.GetSharedPreferences(this.PackageName + Global.ApplicationInfo.VersionName, FileCreationMode.MultiProcess);
                var a = settings.Edit();
                a.PutBoolean("setup", false);
                a.Apply();
                StartActivity(new Intent(this, typeof(MainActivity)));
                this.Finish();
            };
        }
        bool CheckPermissions(string[] permissions)
        {
            bool a = true;
            foreach (string b in permissions)
            {
                if (CheckSelfPermission(b) != Permission.Granted)
                {
                    a = false;
                }
            }            
            return a;
        }
    }
}