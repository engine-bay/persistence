namespace EngineBay.Persistence
{
    using System.Linq.Expressions;
    using EngineBay.Core;

    public class FilteredPaginationParameters<TBaseModel> : SearchParameters
        where TBaseModel : BaseModel
    {
        public FilteredPaginationParameters(PaginationParameters paginationParameters, Expression<Func<TBaseModel, bool>>? filterPredicate)
            : base(string.Empty, paginationParameters)
        {
            this.FilterPredicate = filterPredicate;
        }

        public FilteredPaginationParameters(PaginationParameters paginationParameters, FilterParameters? filterParameters)
            : base(string.Empty, paginationParameters)
        {
            this.BuildFilterPredicate(filterParameters);
        }

        public FilteredPaginationParameters(PaginationParameters paginationParameters)
            : base(string.Empty, paginationParameters)
        {
        }

        public FilteredPaginationParameters()
        {
        }

        public FilteredPaginationParameters(SearchParameters searchParameters, Expression<Func<TBaseModel, bool>>? filterPredicate)
            : base(searchParameters)
        {
            this.FilterPredicate = filterPredicate;
        }

        public FilteredPaginationParameters(SearchParameters searchParameters, FilterParameters? filterParameters)
            : base(searchParameters)
        {
            this.BuildFilterPredicate(filterParameters);
        }

        public FilteredPaginationParameters(SearchParameters searchParameters)
            : base(searchParameters)
        {
        }

        public Expression<Func<TBaseModel, bool>>? FilterPredicate { get; set; }

        private void BuildFilterPredicate(FilterParameters? filterParameters)
        {
            ArgumentNullException.ThrowIfNull(filterParameters);

            if (filterParameters.Ids is not null && filterParameters.Ids.Any())
            {
                this.FilterPredicate = x => filterParameters.Ids.Contains(x.Id);
            }
        }
    }
}