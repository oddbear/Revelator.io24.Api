using SharpDeck;
using SharpDeck.Enums;
using SharpDeck.Events.Received;

namespace Revelator.io24.StreamDeck.Actions
{
    public abstract class ActionBase<TSettings> : StreamDeckAction
        where TSettings : class, new()
    {
        //We need some how to know the settings when Events are received.
        //In other situations, use GetSettings.
        protected TSettings _settings { get; private set; } = new ();

        protected async Task RefreshState()
        {
            var state = GetButtonState() ? 0 : 1;
            await SetStateAsync(state);
        }

        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            _settings = args.Payload
                .GetSettings<TSettings>() ?? new ();

            await SettingsChanged();

            await RefreshState();
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);

            _settings = args.Payload
                .GetSettings<TSettings>() ?? new();

            RegisterCallbacks();

            await SettingsChanged();

            await RefreshState();
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);

            UnregisterCallbacks();
        }

        protected override async Task OnKeyUp(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyUp(args);

            _settings = args.Payload
                .GetSettings<TSettings>() ?? new();

            OnButtonPress();

            var state = GetButtonState() ? 0 : 1;
            await SetStateAsync(state);
        }

        protected abstract void RegisterCallbacks();
        protected abstract void UnregisterCallbacks();
        protected abstract void OnButtonPress();
        protected abstract bool GetButtonState();
        protected abstract Task SettingsChanged();

        protected async Task SetImageStates(string on, string off)
        {
            try
            {
                var onImageBytes = File.ReadAllBytes($"./Images/Plugin/{on}.png");
                var onBase64 = Convert.ToBase64String(onImageBytes);
                await SetImageAsync("data:image/png;base64," + onBase64, TargetType.Both, 0);

                var offImageBytes = File.ReadAllBytes($"./Images/Plugin/{off}.png");
                var offBase64 = Convert.ToBase64String(offImageBytes);
                await SetImageAsync("data:image/png;base64," + offBase64, TargetType.Both, 1);
            }
            catch
            {
                await SetImageAsync(null);
            }
        }
    }
}
