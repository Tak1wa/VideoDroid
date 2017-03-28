using Android.App;
using Android.Widget;
using Android.OS;
using Android.Net;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace VideoDroid
{
    [Activity(Label = "VideoDroid", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : Activity
    {
        #region life cycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Window.AddFlags(Android.Views.WindowManagerFlags.KeepScreenOn);

            RequestWindowFeature(Android.Views.WindowFeatures.NoTitle);

            Window.DecorView.SystemUiVisibility =
                (Android.Views.StatusBarVisibility)
                    (Android.Views.SystemUiFlags.HideNavigation |
                     Android.Views.SystemUiFlags.Fullscreen |
                     Android.Views.SystemUiFlags.ImmersiveSticky);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            video = (VideoView)FindViewById(Resource.Id.videoView);

            progressBar = (ProgressBar)FindViewById(Resource.Id.barProgress);
            progressBar.Progress = 0;
        }

        public async override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                await DownloadProcessAsync();

                Toast.MakeText(this, "Download completed.", ToastLength.Long);
                await Task.Delay(5000);
                progressBar.Visibility = Android.Views.ViewStates.Gone;

                await PlayFromLocalAsync();
            }
        }
        #endregion

        //Views
        VideoView video;
        ProgressBar progressBar;

        #region Donwload Process

        List<string> _DownloadUrls = new List<string>
        {
            "https://hogehoge/hoge1.mkv",
            "https://hogehoge/hoge2.mkv",
            "https://hogehoge/hoge3.mkv",
            "https://hogehoge/hoge4.mkv",
            "https://hogehoge/hoge5.mkv",
            "https://hogehoge/hoge6.mkv",
        };

        private async Task DownloadProcessAsync()
        {
            progressBar.Visibility = Android.Views.ViewStates.Visible;
            progressBar.Max = _DownloadUrls.Count;
            progressBar.Progress = 0;

            foreach (var current in _DownloadUrls.Select((value, index) => new { index, value }))
            {
                var path = await DownloadFileFromWebAsync(current.value, $"movie{current.index}.mp4");
                if(!string.IsNullOrWhiteSpace(path))
                {
                    filePaths.Add(path);
                }
                progressBar.Progress++;
            }
        }

        private async Task<string> DownloadFileFromWebAsync(string url, string fileName)
        {
            try
            {
                var folder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
                var filePath = Path.Combine(folder, fileName);
                if (File.Exists(filePath) == false)
                {
                    var webClient = new WebClient();
                    await webClient.DownloadFileTaskAsync(url, filePath);
                }
                return filePath;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        #endregion

        #region Play Process

        List<string> filePaths = new List<string>();

        private async Task PlayFromLocalAsync()
        {
            foreach (var filePath in filePaths)
            {
                video.SetVideoPath(filePath);
                video.Start();
                await Task.Delay(5000);
            }
        }
        #endregion

    }
}

