namespace EngineBay.Persistence.Tests
{
    using System.Linq.Expressions;
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using Xunit;

    public class FilteredPaginationParametersTests
    {
        [Fact]
        public void ConstructorWithPaginationAndFilterPredicateSetsProperties()
        {
            Guid id = Guid.NewGuid();
            PaginationParameters paginationParameters = new PaginationParameters();
            Expression<Func<MockModel, bool>> filterPredicate = x => x.Id == id;

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(paginationParameters, filterPredicate);

            Assert.NotNull(sut);
            Assert.NotNull(sut.FilterPredicate);
        }

        [Fact]
        public async void ConstructorWithPaginationAndFilterParametersSetsProperties()
        {
            PaginationParameters paginationParameters = new PaginationParameters();
            Guid[] idArray =
           [
                Guid.NewGuid(),
                 Guid.NewGuid(),
            ];
            Dictionary<string, StringValues> queryString = new Dictionary<string, StringValues>
            {
                { "Ids", new StringValues(idArray.Select(id => id.ToString()).ToArray()) },
            };
            QueryCollection queryCollection = new QueryCollection(queryString);
            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Query = queryCollection;

            FilterParameters filterParameters = await FilterParameters.BindAsync(httpContext);

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(paginationParameters, filterParameters);

            Assert.NotNull(sut);
            Assert.NotNull(sut.FilterPredicate);
        }

        [Fact]
        public void ConstructorWithPaginationAndFilterParametersSetsNoProperties()
        {
            PaginationParameters paginationParameters = new PaginationParameters();
            FilterParameters filterParameters = new FilterParameters();

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(paginationParameters, filterParameters);

            Assert.NotNull(sut);
            Assert.Null(sut.FilterPredicate);
        }

        [Fact]
        public void ConstructorWithPaginationParameterdNoProperties()
        {
            PaginationParameters paginationParameters = new PaginationParameters();

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(paginationParameters);

            Assert.NotNull(sut);
            Assert.Null(sut.FilterPredicate);
        }

        [Fact]
        public void ConstructorSetsNoProperties()
        {
            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>();

            Assert.NotNull(sut);
            Assert.Null(sut.FilterPredicate);
        }

        [Fact]
        public void ConstructorWithSearchParametersAndFilterPredicateSetsProperties()
        {
            Guid id = Guid.NewGuid();
            SearchParameters searchParameters = new SearchParameters();
            Expression<Func<MockModel, bool>> filterPredicate = x => x.Id == id;

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(searchParameters, filterPredicate);

            Assert.NotNull(sut);
            Assert.NotNull(sut.FilterPredicate);
        }

        [Fact]
        public async void ConstructorWithSearchParametersAndFilterParametersSetsProperties()
        {
            SearchParameters searchParameters = new SearchParameters();
            Guid[] idArray =
           [
                Guid.NewGuid(),
                 Guid.NewGuid(),
            ];
            Dictionary<string, StringValues> queryString = new Dictionary<string, StringValues>
            {
                { "Ids", new StringValues(idArray.Select(id => id.ToString()).ToArray()) },
            };
            QueryCollection queryCollection = new QueryCollection(queryString);
            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Query = queryCollection;

            FilterParameters filterParameters = await FilterParameters.BindAsync(httpContext);

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(searchParameters, filterParameters);

            Assert.NotNull(sut);
            Assert.NotNull(sut.FilterPredicate);
        }

        [Fact]
        public void ConstructorWithSearchParametersAndFilterParametersSetsNoProperties()
        {
            SearchParameters searchParameters = new SearchParameters();
            FilterParameters filterParameters = new FilterParameters();

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(searchParameters, filterParameters);

            Assert.NotNull(sut);
            Assert.Null(sut.FilterPredicate);
        }

        [Fact]
        public void ConstructorWithSearchParametersParameterdNoProperties()
        {
            SearchParameters searchParameters = new SearchParameters();

            FilteredPaginationParameters<MockModel> sut = new FilteredPaginationParameters<MockModel>(searchParameters);

            Assert.NotNull(sut);
            Assert.Null(sut.FilterPredicate);
        }
    }
}