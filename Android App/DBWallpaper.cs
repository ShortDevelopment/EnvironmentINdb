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
using Android.Service.Wallpaper;
using Android.Graphics;
using System.Reflection;
using Android.Media;

namespace EnvironmentINdb
{
    [Service(Label = "DezibelHintergrund", Permission = "android.permission.BIND_WALLPAPER")]
    [IntentFilter(new string[] { "android.service.wallpaper.WallpaperService" })]
    [MetaData("android.service.wallpaper", Resource = "@xml/wallpaper1")]
    class DBWallpaper : WallpaperService
    {
        public override WallpaperService.Engine OnCreateEngine()
        {
            var settings = GetSharedPreferences(this.PackageName + Global.ApplicationInfo.VersionName + ".Settings", FileCreationMode.MultiProcess);
            var img = BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.env);
            return new dbBack(this, img, settings);
        }
        
        class dbBack : WallpaperService.Engine
        {
            Bitmap BackBMP;
            MediaRecorder mediaRecorder;
            double LevelBefore = 0;
            private Handler mHandler = new Handler();
            private PointF center = new PointF();
            private PointF touch_point = new PointF(-1, -1);
            private float offset;            
            private Action DrawIMGAction;
            private bool is_visible;
            private float offset2;
            ISharedPreferences Settings;
            public dbBack(WallpaperService wall, Bitmap bmp, ISharedPreferences _settings) : base(wall)
            {
                BackBMP = bmp;
                Settings = _settings;                                
                DrawIMGAction = delegate {
                    try
                    {
                        Functions.RunOnNewThread(delegate {
                            DrawFrame();
                        });
                    }
                    catch
                    {
                        DrawFrame();
                    }
                };
            }
            public override void OnCreate(ISurfaceHolder surfaceHolder)
            {
                base.OnCreate(surfaceHolder);                
                SetTouchEventsEnabled(true);
                try
                {                
                    mediaRecorder = new MediaRecorder();
                    mediaRecorder.SetAudioSource(AudioSource.Default);
                    mediaRecorder.SetOutputFormat(OutputFormat.ThreeGpp);
                    mediaRecorder.SetAudioEncoder(AudioEncoder.AmrNb);
                    mediaRecorder.SetOutputFile("/dev/null");
                    mediaRecorder.Prepare();
                    mediaRecorder.Start();
                }
                catch { }
            }
            public override void OnDestroy()
            {
                base.OnDestroy();
                mHandler.RemoveCallbacks(DrawIMGAction);
                try
                {
                    mediaRecorder.Stop();
                    mediaRecorder.Release();
                    mediaRecorder.Dispose();
                }
                catch { }                
            }
            public override void OnVisibilityChanged(bool visible)
            {
                is_visible = visible;
                if (visible)
                    DrawFrame();
                else
                    mHandler.RemoveCallbacks(DrawIMGAction);
            }
            public override void OnSurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
            {
                base.OnSurfaceChanged(holder, format, width, height);                
                center.Set(width / 2.0f, height / 2.0f);
                DrawFrame();
            }
            public override void OnSurfaceDestroyed(ISurfaceHolder holder)
            {
                base.OnSurfaceDestroyed(holder);
                is_visible = false;
                mHandler.RemoveCallbacks(DrawIMGAction);
            }
            public override void OnOffsetsChanged(float xOffset, float yOffset, float xOffsetStep, float yOffsetStep, int xPixelOffset, int yPixelOffset)
            {
                offset = xOffset * 250;
                offset2 = yOffset * 250;
                //System.Diagnostics.Debug.Print("OnOffsetsChanged => xOffset:" + xOffset.ToString() + "|yOffset:" + yOffset.ToString() + "|xOffsetStep:" + xOffsetStep.ToString() + "|yOffsetStep:" + yOffsetStep.ToString());
                DrawFrame();                
            }            
            public override void OnTouchEvent(MotionEvent e)
            {
                if (e.Action == MotionEventActions.Move)
                    touch_point.Set(e.GetX(), e.GetY());
                else
                    touch_point.Set(-1, -1);
                base.OnTouchEvent(e);
            }            
            void DrawFrame()
            {                
                ISurfaceHolder holder = SurfaceHolder;
                Canvas c = null;
                try
                {
                    c = holder.LockCanvas();
                    if (c != null)
                    {
                        DrawIMG(c);
                        if (Settings.GetBoolean("show_touch", true) == true)
                        {
                            if (touch_point.X >= 0 && touch_point.Y >= 0)
                            {
                                var paint = new Paint();
                                paint.SetStyle(Paint.Style.Stroke);
                                paint.AntiAlias = true;
                                paint.StrokeWidth = 2;
                                paint.Color = Color.White;
                                paint.StrokeCap = Paint.Cap.Round;                                
                                c.DrawCircle(touch_point.X, touch_point.Y, 80, paint);
                            }                                
                        }
                    }
                }
                finally
                {
                    if (c != null)
                        holder.UnlockCanvasAndPost(c);
                }                
                mHandler.RemoveCallbacks(DrawIMGAction);
                if (is_visible) { 
                    mHandler.PostDelayed(DrawIMGAction, 1000 / 20);
                }
            }
            void DrawIMG(Canvas c)
            {
                c.Save();                            
                c.DrawColor(Color.Green);
                //c.Translate(center.X, center.Y);
                try
                {
                    float abcoffset = 0;
                    if(Settings.GetBoolean("Hintergrund_bewegt_sich", true) == true)
                    {
                        abcoffset = offset;
                    }
                    c.DrawBitmap(BackBMP, (float)(-1*(BackBMP.Width / 2)) - abcoffset, (float)(-1 * (BackBMP.Height / 2)), new Paint());
                }
                catch { }
                double lastLevel = -1;
                try
                {                    
                    double db = 10 * System.Math.Log(mediaRecorder.MaxAmplitude / 10);
                    lastLevel = System.Math.Round(db, 0);
                    if (double.IsInfinity(lastLevel))
                    {
                        lastLevel = LevelBefore;
                    }else { LevelBefore = lastLevel; }
                }catch { }
                var a = new Paint();
                a.Color = Color.Yellow;
                a.SetStyle(Paint.Style.Fill);
                a.TextSize = 50;
                string text;
                if(lastLevel == -1)
                {
                    text = "Berechtigungen?!";
                }
                else { text = lastLevel.ToString() + "db"; }
                c.DrawText(text, 0, 0, a);                
                c.Restore();
            }            
        }
    }
}