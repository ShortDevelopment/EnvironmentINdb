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
using Android.Graphics.Drawables;

namespace EnvironmentINdb
{
    [Activity(Label = "Einstellungen",Enabled = true, Name = "com.EnvironmentINdb.EnvironmentINdb.Settings", Exported = true, Icon = "@drawable/Settings", Theme = "@style/MyTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    
    public class SettingsActivity : Activity
    {        
        ListViewManager _ListViewManager;
        ISharedPreferences Settings;
        ISharedPreferencesEditor SettingsEditor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SettingsLayout);
            #region ActionBar
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetDisplayShowTitleEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);
            ActionBar.SetHomeAsUpIndicator(Resources.GetDrawable(Resource.Drawable.Back3));
            ActionBar.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.ParseColor("#1f669b")));
            #endregion
            //Toast.MakeText(this, Intent.Extras.ToString(), ToastLength.Long).Show();
            Settings = this.GetSharedPreferences(this.PackageName + Global.ApplicationInfo.VersionName+".Settings", FileCreationMode.MultiProcess);
            SettingsEditor = Settings.Edit();
            _ListViewManager = new ListViewManager(this, FindViewById<ListView>(Resource.Id.listView1));
            _ListViewManager.SetTab(ListViewManager.Tabs.Home);
            if(Intent.Extras != null)
            {
                _ListViewManager.SetTab(ListViewManager.Tabs.LiveWallpaper);
            }
        }
        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            Window.SetTitle(Functions.FormatTitle("Einstellungen"));
        }
        public override void OnBackPressed()
        {
            if (_ListViewManager.CanGoBack)
            {
                _ListViewManager.GoBack();
            }
            else
            {               
                base.OnBackPressed();
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.settings_navigation, menu);
            return base.OnCreateOptionsMenu(menu);
        }        
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    return true;
                case Resource.Id.menu_restore:
                    Functions.FunctionNotImplemented(this);
                    return true;
                case Resource.Id.menu_share:
                    Functions.Share(this);
                    return true;
                case Resource.Id.menu_help:
                    StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse("http://kurzweb.de/umwelt/help.php?id=03")));
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        class ListViewManager
        {
            #region Variablen
            Activity BaseActivity;
            ListView MainListView;
            Tabs _AktuellerTab;
            List<SettingItem> items = new List<SettingItem>();
            #endregion
            #region Tabs
            public Tabs AktuellerTab
            {
                get
                {
                    return _AktuellerTab;
                }
            }
            public CusotmListAdapter HomeTabAdapter
            {
                get
                {
                    List<SettingItem> mlist = new List<SettingItem>();
                    var a = new SettingItem();
                    a.Text = "Upload";
                    a.SetBitmap(Resource.Drawable.Upload);
                    mlist.Add(a);
                    var b = new SettingItem();
                    b.Text = "Live-Wallpaper";
                    b.SetBitmap(Resource.Drawable.wallpaper);
                    mlist.Add(b);
                    var c = new SettingItem();
                    c.Text = "Design";
                    c.SetBitmap(Resource.Drawable.design);
                    mlist.Add(c);
                    var d = new SettingItem();
                    d.Text = "DeveloperMode";
                    d.SetBitmap(Resource.Drawable.devmode);
                    mlist.Add(d);
                    var e = new SettingItem();
                    e.Text = "Open Source-Lizenzen";
                    e.SetBitmap(Resource.Drawable.attach);
                    mlist.Add(e);
                    CusotmListAdapter adapter = new CusotmListAdapter(BaseActivity, mlist);
                    return adapter;
                }
            }
            public CusotmListAdapter LiveWallpaperAdapter
            {
                get
                {
                    List<SettingItem> mlist = new List<SettingItem>();
                    var a = new SettingItem();
                    a.Text = "Hintergrund bewegt sich";
                    a.Description = "Legt fest, ob sich der Hintergrund beim Wischen bewegt";
                    a.HasCheckBox = true;
                    a.Checked = ((SettingsActivity)BaseActivity).Settings.GetBoolean("Hintergrund_bewegt_sich", true);
                    a.ClickEvent = delegate { ((SettingsActivity)BaseActivity).SettingsEditor.PutBoolean("Hintergrund_bewegt_sich", a.Checked); ((SettingsActivity)BaseActivity).SettingsEditor.Apply(); };
                    mlist.Add(a);
                    var e = new SettingItem();
                    e.Text = "Touch-Position darstellen";
                    e.Description = "Legt fest, ob man die Touch-Position sieht";
                    e.HasCheckBox = true;
                    e.Checked = ((SettingsActivity)BaseActivity).Settings.GetBoolean("show_touch", true);
                    e.ClickEvent = delegate { ((SettingsActivity)BaseActivity).SettingsEditor.PutBoolean("show_touch", e.Checked); ((SettingsActivity)BaseActivity).SettingsEditor.Apply(); };
                    mlist.Add(e);
                    var c = new SettingItem();
                    c.Text = "Festlegen";
                    c.Description = "Öffnet den Dialog, um den aktuellen Hintergrund zu ändern";
                    c.ClickEvent = delegate { Functions.OpenLiveWallPaperChooser(BaseActivity); };
                    mlist.Add(c);
                    var b = new SettingItem();
                    b.Text = "Resourcepack installieren";
                    b.Description = "Packet installieren, dass das Aussehen des Hintergrundes beeinflusst";
                    b.Enabled = false;
                    b.ClickEvent = delegate { Functions.FunctionNotImplemented(BaseActivity); };
                    mlist.Add(b);
                    var d = new SettingItem();
                    d.Text = "Resourcepack erstellen";
                    d.Description = "Packet erstellen, dass das Aussehen der Anwendung beeinflusst";
                    d.ClickEvent += delegate
                    {
                        Functions.FunctionNotImplemented(BaseActivity);
                    };
                    mlist.Add(d);
                    CusotmListAdapter adapter = new CusotmListAdapter(BaseActivity, mlist);
                    return adapter;
                }
            }
            public CusotmListAdapter DesignAdapter
            {
                get
                {
                    List<SettingItem> mlist = new List<SettingItem>();
                    var a = new SettingItem();
                    a.Text = "Resourcepack installieren";
                    a.Description = "Packet installieren, dass das Aussehen der Anwendung beeinflusst";
                    a.Enabled = false;
                    mlist.Add(a);
                    var b = new SettingItem();
                    b.Text = "Resourcepack erstellen";
                    b.Description = "Packet erstellen, dass das Aussehen der Anwendung beeinflusst";
                    b.ClickEvent = delegate
                    {
                        Functions.FunctionNotImplemented(BaseActivity);
                    };
                    mlist.Add(b);
                    CusotmListAdapter adapter = new CusotmListAdapter(BaseActivity, mlist);
                    return adapter;
                }
            }
            public CusotmListAdapter UploadTabAdapter
            {
                get
                {
                    List<SettingItem> mlist = new List<SettingItem>();
                    var b = new SettingItem();
                    b.Text = "Automatisch Hochladen";
                    b.HasCheckBox = true;
                    b.Checked = ((SettingsActivity)BaseActivity).Settings.GetBoolean("Automatisch_Hochladen", false);
                    b.Description = "Legt fest, ob die Daten manuell (per Knopfdruck) oder automatisch an den Server übermittelt werden";
                    b.ClickEvent = delegate {
                        ((SettingsActivity)BaseActivity).SettingsEditor.PutBoolean("Automatisch_Hochladen", b.Checked); ((SettingsActivity)BaseActivity).SettingsEditor.Apply();
                    };
                    if (((SettingsActivity)BaseActivity).Settings.GetBoolean("Automatisch_Hochladen", false))
                    {
                        b.Enabled = true;
                    }
                    else
                    {
                        b.Enabled = false;
                    }                    
                    mlist.Add(b);
                    var a = new SettingItem();
                    a.Text = "HochladeIntervall";
                    a.Description = "Legt fest, wie oft automatisch hochgeladen werden soll<br/><i style=\"padding:15px;\">Alle 10 Sekunden</i>";
                    a.ClickEvent = delegate { Functions.FunctionNotImplemented(BaseActivity); };
                    mlist.Add(a);
                    CusotmListAdapter adapter = new CusotmListAdapter(BaseActivity, mlist);
                    return adapter;
                }
            }
            public CusotmListAdapter DeveloperModeTabAdapter
            {
                get
                {
                    List<SettingItem> mlist = new List<SettingItem>();                    
                    var a = new SettingItem();
                    a.Text = "ErrorLog anzeigen";
                    a.Description = "Zeigt das ErrorLog an";
                    a.ClickEvent = delegate {
                        var settings = Application.Context.GetSharedPreferences(Application.Context.PackageName + ".DeveloperTool", FileCreationMode.MultiProcess);
                        var value = settings.GetString("ErrorLog", "Es sind noch keine Fehler aufgetreten");
                        var dialog = new AlertDialog.Builder(BaseActivity);
                        dialog.SetTitle("ErrorLog");
                        dialog.SetNeutralButton("OK", delegate { });
                        dialog.SetMessage(value);
                        dialog.Show();
                    };
                    mlist.Add(a);
                    var b = new SettingItem();
                    b.Text = "ErrorLog senden";
                    b.Description = "Sendet das ErrorLog an den Entwickler dieser App";
                    b.ClickEvent = delegate {
                        BaseActivity.RunOnUiThread(delegate
                        {
                            Toast.MakeText(BaseActivity, "Vielen Dank!", ToastLength.Short).Show();
                            try
                            {                            
                                var settings = Application.Context.GetSharedPreferences(Application.Context.PackageName + ".DeveloperTool", FileCreationMode.MultiProcess);
                                var value = settings.GetString("ErrorLog", "Es sind noch keine Fehler aufgetreten");
                                var i = new Intent(Intent.ActionSend);
                                i.PutExtra(Intent.ExtraSubject, "ErrorLog of EnvironemtINdb");
                                i.PutExtra(Intent.ExtraEmail, new string[] { "vb.net@kurzweb.de" });
                                i.PutExtra(Intent.ExtraText, value);
                                i.SetType("message/rfc822");
                                BaseActivity.StartActivity(i);
                            }
                            catch (Exception ex) { Toast.MakeText(BaseActivity, ex.Message, ToastLength.Short).Show(); }
                        });
                    };
                    mlist.Add(b);
                    var c = new SettingItem();
                    c.Text = "ErrorLog löschen";
                    c.Description = "Löscht das ErrorLog";
                    c.ClickEvent = delegate {
                        var settings = Application.Context.GetSharedPreferences(Application.Context.PackageName + ".DeveloperTool", FileCreationMode.MultiProcess);
                        settings.Edit().PutString("ErrorLog", "").Apply();
                        Toast.MakeText(BaseActivity, "ErrorLog gelöscht", ToastLength.Long).Show();
                    };
                    mlist.Add(c);
                    CusotmListAdapter adapter = new CusotmListAdapter(BaseActivity, mlist);
                    return adapter;
                }
            }
            #endregion
            #region main
            public ListViewManager(Activity _Activity, ListView _ListView){
                BaseActivity = _Activity;
                MainListView = _ListView;
                _ListView.ItemClick += _ListView_ItemClick;
            }
            public enum Tabs { Home, Upload, LiveWallpaper, Design, DeveloperMode }
            public void SetTab(Tabs tab)
            {
                _AktuellerTab = tab;
                try
                {
                    if (tab == Tabs.Home)
                    {
                        BaseActivity.Window.SetTitle(Functions.FormatTitle("Einstellungen"));
                    }
                    else
                    {
                        BaseActivity.Window.SetTitle(Functions.FormatTitle(tab.ToString()));
                    }
                }
                catch { }                
                HistoryList.Add(tab);
                switch (tab){
                    case Tabs.Home:
                        MainListView.Adapter = HomeTabAdapter;
                        items = HomeTabAdapter.ItemList;
                        return;
                    case Tabs.LiveWallpaper:
                        MainListView.Adapter = LiveWallpaperAdapter;
                        items = LiveWallpaperAdapter.ItemList;
                        return;
                    case Tabs.Design:
                        MainListView.Adapter = DesignAdapter;
                        items = DesignAdapter.ItemList;
                        return;
                    case Tabs.Upload:
                        MainListView.Adapter = UploadTabAdapter;
                        items = UploadTabAdapter.ItemList;
                        return;
                    case Tabs.DeveloperMode:
                        MainListView.Adapter = DeveloperModeTabAdapter;
                        items = DeveloperModeTabAdapter.ItemList;
                        return;
                }                
            }
            #endregion
            #region Clicked
            private void _ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
            {               
                switch (_AktuellerTab)
                {
                    case Tabs.Home:                        
                        switch (items[e.Position].Text)
                        {
                            case "Live-Wallpaper":
                                SetTab(Tabs.LiveWallpaper);
                                return;
                            case "Design":
                                SetTab(Tabs.Design);
                                return;
                            case "Upload":
                                SetTab(Tabs.Upload);
                                return;
                            case "DeveloperMode":
                                SetTab(Tabs.DeveloperMode);
                                return;
                            case "Open Source-Lizenzen":
                                try
                                {                                
                                    var a = new Intent();
                                    a.SetAction(Intent.ActionView);
                                    a.SetDataAndType(Android.Net.Uri.Parse("http://kurzweb.de/umwelt/app_license.pdf"), "application/pdf");
                                    BaseActivity.StartActivity(a);
                                }
                                catch(Exception ex) {
                                    Functions.SaveError(ex, "Show-Open-Source-Licenses");
                                    var alert = new AlertDialog.Builder(BaseActivity);
                                    alert.SetMessage("Die Datei http://kurzweb.de/umwelt/app_license.pdf kann nicht angezeigt werden.");
                                    alert.SetTitle("Fehler");
                                    alert.Show();
                                }
                                return;
                        }
                        return;                    
                    default:
                        try
                        {
                            items[e.Position].ClickEvent.Invoke();                       
                        }
                        catch { }
                        return;
                }
            }     
            #endregion
            #region History
            private List<Tabs> HistoryList = new List<Tabs>();
            public void GoBack()
            {
                try
                {
                    SetTab(HistoryList[HistoryList.Count - 2]);                    
                    HistoryList.RemoveAt(HistoryList.Count - 1);
                    HistoryList.RemoveAt(HistoryList.Count - 1);
                }
                catch { }                               
            }
            public bool CanGoBack
            {
                get
                {                    
                    if (HistoryList.Count > 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            #endregion
        }
    }
}