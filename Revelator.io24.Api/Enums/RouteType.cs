namespace Revelator.io24.Api.Enums
{
    /// <summary>
    /// Some values has inverted On/Off state.
    /// This is becuase some has mute states, and some has assign states.
    /// IsMuted != IsAssigned
    /// IsMuted: Ends with /mute
    /// IsAssigned: Ends contains /assign_aux then a number.
    /// </summary>
    public enum RouteType
    {
        Mute,
        Assign
    }
}
