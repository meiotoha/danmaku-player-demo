using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace App1.Ex
{
    public sealed partial class OpenChannelDialog : ContentDialog
    {
        public OpenChannelDialog()
        {
            this.InitializeComponent();
#if DEBUG
            PathBox.Text = "http://10.233.233.220:80/Danmaku";
            ChannelBox.Text = "DEFAULT";
#endif
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Url = PathBox.Text.Trim();
            Channel = ChannelBox.Text.Trim();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Url = null;
            Channel = null;

        }
        public string Url { get; private set; }
        public string Channel { get; private set; }

    }
}
