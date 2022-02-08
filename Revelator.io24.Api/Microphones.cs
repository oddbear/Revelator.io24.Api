using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;

namespace Revelator.io24.Api
{
    public class Microphones
    {
        public event EventHandler<MicrophoneChannel>? FatChannelUpdated;

        private readonly CommunicationService _communicationService;
        private readonly MicrophoneModel _microphoneModel;

        public Microphones(
            CommunicationService communicationService,
            MicrophoneModel microphoneModel)
        {
            _communicationService = communicationService;
            _microphoneModel = microphoneModel;

            _microphoneModel.SynchronizeReceived += SynchronizeReceived;
            _microphoneModel.FatChannelValueUpdated += (sender, channel) => FatChannelUpdated?.Invoke(this, channel);
        }

        private void SynchronizeReceived(object? sender, EventArgs e)
        {
            FatChannelUpdated?.Invoke(this, MicrophoneChannel.Left);
            FatChannelUpdated?.Invoke(this, MicrophoneChannel.Right);
        }

        public bool GetFatChannelStatus(MicrophoneChannel channel)
        {
            return _microphoneModel.GetFatChannelState(channel);
        }

        public void SetFatChannelStatus(MicrophoneChannel channel, Value value)
        {
            var route = GetRoute(channel);
            switch (value)
            {
                case Value.On:
                    _communicationService.SetRouteValue(route, 0.0f);
                    break;
                case Value.Off:
                    _communicationService.SetRouteValue(route, 1.0f);
                    break;
                default:
                    var floatValue = GetFatChannelStatus(channel) ? 1.0f : 0.0f;
                    _communicationService.SetRouteValue(route, floatValue);
                    break;
            }
        }

        private string GetRoute(MicrophoneChannel channel)
            => channel switch
            {
                MicrophoneChannel.Left => "line/ch1/bypassDSP",
                MicrophoneChannel.Right => "line/ch2/bypassDSP",
                _ => throw new InvalidOperationException($"Unknown '{nameof(MicrophoneChannel)}' enum value '{channel}'")
            };
    }

    public enum MicrophoneChannel
    {
        Left,
        Right
    }
}
