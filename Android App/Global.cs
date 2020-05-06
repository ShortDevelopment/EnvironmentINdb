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
using Android.Content.PM;

namespace EnvironmentINdb
{
    abstract class Global
    {
        public static MainActivity SavedMainActivity { get; set; }
        public static Timer ServiceTimer { get; set; }
        public static PackageInfo ApplicationInfo{
            get{
                return Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0);
            }
        }
        public static string[] Permissions
        {
            get
            {
                return new[] {
                    Android.Manifest.Permission.RecordAudio,
                    Android.Manifest.Permission.AccessCoarseLocation,
                    Android.Manifest.Permission.AccessFineLocation,
                    Android.Manifest.Permission.AccessNetworkState,
                    Android.Manifest.Permission.AccessWifiState,
                    Android.Manifest.Permission.Internet,
                    Android.Manifest.Permission.WriteExternalStorage
                };
            }
        }
        public static string ShareContent
        {
            get
            {
                return "Nutze jetzt die \"EnvironmentINdb\" App und zeige anderen wie Laut es bei dir ist!" + System.Environment.NewLine + "Weitere Infos gibt's hier: umwelt.kurzweb.de";
            }
        }
        public static string HomeDirectory
        {
            get
            {
                return "/sdcard/EnvironmentINdb/";
            }
        }        
        public static string MyDateTime
        {
            get
            {
                return DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
            }
        }
    }
}