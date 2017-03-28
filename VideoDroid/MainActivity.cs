using Android.App;
using Android.Widget;
using Android.OS;
using Android.Net;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace VideoDroid
{
    [Activity(Label = "VideoDroid", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : Activity
    {
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

            panel = (LinearLayout)FindViewById(Resource.Id.controlPanel);
            
            btnDownload = (Button)FindViewById(Resource.Id.btnDownload);
            btnDownload.Click += BtnDownload_Click;

            btnPlay = (Button)FindViewById(Resource.Id.btnPlay);
            btnPlay.Click += BtnPlay_Click;

            progressBar = (ProgressBar)FindViewById(Resource.Id.barProgress);
            progressBar.Progress = 0;
        }

        #region Views
        VideoView video;
        LinearLayout panel;
        Button btnDownload;
        Button btnPlay;
        ProgressBar progressBar;
        #endregion

        private async void BtnDownload_Click(object sender, System.EventArgs e)
        {
            btnDownload.Enabled = false;
            await DownloadFileFromWeb(PATH1, "movie1.mkv");
            await DownloadFileFromWeb(PATH2, "movie2.mlv");
            await DownloadFileFromWeb(PATH3, "movie3.mlv");
            await DownloadFileFromWeb(PATH4, "movie4.mlv");
            await DownloadFileFromWeb(PATH5, "movie5.mlv");
            await DownloadFileFromWeb(PATH6, "movie6.mlv");

            btnPlay.Enabled = true;
        }

        private async Task DownloadFileFromWeb(string url, string fileName)
        {
            var webClient = new WebClient();
            var folder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
            var filePath = Path.Combine(folder, fileName);
            await webClient.DownloadFileTaskAsync(url, filePath);
            progressBar.Progress += 1;
            filePaths.Add(filePath);
        }

        List<string> filePaths = new List<string>();
        
        private void ControlInVisible()
        {
            panel.Visibility = Android.Views.ViewStates.Gone;
        }

        private async void BtnPlay_Click(object sender, System.EventArgs e)
        {
            ControlInVisible();
            await PlayFromLocal();
        }

        private async Task PlayFromLocal()
        {
            foreach(var filePath in filePaths)
            {
                video.SetVideoPath(filePath);
                video.Start();
                await Task.Delay(100000);
            }
        }
        
        string PATH1 = @"https://xxxx/movie1.mkv";
        string PATH2 = @"https://xxxx/movie2.mkv";
        string PATH3 = @"https://xxxx/movie3.mkv";
        string PATH4 = @"https://xxxx/movie4.mkv";
        string PATH5 = @"https://xxxx/movie5.mkv";
        string PATH6 = @"https://xxxx/movie6.mkv";
    }
}

