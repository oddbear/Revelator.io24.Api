namespace Revelator.io24.Api.Models.Json
{
    public class Synchronize_Children : ExtensionBase
    {
        public ValuesObject<Synchronize_Diagnostics> Diagnostics { get; set; }
        public Synchronize_Global Global { get; set; }
        public ChildrenObject<Synchronize_Line> Line { get; set; }
        public ChildrenObject<Synchronize_Return> Return { get; set; }
        public ChildrenObject<Synchronize_FxReturn> Fxreturn { get; set; }
        public ChildrenObject<Synchronize_Aux> Aux { get; set; }
        public ChildrenObject<Synchronize_Main> Main { get; set; }
        public ChildrenObject<Synchronize_Fx> Fx { get; set; }
    }
}
