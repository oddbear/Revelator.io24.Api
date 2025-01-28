using System.ComponentModel;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models.ValueConverters;

namespace Revelator.io24.StreamDeck.Actions.Encoders.VolumeLevel;

internal class VolumeLevelCache : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly RoutingTable _routingTable;
    private readonly CacheTimer<(Input, MixOut)> _cacheTimer;

    public VolumeLevelCache(RoutingTable routingTable)
    {
        _routingTable = routingTable;

        _cacheTimer = new CacheTimer<(Input, MixOut)>(NamedPropertyChanged);

        _routingTable.VolumeUpdated += VolumeUpdated;
    }

    private void VolumeUpdated(object? sender, (Input, MixOut) e)
    {
        if (_cacheTimer.Active)
            return;

        NamedPropertyChanged();
    }

    private void NamedPropertyChanged()
    {
        // When last item is set, we wait and then set it to the true value:
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
    
    public void SetVolume(Input input, MixOut mixOut, VolumeValue value)
    {
        var key = (input, mixOut);
        _cacheTimer.SetValue(key, value);
        _routingTable.SetVolume(input, mixOut, value);
    }

    public VolumeValue GetVolume(Input input, MixOut mixOut)
    {
        var key = (input, mixOut);
        return _cacheTimer.GetValueOr(key, _routingTable.GetVolume(input, mixOut));
    }
}
