using System.ComponentModel;

namespace Revelator.io24.Api.Enums
{
    //TODO: This can be changed to Output.
    public enum Headphones : ushort
    {
        //Route: "global/phonesSrc"
        Unknown = ushort.MaxValue,

        [Description("Main")]
        Main = 0,       //    0: 0x00, 0x00, 0x00, 0x00, 0.0f

        [Description("Stream Mix A")]
        MixA = 16128,   //16128: 0x00, 0x00, 0x00, 0x3F, 0.5f

        [Description("Stream Mix B")]
        MixB = 16256    //16256: 0x00, 0x00, 0x80, 0x3F, 1.0f
    }
}
