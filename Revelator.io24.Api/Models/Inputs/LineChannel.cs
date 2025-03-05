using Revelator.io24.Api.Attributes;

namespace Revelator.io24.Api.Models.Inputs;

public abstract class LineChannel : InputChannel
{
    protected readonly RawService _rawService;

    protected LineChannel(RawService rawService)
        : base(rawService)
    {
        _rawService = rawService;
    }


    [RouteValue("clip")]
    public bool Clip
    {
        get => GetBoolean();
    }

    /// <summary>
    /// Bypass Dsp:
    /// True: FatChannel off
    /// False: FatChannel on
    /// </summary>
    [RouteValue("bypassDSP")]
    public bool BypassDSP
    {
        get => GetBoolean();
        set => SetBoolean(value);
    }

    [RouteValue("FXA")]
    public int Reverb
    {
        get => GetVolume();
        set => SetVolume(value);
    }
}