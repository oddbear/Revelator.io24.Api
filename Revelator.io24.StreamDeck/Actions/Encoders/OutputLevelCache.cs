using Revelator.io24.Api;
using System.ComponentModel;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

internal class OutputLevelCache : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly MemoryCache _memoryCache;
    private readonly CacheItemPolicy _policy;
    private readonly Device _device;

    public OutputLevelCache(Device device)
    {
        _device = device;

        device.Global.PropertyChanged += VolumeEngineMockPropertyChanged;

        _memoryCache = MemoryCache.Default;
        _policy = new CacheItemPolicy
        {
            // When the last temp value is set, we fetch the real value and set it:
            SlidingExpiration = TimeSpan.FromSeconds(1),
            RemovedCallback = ExpiredCallback
        };
    }

    private void VolumeEngineMockPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is null)
            return;

        var cacheItem = _memoryCache.GetCacheItem(e.PropertyName);
        if (cacheItem is not null)
            return; // Should be delayed by the expired event.

        // This is an outside event, and we have not changed anything for a while, so we can set it:
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e.PropertyName));
    }

    private void VolumeCachePropertyChanged([CallerMemberName] string propertyName = "")
    {
        // This must always be invoked, so all keypads and dials gets updated, and not only the one we are working with:
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ExpiredCallback(CacheEntryRemovedArguments arguments)
    {
        if (arguments.RemovedReason != CacheEntryRemovedReason.Expired)
            return;

        // When last item is set, we wait and then set it to the true value:
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(arguments.CacheItem.Key));
    }

    public float MainOutVolume
    {
        get
        {
            var tempValue = (float?)_memoryCache.Get(nameof(MainOutVolume));
            return tempValue ?? _device.Global.MainOutVolume;
        }
        set
        {
            _memoryCache.Set(nameof(MainOutVolume), value, _policy);
            _device.Global.MainOutVolume = value;
            VolumeCachePropertyChanged();
        }
    }

    public float HeadphonesVolume
    {
        get
        {
            var tempValue = (float?)_memoryCache.Get(nameof(HeadphonesVolume));
            return tempValue ?? _device.Global.HeadphonesVolume;
        }
        set
        {
            _memoryCache.Set(nameof(HeadphonesVolume), value, _policy);
            _device.Global.HeadphonesVolume = value;
            VolumeCachePropertyChanged();
        }
    }

    public float MonitorBlend
    {
        get
        {
            var tempValue = (float?)_memoryCache.Get(nameof(MonitorBlend));
            return tempValue ?? _device.Global.MonitorBlend;
        }
        set
        {
            _memoryCache.Set(nameof(MonitorBlend), value, _policy);
            _device.Global.MonitorBlend = value;
            VolumeCachePropertyChanged();
        }
    }
}
