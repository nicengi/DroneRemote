using Microsoft.Maui.Controls;
using System.Text.Json;

namespace DroneRemote;

public partial class MainPage : ContentPage
{
    private readonly string serverUrl = "http://101.43.217.188:9200/code01/";
    private readonly Dictionary<string, string> commands = new Dictionary<string, string>
    {
        // 无人机操控杆
        { nameof(DroneGo), "44" },
        { nameof(DroneBack), "55" },
        { nameof(DroneLeft), "66" },
        { nameof(DroneRight), "77" },
        { nameof(DroneUp), "88" },
        { nameof(DroneDown), "99" },

        // 摄像头操作杆
        { nameof(CamUp), "SG1/{0}" },
        { nameof(CamDown), "SG1/{0}" },
        { nameof(CamLeft), "SG2/{0}" },
        { nameof(CamRight), "SG2/{0}" },

        // 下方功能区
        { nameof(DroneTakeOff), "0" },
        { nameof(DroneReturn), "1" },
        { nameof(DroneVoice), "2004" },
        { nameof(DroneLight), "2003" },
        { nameof(DroneAnalyze), "2002" },
        { nameof(DronePut), "2001" },

        // 拍照按钮
        { nameof(ButtonShutter), "123" },
    };

    private int angleSG1 = 0;
    private int angleSG2 = 0;

    public int AngleSG1
    {
        get { return angleSG1; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value > 180)
            {
                value = 180;
            }

            angleSG1 = value;
        }
    }

    public int AngleSG2
    {
        get { return angleSG2; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value > 180)
            {
                value = 180;
            }

            angleSG2 = value;
        }
    }

    public MainPage()
    {
        InitializeComponent();
        DroneLight.Clicked += (s, e) =>
        {
            WebViewCam.IsVisible = !WebViewCam.IsVisible;
        };


#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        UpdateStatus(true);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            while (true)
            {
                await UpdateStatus();
                await Task.Delay(1000);
            }
        });
    }

    private async void Command_Process(object sender, EventArgs e)
    {
        Element element = (Element)sender;
        string command = commands[element.ClassId];
        await SendCommand(command);
    }

    private async void Command_Process_SG(object sender, EventArgs e)
    {
        Element element = (Element)sender;
        string command = commands[element.ClassId];

        int angle = 0;

        if (sender == CamUp)
        {
            AngleSG1 += 5;
            angle = AngleSG1;
        }
        else if (sender == CamDown)
        {
            AngleSG1 -= 5;
            angle = AngleSG1;
        }
        else if (sender == CamLeft)
        {
            AngleSG2 -= 5;
            angle = AngleSG2;
        }
        else if (sender == CamRight)
        {
            AngleSG2 += 5;
            angle = AngleSG2;
        }

        await SendCommand(string.Format(command, angle));

        LabelSG1.Text = $"上下云台：{AngleSG1}°";
        LabelSG2.Text = $"左右云台：{AngleSG2}°";
    }

    private void ButtonDebugInfo_Clicked(object sender, EventArgs e)
    {
        FrameDroneInfo.IsVisible = !FrameDroneInfo.IsVisible;
        FrameSensorInfo.IsVisible = !FrameSensorInfo.IsVisible;
    }

    private async void ButtonReConnect_Clicked(object sender, EventArgs e)
    {
        await UpdateStatus(true);
        WebViewCam.Reload();
    }

    private async Task UpdateStatus(bool ignoreVisible = false)
    {
        if (FrameDroneInfo.IsVisible || ignoreVisible)
        {
            try
            {
                ImageSyncState.Source = ImageSource.FromFile("cloud_sync.png");
                Dictionary<string, object> data = await HttpGet("check");

                if (ignoreVisible)
                {
                    LabelDroneState.Text = data["data"].ToString();
                    ImageSyncState.Source = ImageSource.FromFile("cloud_server.png");
                }

                LabelFlyMode.Text = $"飞行模式：{data["Mode"]}";
                LabelVoltage.Text = $"电压：{data["A_v"]}V";
                LabelAltitude.Text = $"高度：{data["Altitude"]}米";
                LabelEKF.Text = $"EKF：{(data["EKF"].ToString() == "flase" ? "NO" : "YES")}";
                LabelCanArmed.Text = $"可解锁：{(data["Can_Armed"].ToString() == "fales" ? "NO" : "YES")}";
                LabelSystem.Text = $"系统状态：{data["System"]}";
                LabelLat.Text = $"经度：{data["Location1"]}°";
                LabelLon.Text = $"纬度：{data["Location2"]}°";

                data = await HttpGet("Wenshidu");
                LabelTemperature.Text = $"温度：{data["Temperature"]}℃";
                LabelHumidity.Text = $"湿度：{data["Humidity"]}%";

                data = await HttpGet("Yanwu");
                LabelFume.Text = $"烟雾：{data["Fumes"]}";

                data = await HttpGet("Guangqiang");
                LabelLight.Text = $"光照：{data["GQ"]} Lux";
            }
            catch (Exception ex)
            {
                LabelDroneState.Text = ex.Message;
                ImageSyncState.Source = ImageSource.FromFile("cloud_offline.png");
            }
        }
    }

    private async Task SendCommand(string command)
    {
        try
        {
            ImageSyncState.Source = ImageSource.FromFile("cloud_sync.png");
            await HttpGet(command);
            Dictionary<string, object> data = await HttpGet("Status");
            LabelDroneState.Text = data["Status"].ToString();
            ImageSyncState.Source = ImageSource.FromFile("cloud_server.png");
        }
        catch (Exception ex)
        {
            LabelDroneState.Text = ex.Message;
            ImageSyncState.Source = ImageSource.FromFile("cloud_offline.png");
        }
    }

    private async Task<Dictionary<string, object>> HttpGet(string url)
    {
        HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.GetAsync($"{serverUrl}{url}");
        string content = await response.Content.ReadAsStringAsync();
        Dictionary<string, object> data = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
        return data;
    }
}

