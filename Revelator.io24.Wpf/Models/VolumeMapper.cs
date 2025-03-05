using Revelator.io24.Api;
using Revelator.io24.Api.Enums;

namespace Revelator.io24.Wpf.Models;

public class VolumeMapper
{
    private readonly RoutingTable _routingTable;

    public float Main_MicL
    {
        get => GetValue(Input.Mic_L, MixOut.Main);
        set => SetValue(Input.Mic_L, MixOut.Main, value);
    }

    public float Main_MicR
    {
        get => GetValue(Input.Mic_R, MixOut.Main);
        set => SetValue(Input.Mic_R, MixOut.Main, value);
    }

    public float Main_Playback
    {
        get => GetValue(Input.Playback, MixOut.Main);
        set => SetValue(Input.Playback, MixOut.Main, value);
    }

    public float Main_VirtualA
    {
        get => GetValue(Input.Virtual_A, MixOut.Main);
        set => SetValue(Input.Virtual_A, MixOut.Main, value);
    }

    public float Main_VirtualB
    {
        get => GetValue(Input.Virtual_B, MixOut.Main);
        set => SetValue(Input.Virtual_B, MixOut.Main, value);
    }

    public float Main_Reverb
    {
        get => GetValue(Input.Reverb, MixOut.Main);
        set => SetValue(Input.Reverb, MixOut.Main, value);
    }

    public float Main_Mix
    {
        get => GetValue(Input.Mix, MixOut.Main);
        set => SetValue(Input.Mix, MixOut.Main, value);
    }

    public float MixA_MicL
    {
        get => GetValue(Input.Mic_L, MixOut.Mix_A);
        set => SetValue(Input.Mic_L, MixOut.Mix_A, value);
    }

    public float MixA_MicR
    {
        get => GetValue(Input.Mic_R, MixOut.Mix_A);
        set => SetValue(Input.Mic_R, MixOut.Mix_A, value);
    }

    public float MixA_Playback
    {
        get => GetValue(Input.Playback, MixOut.Mix_A);
        set => SetValue(Input.Playback, MixOut.Mix_A, value);
    }

    public float MixA_VirtualA
    {
        get => GetValue(Input.Virtual_A, MixOut.Mix_A);
        set => SetValue(Input.Virtual_A, MixOut.Mix_A, value);
    }

    public float MixA_VirtualB
    {
        get => GetValue(Input.Virtual_B, MixOut.Mix_A);
        set => SetValue(Input.Virtual_B, MixOut.Mix_A, value);
    }

    public float MixA_Reverb
    {
        get => GetValue(Input.Reverb, MixOut.Mix_A);
        set => SetValue(Input.Reverb, MixOut.Mix_A, value);
    }

    public float MixA_Mix
    {
        get => GetValue(Input.Mix, MixOut.Mix_A);
        set => SetValue(Input.Mix, MixOut.Mix_A, value);
    }

    public float MixB_MicL
    {
        get => GetValue(Input.Mic_L, MixOut.Mix_B);
        set => SetValue(Input.Mic_L, MixOut.Mix_B, value);
    }

    public float MixB_MicR
    {
        get => GetValue(Input.Mic_R, MixOut.Mix_B);
        set => SetValue(Input.Mic_R, MixOut.Mix_B, value);
    }

    public float MixB_Playback
    {
        get => GetValue(Input.Playback, MixOut.Mix_B);
        set => SetValue(Input.Playback, MixOut.Mix_B, value);
    }

    public float MixB_VirtualA
    {
        get => GetValue(Input.Virtual_A, MixOut.Mix_B);
        set => SetValue(Input.Virtual_A, MixOut.Mix_B, value);
    }

    public float MixB_VirtualB
    {
        get => GetValue(Input.Virtual_B, MixOut.Mix_B);
        set => SetValue(Input.Virtual_B, MixOut.Mix_B, value);
    }

    public float MixB_Reverb
    {
        get => GetValue(Input.Reverb, MixOut.Mix_B);
        set => SetValue(Input.Reverb, MixOut.Mix_B, value);
    }

    public float MixB_Mix
    {
        get => GetValue(Input.Mix, MixOut.Mix_B);
        set => SetValue(Input.Mix, MixOut.Mix_B, value);
    }

    public VolumeMapper(RoutingTable routingTable)
    {
        _routingTable = routingTable;
    }

    private float GetValue(Input input, MixOut mixOut)
        => _routingTable.GetVolume(input, mixOut).Raw;

    private void SetValue(Input input, MixOut mixOut, float value)
        => _routingTable.SetVolume(input, mixOut, new VolumeValue { ValueRaw = value });
}