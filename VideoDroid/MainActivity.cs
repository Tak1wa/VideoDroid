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
using Android.Media;
using Android.Views;

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
        }

        public async override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (hasFocus)
            {
                progressBar = (ProgressBar)FindViewById(Resource.Id.barProgress);
                await DownloadProcessAsync();

                Toast.MakeText(this, "Download completed.", ToastLength.Long).Show();
                await Task.Delay(5000);
                progressBar.Visibility = ViewStates.Gone;

                await PlayFromLocalAsync();
            }
        }
        #endregion

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
            InitialVideo();
            
            var counter = 0;
            while(true)
            {
                if(counter == filePaths.Count - 1)
                {
                    counter = 0;
                }
                await CurrentPlayAndNextPrepare(filePaths[counter++]);
                await Task.Delay(10000);
            }
        }

        #region Play

        SurfaceView videoView;

        MediaPlayer prePlayer = null;
        MediaPlayer currentPlayer = null;
        MediaPlayer nextPlayer = null;

        private void InitialVideo()
        {
            videoView = (SurfaceView)FindViewById(Resource.Id.videoView);
        }

        private async Task CurrentPlayAndNextPrepare(string nextMovieFile)
        {
            currentPlayer = nextPlayer;
            nextPlayer = null;

            if(prePlayer != null)
            {
                prePlayer.Stop();
                prePlayer.Release();
                prePlayer = null;
            }

            if(currentPlayer != null)
            {
                currentPlayer.SetDisplay(videoView.Holder);
                currentPlayer.Start();
                prePlayer = currentPlayer;
            }

            nextPlayer = new MediaPlayer();
            await nextPlayer.SetDataSourceAsync(nextMovieFile);
            nextPlayer.PrepareAsync();
        }
        
        #endregion

        #endregion

    }
}

