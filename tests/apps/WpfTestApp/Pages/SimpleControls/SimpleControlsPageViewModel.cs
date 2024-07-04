namespace WpfTestApp.Pages.SimpleControls
{
    using System.ComponentModel.Composition;
    using System.Windows;

    [Export(typeof(TabPageBase))]
    public class SimpleControlsPageViewModel : TabPageBase
    {
        private SimpleControlsPageViewModel()
            : base("SimpleControls") { }

        public void DoSomething()
        {
            MessageBox.Show("Done");
        }
    }
}
