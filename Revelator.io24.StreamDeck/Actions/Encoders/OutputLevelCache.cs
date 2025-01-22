using Revelator.io24.Api;
using System.ComponentModel;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

internal class OutputLevelCache : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Device _device;
    private readonly CacheTimer _cacheTimer;

    public OutputLevelCache(Device device)
    {
        _device = device;

        _cacheTimer = new CacheTimer(NamedPropertyChanged);

        device.Global.PropertyChanged += VolumeEngineMockPropertyChanged;
    }

    private void VolumeEngineMockPropertyChanged(object? sender, PropertyChangedEventArgs e)
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
        get => _cacheTimer.GetValueOr(_device.Global.MainOutVolume);
        set => _cacheTimer.SetValue(_device.Global.MainOutVolume = value);
    }

    public float HeadphonesVolume
    {
        get => _cacheTimer.GetValueOr(_device.Global.HeadphonesVolume);
        set => _cacheTimer.SetValue(_device.Global.HeadphonesVolume = value);
    }

    public float MonitorBlend
    {
        get => _cacheTimer.GetValueOr(_device.Global.MonitorBlend);
        set => _cacheTimer.SetValue(_device.Global.MonitorBlend = value);
    }
}
