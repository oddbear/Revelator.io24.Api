namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize : ExtensionBase
    {
        public string Id { get; set; }

        public Synchronize_Children Children { get; set; }

        public Synchronize_Shared Shared { get; set; }
    }
}
