using Dodo.Logic.Shared;

namespace Dodo.Modules.Search
{
    public class SearchViewModel : BindableBase
    {
        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }
    }
}
