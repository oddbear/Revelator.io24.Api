using Revelator.io24.Api.Services;
using SharpDeck;
using SharpDeck.Events.Received;

namespace Revelator.io24.StreamDeck
{
    [StreamDeckAction("com.oddbear.revelator.io24.headphonesource")]
    public class HeadphonesSourceAction : StreamDeckAction
    {
        private readonly UpdateService _updateService;

        public HeadphonesSourceAction(UpdateService updateService)
        {
            _updateService = updateService;
            _updateService.RoutingUpdated += RoutingUpdated;
        }

        private async void RoutingUpdated(object? sender, EventArgs e)
        {
            try
            {
                var routing = _updateService.Routing;
                var headponeSource = routing.HeadphonesSource.ToString();

                await base.SetTitleAsync(headponeSource);
            }
            catch (Exception)
            {
                //
            }
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);
        }

        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
        }

        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyDown(args);
        }
    }
}
