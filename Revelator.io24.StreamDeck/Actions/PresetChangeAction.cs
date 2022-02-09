using Revelator.io24.Api;
using Revelator.io24.StreamDeck.Settings;
using SharpDeck;
using SharpDeck.Events.Received;
using System.Diagnostics;

namespace Revelator.io24.StreamDeck.Actions
{
    [StreamDeckAction("com.oddbear.revelator.io24.presetchange")]
    public class PresetChangeAction : StreamDeckAction
    {
        private readonly Microphones _microphones;

        private int _preset;

        public PresetChangeAction(
            Microphones microphones)
        {
            _microphones = microphones;
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            var settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            _preset = settings.Preset;
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
            _microphones.PresetUpdated += PresetUpdated;

            var settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            _preset = settings.Preset;

            await StateUpdated(settings.Channel);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            _microphones.PresetUpdated -= PresetUpdated;
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            var settings = args.Payload
                .GetSettings<PresetChangeSettings>();

            _preset = settings.Preset;

            _microphones.SetPreset(settings.Channel, settings.Preset);

            await StateUpdated(settings.Channel);
        }

        private async void PresetUpdated(object? sender, MicrophoneChannel e)
        {
            try
            {
                await StateUpdated(e);
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
            }
        }

        private async Task StateUpdated(MicrophoneChannel channel)
        {
            var presetSelected = _microphones.GetPreset(channel) == _preset;
            var state = presetSelected ? 0 : 1;

            await SetStateAsync(state);
        }
    }
}
