using System;
using System.Collections.Specialized;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Dodo.Modules.Dashboard
{
    public sealed partial class DashboardView
    {
        DashboardViewModel _viewModel;
        Storyboard _storyboard;
        bool _isLoading = true;
        CoreDispatcher _dispatcher;

        public DashboardView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _dispatcher = Window.Current.Dispatcher;
            _storyboard = Resources["RotatingSquare"] as Storyboard;
            _storyboard.Begin();

            _viewModel = e.Parameter as DashboardViewModel;
            _viewModel.Tweets.CollectionChanged += TweetsCollectionChanged;
            _viewModel.Start();

            DataContext = _viewModel;
        }

        void TweetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ExecuteInBackground(() =>
                {
                    _isLoading = true;
                    rectangle.Visibility = Visibility.Visible;
                });
            }

            if (_isLoading && e.Action == NotifyCollectionChangedAction.Add)
            {
                ExecuteInBackground(() =>
                {
                    _isLoading = false;
                    rectangle.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void ExecuteInBackground(Action action)
        {
            _dispatcher.InvokeAsync(CoreDispatcherPriority.Low, (s, e) => action(), this, null);
        }
    }
}