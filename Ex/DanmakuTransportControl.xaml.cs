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
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace App1.Ex
{
    public sealed partial class DanmakuTransportControl : MediaTransportControls
    {
        public event TypedEventHandler<ICanvasAnimatedControl, CanvasAnimatedUpdateEventArgs> Update;
        public event TypedEventHandler<ICanvasAnimatedControl, CanvasAnimatedDrawEventArgs> Draw;
        public DanmakuTransportControl()
        {
            this.InitializeComponent();
        }

        private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            Update?.Invoke(sender, args);
        }

        private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            Draw?.Invoke(sender, args);
        }
    }
}
