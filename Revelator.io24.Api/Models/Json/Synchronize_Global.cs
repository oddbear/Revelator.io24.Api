namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Global : ExtensionBase
    {
        public Synchronize_Global_Values Values { get; set; }

        public Synchronize_Global_Strings Strings { get; set; }

        public Synchronize_Global_Ranges Ranges { get; set; }

        public Synchronize_Global_States States { get; set; }
    }
}
