using Dodo.Modules.Dashboard;
using Dodo.Modules.Search;
using Dodo.Modules.Share;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dodo
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // TODO: Load state from previously suspended application
            }

            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(DashboardView));
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
        }

        void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //TODO: Save application state and stop any background activity
        }

        protected override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            base.OnSearchActivated(args);

            var view = new SearchView();
            var viewModel = new SearchViewModel();
            viewModel.SearchText = args.QueryText;
            view.DataContext = viewModel;


            Window.Current.Content = view;
            Window.Current.Activate();
        }

        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            base.OnShareTargetActivated(args);

            var view = new ShareView();
            var viewModel = new ShareViewModel();
            viewModel.SetShareTarget(args);
            view.DataContext = viewModel;
            Window.Current.Content = view;
            Window.Current.Activate();
        }
    }
}
