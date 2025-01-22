using System.Runtime.CompilerServices;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Revelator.io24.StreamDeck.Actions.Encoders;

/// <summary>
/// If we turn a dial, we want to store a cached value while turning.
/// If we stop turning, we want to set the real value to the display.
/// </summary>
internal class CacheTimer : IDisposable
{
    private readonly Dictionary<string, float> _cache = new();

    public float GetValueOr(float value, [CallerMemberName] string propertyName = "")
    {
        // Cache does not know about this value:
        if (_cache.ContainsKey(propertyName) is false)
            return value;

        // No value is cached, return the real value:
        if (_timer.Enabled is false)
            return value;

        // Return the cached value:
        return _cache[propertyName];
    }

    public void SetValue(float value, [CallerMemberName] string propertyName = "")
    {
        // Reset the timer:
        _timer.Stop();
        _timer.Start();
        _cache[propertyName] = value;

        // Call this when set new value, or cache time has elapsed:
        _elapsedDelegate();
    }

    public bool Active => _timer.Enabled;

    private readonly Timer _timer;
    private readonly Action _elapsedDelegate;

    public CacheTimer(Action elapsedDelegate)
    {
        _elapsedDelegate = elapsedDelegate;

        _timer = new Timer(TimeSpan.FromSeconds(1))
        {
            AutoReset = false
        };
        _timer.Elapsed += Elapsed;
    }

    private void Elapsed(object? sender, ElapsedEventArgs e)
    {
        _elapsedDelegate();
    }

    public void Dispose()
    {
        _timer.Elapsed -= Elapsed;
        _timer.Dispose();
    }
}