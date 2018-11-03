namespace AB.Extensions
{
    /// <summary>
    /// Paging response language does not change across application types.
    /// Could possibly be implemented as a struct in the future.
    /// </summary>
    public class PagingResponse
    {
        /// <summary>
        /// You are on this page, with respect to the total possible results.
        /// </summary>
        public uint CurrentPage { get; set; }

        /// <summary>
        /// There are this many pages of data available.
        /// </summary>
        public uint TotalPages { get; set; }

        /// <summary>
        /// The count of results for the given page in the response collection.
        /// </summary>
        public uint ResultCount { get; set; }

        /// <summary>
        /// Grand total of records in the underlying collection.
        /// </summary>
        public uint TotalCount { get; set; }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public PagingResponse() {}

        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="totalPages"></param>
        /// <param name="resultCount"></param>
        /// <param name="totalCount"></param>
        public PagingResponse(uint currentPage, uint totalPages, uint resultCount, uint totalCount)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            ResultCount = resultCount;
            TotalCount = totalCount;
        }
    }    
}
