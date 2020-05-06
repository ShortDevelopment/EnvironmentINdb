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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using System.Net;
using System.IO;
using Android.Graphics.Drawables;

namespace EnvironmentINdb
{
    [Activity(Label = "Karte", Icon = "@drawable/Location", Theme = "@style/MyTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MapActivity : Activity, IOnMapReadyCallback, GoogleMap.IInfoWindowAdapter
    {
        private bool MapReady;
        public static GoogleMap _map;
        LatLng HGG = new LatLng(48.9016232, 9.0812541);        
        LatLngBounds BadenWürttemberg = new LatLngBounds(new LatLng(47.5594118, 7.5880493), new LatLng(49.7781628, 9.872989));
        public CustomProgressDialog _ProgressDialog;
        public void OnMapReady(GoogleMap map)
        {
            _map = map;            
            map.UiSettings.ZoomControlsEnabled = true;
            map.UiSettings.CompassEnabled = true;
            _map.MyLocationEnabled = true;
            _map.MapType = GoogleMap.MapTypeNormal;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.UiSettings.MapToolbarEnabled = true;
            map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(BadenWürttemberg, 1000, 1000, 1));
            map.SetInfoWindowAdapter(this);            
            //AddMarker(22, 0, 0);
            InitData();
        }
        void InitData()
        {
            _ProgressDialog = new CustomProgressDialog(this);
            _ProgressDialog.SetMessage("Daten werden geladen");
            _ProgressDialog.SetCancelable(false);
            _ProgressDialog.Show();
            MainActivity.RunOnNewThread(delegate {                
                var client = new WebClient();
                string data = "";
                try
                {
                    client.Credentials = new NetworkCredential("jonjon0815", "Toaster1144");
                    data = client.DownloadString("https://umwelt.wasweisich.com/get/");
                    //Toast.MakeText(this, data, ToastLength.Short).Show();
                }
                catch(Exception ex)
                {
                    RunOnUiThread(delegate {
                        var dialog = new AlertDialog.Builder(this);
                        dialog.SetTitle("Fehler");
                        dialog.SetMessage("Die Daten konnten nicht vom Server geladen werden" + System.Environment.NewLine + "[" + ex.Message + "]");
                        dialog.SetIcon(Resources.GetDrawable(Resource.Drawable.Error));
                        dialog.SetPositiveButton("Wiederholen", delegate { InitData(); });
                        dialog.SetNegativeButton("Abbrechen", delegate { this.OnBackPressed(); });
                        dialog.Show();
                    });
                    return;
                }
                var a = data.Split(new[] { "<br/>" }, StringSplitOptions.None);
                RunOnUiThread(delegate {
                foreach (var b in a)
                {
                    if (b.Contains("<")) { continue; }
                    if (b == "" || b.Contains(System.Environment.NewLine)) { continue; }
                    var c = b.Replace('.', ',').Split('|');                    
                    try
                    {
                        //System.Diagnostics.Debug.Print("---[\"" + (double.Parse(c[0])).ToString() + "\"]---");
                        //Toast.MakeText(this, c[0] + "|" + c[1] + "|" + c[2] + "|", ToastLength.Short).Show();
                        var KommStr = "";
                        if (c[4] != "")
                        {
                            KommStr = System.Environment.NewLine + c[4];
                        }
                        AddMarker(int.Parse(c[2]), double.Parse(c[1]), double.Parse(c[0]), c[3] + KommStr);
                    }
                    catch { }                    
                }
                    _ProgressDialog.Cancel();
                });               
            });
        }
        private void AddMarker(int db, double lat, double lon, string info = "Keine Infos")
        {
            MarkerOptions opt = new MarkerOptions();
            opt.SetPosition(new LatLng(lat, lon));
            opt.SetTitle(db.ToString()+"db");
            opt.SetSnippet(info);
            opt.InvokeIcon(GetIcon(GetColor(db)));
            _map.AddMarker(opt);
        }
        private new Android.Graphics.Color GetColor(int val)
        {
            double r = 0, g = 255;
            double a = Math.Pow(1.071, val + 7); // val * 3.5;
            if (a > 255)
            {
                r = 255;
                g = 255 + (255 - a);
            }
            else
            {
                r = a;
            }
            if (g < 0) { g = 0; }
            return Color.Argb(255, (int)r, (int)g, 0);
        }
        private BitmapDescriptor GetIcon(Android.Graphics.Color color)
        {
            Bitmap bmp = Bitmap.CreateBitmap(50,50, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(bmp);
            var p = new Paint();
            p.Color = color;
            p.SetStyle(Paint.Style.Fill);
            c.DrawOval(new RectF(11, 11, 40, 40), p);
            var p2 = new Paint();
            p2.Color = color;            
            p2.SetStyle(Paint.Style.Stroke);
            p2.StrokeWidth = 3;
            c.DrawOval(new RectF(5, 5, 45, 45), p2);
            return BitmapDescriptorFactory.FromBitmap(bmp);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Map);
            var m = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            if (MapReady == false) m.GetMapAsync(this);            
            MapReady = true;
            #region ActionBar
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowTitleEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetHomeAsUpIndicator(Resources.GetDrawable(Resource.Drawable.Back3));
            ActionBar.TitleFormatted = Android.Text.Html.FromHtml("<font color='#FF8000'>" + this.Title + "</font>");
            ActionBar.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.ParseColor("#1f669b")));
            #endregion
            //AddMarker(22, 0, 0);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.map_navigation, menu);            
            return base.OnCreateOptionsMenu(menu);
        }        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {            
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    return true;
                case Resource.Id.menu_settings:
                    StartActivity(new Intent(this, typeof(SettingsActivity)));
                    return true;
                case Resource.Id.menu_share:
                    Functions.Share(this);
                    return true;
                case Resource.Id.menu_layer:
                    ChooseLayer();
                    return true;
                case Resource.Id.menu_refresh:
                    _map.Clear();
                    InitData();
                    break;
                case Resource.Id.menu_hgg:
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(HGG, 17));
                    break;
                case Resource.Id.menu_help:
                    StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://kurzweb.de/umwelt/help.php?id=02")));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        void ChooseLayer()
        {
            var dialog = new AlertDialog.Builder(this);
            dialog.SetView(Resource.Layout.ChooseMapLayer);
            dialog.SetCancelable(false);
            dialog.SetTitle("KartenTyp auswählen");
            var view = dialog.Show();
            view.FindViewById<ImageButton>(Resource.Id.imageButton1).Click += delegate
            {
                _map.MapType = GoogleMap.MapTypeNormal;
                view.Cancel();
            };
            view.FindViewById<ImageButton>(Resource.Id.imageButton2).Click += delegate
            {
                _map.MapType = GoogleMap.MapTypeSatellite;
                view.Cancel();
            };
            view.FindViewById<ImageButton>(Resource.Id.imageButton3).Click += delegate
            {
                _map.MapType = GoogleMap.MapTypeTerrain;
                view.Cancel();
            };
        }

        public View GetInfoContents(Marker marker)
        {
            #region Root
            var root = new LinearLayout(this);
            root.Orientation = Orientation.Vertical;
            #endregion
            #region Title
            var Title = new TextView(this);
            Title.Gravity = GravityFlags.Center;
            Title.SetTextColor(Color.Black);
            Title.Text = marker.Title;
            Title.SetTypeface(null, TypefaceStyle.Bold);
            root.AddView(Title);
            #endregion
            #region Snippet
            var Snippet = new TextView(this);
            Snippet.SetTextColor(Color.Black);
            Snippet.Text = marker.Snippet;
            root.AddView(Snippet);
            #endregion
            return root;
        }
        public View GetInfoWindow(Marker marker)
        {
            return null;
        }
    }
}