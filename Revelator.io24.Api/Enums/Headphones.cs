﻿namespace Revelator.io24.Api.Enums
{
    public enum Headphones : ushort
    {
        //Route: "global/phonesSrc"
        Unknown = ushort.MaxValue,
        Main = 0,       //    0: 0x00, 0x00, 0.0f
        MixA = 16128,   //16128: 0x00, 0x3F, 0.5f
        MixB = 16256    //16256: 0x80, 0x3F, 1.0f
    }
}
