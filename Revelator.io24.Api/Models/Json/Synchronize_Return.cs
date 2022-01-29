namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Return : ExtensionBase
    {
        public ValuesObject<Synchronize_Channel_Extended> Ch1 { get; set; }
        public ValuesObject<Synchronize_Channel_Extended> Ch2 { get; set; }
        public ValuesObject<Synchronize_Channel_Extended> Ch3 { get; set; }
    }
}
