namespace WpfTestApp
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;

    [Export]
    public class ShellViewModel : Conductor<TabPageBase>.Collection.OneActive
    {
        private string _theMessage = "Hello World";
        private string _title = "WPF Test Application";

        [ImportingConstructor]
        public ShellViewModel([ImportMany] TabPageBase[] pages)
        {
            Items.AddRange(pages.OrderBy(v => v.DisplayName));
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                {
                    return;
                }

                _title = value;
                NotifyOfPropertyChange();
            }
        }

        public string TheMessage
        {
            get => _theMessage;
            set
            {
                if (value == _theMessage)
                {
                    return;
                }

                _theMessage = value;
                NotifyOfPropertyChange();
            }
        }

        public async void Exit()
        {
            await TryCloseAsync();
        }

        public void New()
        {
            MessageBox.Show("New activated");
        }

        public void Fourth()
        {
            MessageBox.Show("Fourth activated");
        }

        public void Eighth()
        {
            MessageBox.Show("Eighth activated");
        }

        public void ClickMe()
        {
            MessageBox.Show(TheMessage, "A Message");
        }

        public async void ShowSettings()
        {
            await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(
                "sorry this is not implemented at the moment",
                "This should be the settings"
            );
        }

        public async void DeployCupcakes()
        {
            await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(
                "Hope it tastes good",
                "CupCakes deployed",
                MessageDialogStyle.AffirmativeAndNegative
            );
        }
    }
}
