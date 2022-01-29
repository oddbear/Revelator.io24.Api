namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Line_Channel : ExtensionBase
    {
        public Synchronize_Line_Channel_Values Values { get; set; }

        public Synchronize_Line_Channel_Ranges Ranges { get; set; }

        public Synchronize_Line_Channel_Children Children { get; set; }
    }
}
