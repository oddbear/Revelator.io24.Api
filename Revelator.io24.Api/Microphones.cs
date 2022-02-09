using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Extensions;
using Revelator.io24.Api.Models;
using Revelator.io24.Api.Services;

namespace Revelator.io24.Api
{
    /// <summary>
    /// API for functions bound to the Microphone channels.
    /// - FatChannel Toggle
    /// - Presets Switching
    /// </summary>
    public class Microphones
    {
        public event EventHandler<MicrophoneChannel>? FatChannelUpdated;
        public event EventHandler<MicrophoneChannel>? PresetUpdated;
        public event EventHandler<MicrophoneChannel>? PresetsUpdated;

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
            _microphoneModel.PresetIndexValueUpdated += (sender, channel) => PresetUpdated?.Invoke(this, channel);
        }

        private void SynchronizeReceived(object? sender, EventArgs e)
        {
            FatChannelUpdated?.Invoke(this, MicrophoneChannel.Left);
            FatChannelUpdated?.Invoke(this, MicrophoneChannel.Right);

            PresetsUpdated?.Invoke(this, MicrophoneChannel.Left);
            PresetsUpdated?.Invoke(this, MicrophoneChannel.Right);

            PresetUpdated?.Invoke(this, MicrophoneChannel.Left);
            PresetUpdated?.Invoke(this, MicrophoneChannel.Right);
        }

        public string GetPreset(MicrophoneChannel channel)
            => _microphoneModel.GetPreset(channel);

        public string[] GetPresets(MicrophoneChannel channel)
            => _microphoneModel.GetPresets(channel);

        public void SetPreset(MicrophoneChannel channel, string preset)
        {
            var presets = _microphoneModel.GetPresets(channel);
            var index = Array.IndexOf(presets, preset);
            var valueFloat = index / 13f;

            var route = GetPresetRoute(channel);
            _communicationService.SetRouteValue(route, valueFloat);
        }

        public bool GetFatChannelStatus(MicrophoneChannel channel)
            =>  _microphoneModel.GetFatChannelState(channel);

        public void SetFatChannelStatus(MicrophoneChannel channel, Value value)
        {
            var route = GetByPassDspRoute(channel);
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

        private string GetPresetRoute(MicrophoneChannel channel)
            => channel switch
            {
                MicrophoneChannel.Left => "line/ch1/presets/preset",
                MicrophoneChannel.Right => "line/ch2/presets/preset",
                _ => throw new InvalidOperationException($"Unknown '{nameof(MicrophoneChannel)}' enum value '{channel}'")
            };

        private string GetByPassDspRoute(MicrophoneChannel channel)
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
