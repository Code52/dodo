using Dodo.Logic.Services;
using Dodo.Logic.Shared;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Dodo
{
    public sealed partial class Dashboard
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: wireup IoC bits and do this properly
            var viewModel = new DashboardViewModel(new TwitterService(), Window.Current.Dispatcher);
            viewModel.Start();
            DataContext = viewModel;
        }

        private void CommandClicked(object sender, ItemClickEventArgs e)
        {
            // TODO: get a proper framework to handle this behaviour
            var task = e.ClickedItem as UserTask;

            var command = task.Command;
            if (command == null)
                return;

            if (task.Command.CanExecute(null))
                task.Command.Execute(null);
        }
    }
}
