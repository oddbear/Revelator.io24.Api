using Revelator.io24.Api;
using System.ComponentModel;

namespace Revelator.io24.StreamDeck.Actions.Encoders.OutputLevel;

internal class OutputLevelCache : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Device _device;
    private readonly CacheTimer<string> _cacheTimer;

    public OutputLevelCache(Device device)
    {
        _device = device;

        _cacheTimer = new CacheTimer<string>(NamedPropertyChanged);

        device.Global.PropertyChanged += DeviceGlobalPropertyChanged;
    }

    private void DeviceGlobalPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
            return;

        if (_cacheTimer.Active)
            return;

        NamedPropertyChanged();
    }

    private void NamedPropertyChanged()
    {
        // When last item is set, we wait and then set it to the true value:
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }

    public float MainOutVolume
    {
        get => _cacheTimer.GetValueOr(nameof(MainOutVolume), _device.Global.MainOutVolume);
        set => _cacheTimer.SetValue(nameof(MainOutVolume), _device.Global.MainOutVolume = value);
    }

    public float HeadphonesVolume
    {
        get => _cacheTimer.GetValueOr(nameof(HeadphonesVolume), _device.Global.HeadphonesVolume);
        set => _cacheTimer.SetValue(nameof(HeadphonesVolume), _device.Global.HeadphonesVolume = value);
    }

    public float MonitorBlend
    {
        get => _cacheTimer.GetValueOr(nameof(MonitorBlend), _device.Global.MonitorBlend);
        set => _cacheTimer.SetValue(nameof(MonitorBlend), _device.Global.MonitorBlend = value);
    }
}
