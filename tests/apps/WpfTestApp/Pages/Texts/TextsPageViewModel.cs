namespace WpfTestApp.Pages.Texts
{
    using System.ComponentModel.Composition;

    [Export(typeof(TabPageBase))]
    public class TextsPageViewModel : TabPageBase
    {
        private TextsPageViewModel()
            : base("Texts") { }
    }
}
