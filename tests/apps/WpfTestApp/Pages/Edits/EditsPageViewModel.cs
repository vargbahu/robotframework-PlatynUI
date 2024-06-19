namespace WpfTestApp.Pages.Edits
{
    using System.ComponentModel.Composition;

    [Export(typeof(TabPageBase))]
    public class EditsPageViewModel: TabPageBase
    {
        private EditsPageViewModel()
            : base("Edits")
        {
        }
    }
}