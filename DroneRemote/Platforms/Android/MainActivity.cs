﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace DroneRemote;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.SensorLandscape)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        //隐藏状态栏
        Window.SetFlags(Android.Views.WindowManagerFlags.Fullscreen, Android.Views.WindowManagerFlags.Fullscreen);

        //半透明任务栏
        // Window.SetFlags(Android.Views.WindowManagerFlags.TranslucentStatus, Android.Views.WindowManagerFlags.TranslucentStatus);

        //全透明任务栏
        //Window.SetFlags(Android.Views.WindowManagerFlags.TranslucentNavigation, Android.Views.WindowManagerFlags.TranslucentNavigation);

        //设置状态栏、导航栏色颜色为透明
        //Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        //Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);

        base.OnCreate(savedInstanceState);
    }
}
