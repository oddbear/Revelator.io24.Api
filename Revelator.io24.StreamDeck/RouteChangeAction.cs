using Revelator.io24.Api.Services;
using SharpDeck;
using SharpDeck.Events.Received;

namespace Revelator.io24.StreamDeck
{
    [StreamDeckAction("com.oddbear.revelator.io24.routechange")]
    public class RouteChangeAction : StreamDeckAction
    {
        private readonly Dictionary<string, object> _stateTracker = new ();
        private readonly UpdateService _updateService;

        public RouteChangeAction(UpdateService updateService)
        {
            _updateService = updateService;
            _updateService.RoutingUpdated += _updateService_RoutingUpdated;
        }

        private async void _updateService_RoutingUpdated(object? sender, EventArgs e)
        {
            try
            {
                var routing = _updateService.Routing;
                var headponeSource = routing.HeadphonesSource;

                await base.SetTitleAsync(headponeSource.ToString());
            }
            catch (Exception)
            {
                //
            }
        }

        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);

            _stateTracker[args.Context] = null;
        }

        /// <inheritdoc/>
        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);

            if (_stateTracker.ContainsKey(args.Context))
                _stateTracker.Remove(args.Context);
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyDown(args);
        }
    }
}
