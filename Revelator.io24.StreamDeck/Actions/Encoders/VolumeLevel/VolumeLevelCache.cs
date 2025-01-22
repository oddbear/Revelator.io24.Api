using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Revelator.io24.Api;
using Revelator.io24.Api.Enums;
using Revelator.io24.Api.Models;

namespace Revelator.io24.StreamDeck.Actions.Encoders.VolumeLevel;

internal class VolumeLevelCache : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly RoutingTable _routingTable;
    private readonly CacheTimer<(Input, MixOut)> _volumeCacheTimer;

    public VolumeLevelCache(RoutingTable routingTable)
    {
        _routingTable = routingTable;

        _volumeCacheTimer = new CacheTimer<(Input, MixOut)>(NamedPropertyChanged);
    }

    private void VolumeEngineMockPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
            return;

        if (_volumeCacheTimer.Active)
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
        _volumeCacheTimer.SetValue(key, value.ValueRaw);
        _routingTable.SetVolume(input, mixOut, value);
    }

    public VolumeValue GetVolume(Input input, MixOut mixOut)
    {
        var key = (input, mixOut);
        var volumeRaw = _volumeCacheTimer.GetValueOr(key, _routingTable.GetVolume(input, mixOut).ValueRaw);
        return new VolumeValue { ValueRaw = volumeRaw };
    }
}
