namespace WpfTestApp.Pages.Trees
{
    using System.ComponentModel.Composition;

    [Export(typeof(TabPageBase))]
    public class TreesPageViewModel: TabPageBase
    {
        public TreesPageViewModel(): base("Trees")
        {
        }
    }
}