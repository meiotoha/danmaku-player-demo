using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace App1
{
    public delegate void DanmakuRecievedHandler(string danmaku);

    public class DanmakuHub : IDisposable
    {
     
        public event DanmakuRecievedHandler DanmakuRecieved;
        public DanmakuHub()
        {

        }

        private HubConnection _danmakuConnection;

        public async Task Start(string url,string channel)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();
            connection.On<string>(channel, Handler);
            await connection.StartAsync();
            _danmakuConnection = connection;
        }

        private void Handler(string obj)
        {
            DanmakuRecieved?.Invoke(obj);
        }

        public async Task Stop()
        {
            if (_danmakuConnection != null)
            {
                await _danmakuConnection.StopAsync();
                await _danmakuConnection.DisposeAsync();
                _danmakuConnection = null;
            }
        }

        public async void Dispose()
        {
            await Stop();
        }
    }
}
