namespace WpfTestApp.Pages.Disabled
{
    using System.ComponentModel.Composition;

    [Export(typeof(TabPageBase))]
    public class DisabledPageViewModel : TabPageBase
    {
        private DisabledPageViewModel()
            : base("Disabled")
        {
            IsEnabled = false;
        }
    }
}
