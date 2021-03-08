using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;
using Windows.Web.Http.Headers;
using Microsoft.Graphics.Canvas.Geometry;

namespace App1
{
    class DanmakuGame : IDisposable
    {
        private readonly List<Danmaku> _danmakuList = new List<Danmaku>();
        private Size _panelSize;
        private Random ra = new Random();

        public DanmakuGame()
        {

        }

        public void Update(Size panelSize)
        {
            _panelSize = panelSize;
            foreach (var danmaku in _danmakuList.ToArray())
            {
                danmaku.Update(panelSize);
            }
            _danmakuList.RemoveAll(x => x.ShouldRemove);
        }

        public void Draw(CanvasDrawingSession dr, Size panelSize)
        {
            foreach (var danmaku in _danmakuList.ToArray())
            {
                danmaku.Draw(dr, panelSize);
            }

            dr.Transform = Matrix3x2.Identity;
        }

        public void TryAdd(string str)
        {
            if (_danmakuList.Count > 100)
            {
                return;
            }

            var y = GetFreeLineIndex();
            if (y > 0)
            {
                _danmakuList.Add(new Danmaku(str, y, _panelSize));
            }
        }

        private int GetFreeLineIndex()
        {
            var lines = (int)(_panelSize.Height / 28);
            int errcnt = 0;
            var danmakuList = this._danmakuList.ToArray();

            while (true)
            {
                var y = ra.Next(0, lines) * 28;
                var has = danmakuList
                    .Where(x => Math.Abs(x.Position.Y - y) < 10).FirstOrDefault(x => x.EndX + 40 > _panelSize.Width);

                if (has != null)
                {
                    errcnt = errcnt + 1;
                    if (errcnt > 5)
                    {
                        return -1;
                    }
                    continue;
                }
                return y;
            }
        }

        public void Dispose()
        {
            _danmakuList.Clear();
        }
    }

    public class Danmaku
    {
        private static float _speed = 120 / 1000f;
        private static int _fontSize = 24;
        private static string _fontFamily = "SimHei";

        private static readonly CanvasTextFormat _textFormat = new CanvasTextFormat
        {
            FontSize = _fontSize,
            Direction = CanvasTextDirection.LeftToRightThenTopToBottom,
            VerticalAlignment = CanvasVerticalAlignment.Top,
            HorizontalAlignment = CanvasHorizontalAlignment.Left,
            FontFamily = _fontFamily
        };

        public Danmaku(string content, int y, Size panelSize)
        {
            CreateTime = DateTime.Now;
            Position = new Vector2((float)panelSize.Width, y);
            Content = content;
            using (var layout = new CanvasTextLayout(CanvasDevice.GetSharedDevice(false), content, _textFormat, (float)panelSize.Width, (float)panelSize.Height))
            {
                Width = (int)Math.Ceiling(layout.DrawBounds.Width);
                Height = (int)Math.Ceiling(layout.DrawBounds.Height);
            }
            ShouldDraw = true;
        }

        public DateTime CreateTime { get; }
        public string Content { get; }
        public int Width { get; }
        public int Height { get; }
        public Vector2 Position { get; private set; }
        public bool ShouldRemove => Position.X < -Width;
        public bool ShouldDraw { get; private set; }

        public float EndX => Position.X + Width;


        public void Update(Size panelSize)
        {
            if (Position.Y > panelSize.Height)
            {
                ShouldDraw = false;
                return;
            }
            ShouldDraw = true;
            var totalSec = (DateTime.Now - CreateTime).TotalMilliseconds;
            var run = totalSec * _speed;
            var x = panelSize.Width - run;
            Position = new Vector2((float)x, Position.Y);
        }

        public void Draw(CanvasDrawingSession dr, Size panelSize)
        {
            if (!ShouldDraw || ShouldRemove)
            {
                return;
            }

            if (Position.X == 0)
            {
                
            }

            if (Position.Y == 0)
            {

            }


            using (var textLayout = new CanvasTextLayout(dr, Content, _textFormat, (float)panelSize.Width, (float)panelSize.Height))
            {
                dr.DrawTextLayout(textLayout, Position, Color.FromArgb(188, 255, 255, 255));

                using (var textGeometry = CanvasGeometry.CreateText(textLayout))
                {
                    var dashedStroke = new CanvasStrokeStyle()
                    {
                        DashStyle = CanvasDashStyle.Solid
                    };
                    dr.DrawGeometry(textGeometry,Position, Color.FromArgb(127, 0, 0, 0), (float)1, dashedStroke);
                }
            }
            //dr.DrawText(Content, Position, Color.FromArgb(127, 255, 255, 255), _textFormat);
        }
    }
}
