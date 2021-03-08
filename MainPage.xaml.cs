using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Printing;
using Windows.Media.Playback;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using App1.Ex;
using FFmpegInterop;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private readonly DanmakuGame _game;
        private readonly DanmakuHub _hub;
        private readonly MediaPlayer _mediaPlayer;


        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
            _mediaPlayer = new MediaPlayer();
            PlayerControl.SetMediaPlayer(_mediaPlayer);
            _playerManager = new PlayerManager(_mediaPlayer);
            _game = new DanmakuGame();
            _hub = new DanmakuHub();
            _hub.DanmakuRecieved += Hub_DanmakuReceived;
        }
     
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ExtendTitlebar();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Dispose();
            _playerManager?.Dispose();
            _game.Dispose();
            _hub.Dispose();
        }

        private void ExtendTitlebar()
        {
            var applicationView = CoreApplication.GetCurrentView();
            applicationView.TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(MenuBarIns);
            var titlebar = applicationView.TitleBar;

            ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.InactiveBackgroundColor = Colors.Transparent;
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        }

        private void Hub_DanmakuReceived(string danmaku)
        {
            _game.TryAdd(danmaku);
        }

 
        private void CanvasAnimatedControl_OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            _game.Update(sender.Size);
        }
        private void CanvasAnimatedControl_OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            _game.Draw(args.DrawingSession, sender.Size);
        }

        private readonly PlayerManager _playerManager;

        private async void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var filedialog = new FileOpenPicker();
            filedialog.ViewMode = PickerViewMode.List;
            filedialog.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            filedialog.FileTypeFilter.Add("*");
            var file = await filedialog.PickSingleFileAsync();
            if (file != null)
            {
                await _playerManager.Play(file);
            }
        }

        private async void OpenLinkClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenDialog();
            if (await dlg.ShowAsync() == ContentDialogResult.Primary)
            {
                await _playerManager.Play(dlg.Url);
            }
        }

        private async void StartDanmaku(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenChannelDialog();
            if (await dlg.ShowAsync() == ContentDialogResult.Primary)
            {
                await _hub.Start(dlg.Url, dlg.Channel);
            }
        }

        private async void StopDanmaku(object sender, RoutedEventArgs e)
        {
            await _hub.Stop();
        }

        private void StopSubtitle(object sender, RoutedEventArgs e)
        {
            _playerManager.StopSubtitle();
        }

        private async void LoadSubtitle(object sender, RoutedEventArgs e)
        {
            if (_playerManager.HasMediaSource)
            {
                var filedialog = new FileOpenPicker();
                filedialog.ViewMode = PickerViewMode.List;
                filedialog.SuggestedStartLocation = PickerLocationId.VideosLibrary;
                filedialog.FileTypeFilter.Add("*");
                var file = await filedialog.PickSingleFileAsync();
                await _playerManager.PlaySubtitle(file);
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
           PlayerControl.TransportControls.Show();
        }
    }
}
