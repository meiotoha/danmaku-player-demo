using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using FFmpegInterop;

namespace App1
{
    public class PlayerManager : IDisposable
    {
        private readonly MediaPlayer _mediaPlayer;
        private FFmpegInteropMSS _currentMss;
        private MediaPlaybackItem _playbackItem;

        public PlayerManager(MediaPlayer player)
        {
            _mediaPlayer = player;
        }

        public bool HasMediaSource => _currentMss != null;

     
        public void PlayByFFMpeg(FFmpegInteropMSS mss)
        {
            _currentMss = mss;
            _playbackItem = _currentMss.CreateMediaPlaybackItem();
            _mediaPlayer.Source = _playbackItem;
            _mediaPlayer.Play();
        }

        public async Task Play(string url)
        {
            try
            {
                Stop();
                var result = await FFmpegInterop.FFmpegInteropMSS.CreateFromUriAsync(url);
                PlayByFFMpeg(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task Play(StorageFile file)
        {
            try
            {
                Stop();
                var sr = await file.OpenAsync(FileAccessMode.Read);
                var result = await FFmpegInterop.FFmpegInteropMSS.CreateFromStreamAsync(sr);
                PlayByFFMpeg(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public void Stop()
        {
            _mediaPlayer.Source = null;
            _playbackItem = null;
            _currentMss?.Dispose();
            _currentMss = null;
        }

        public async Task PlaySubtitle(StorageFile file)
        {
            if (_playbackItem != null)
            {
                _playbackItem.TimedMetadataTracksChanged += PlaybackItem_TimedMetadataTracksChanged;
            }

            if (file != null)
            {
                var sr = await file.OpenAsync(FileAccessMode.Read);
                await _currentMss.AddExternalSubtitleAsync(sr, file.Name);
            }
        }

        public void StopSubtitle()
        {
            for (uint i = 0; i < _playbackItem.TimedMetadataTracks.Count; i++)
            {
                _playbackItem.TimedMetadataTracks.SetPresentationMode(i, TimedMetadataTrackPresentationMode.Disabled);
            }
        }

        private void PlaybackItem_TimedMetadataTracksChanged(MediaPlaybackItem sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            if (args.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemInserted)
            {
                sender.TimedMetadataTracksChanged -= PlaybackItem_TimedMetadataTracksChanged;

                // unselect other subs
                for (uint i = 0; i < sender.TimedMetadataTracks.Count; i++)
                {
                    sender.TimedMetadataTracks.SetPresentationMode(i, TimedMetadataTrackPresentationMode.Disabled);
                }

                // pre-select added subtitle
                sender.TimedMetadataTracks.SetPresentationMode(args.Index, TimedMetadataTrackPresentationMode.PlatformPresented);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
