using SharpDeck;
using SharpDeck.Enums;

namespace Revelator.io24.StreamDeck.Actions
{
    public class ActionBase : StreamDeckAction
    {
        protected async Task SetImageStates(string on, string off)
        {
            try
            {
                var onImageBytes = File.ReadAllBytes($"./Images/Plugin/{on}.png");
                var onBase64 = Convert.ToBase64String(onImageBytes);
                await SetImageAsync("data:image/png;base64," + onBase64, TargetType.Both, 0);

                var offImageBytes = File.ReadAllBytes($"./Images/Plugin/{off}.png");
                var offBase64 = Convert.ToBase64String(offImageBytes);
                await SetImageAsync("data:image/png;base64," + offBase64, TargetType.Both, 1);
            }
            catch
            {
                await SetImageAsync(null);
            }
        }
    }
}
