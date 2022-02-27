using System;

namespace Revelator.io24.Api.Extensions
{
    internal static class ArrayExtensions
    {
        public static T[] Range<T>(this T[] data, int startIndex)
        {
            return Range(data, startIndex, data.Length);
        }

        public static T[] Range<T>(this T[] data, int startIndex, int toIndex)
        {
            if (startIndex < 0)
                startIndex = data.Length + startIndex;

            if (toIndex < 0)
                toIndex = data.Length + toIndex;

            var slice = new T[toIndex - startIndex];
            Array.Copy(data, startIndex, slice, 0, slice.Length);
            return slice;
        }
    }
}
