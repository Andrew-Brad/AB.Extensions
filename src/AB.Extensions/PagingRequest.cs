namespace AB.Extensions
{
    /// <summary>
    /// Paging request language does not change across application types.  Default values are framework provided - no opinions built in.
    /// Could possibly be implemented as a struct in the future.
    /// </summary>
    public class PagingRequest
    {
        /// <summary>
        /// The 1-based index of the requested page.
        /// </summary>
        public uint PageNumber { get; set; }

        /// <summary>
        /// The maximum number of results to return per page.
        /// </summary>
        public uint ResultsPerPage { get; set; }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public PagingRequest() { }

        /// <summary>
        /// Constructs a paging request for the given page and page size.
        /// </summary>
        /// <param name="pageNumber">The 1-based page index.</param>
        /// <param name="resultPerPage">The maximum number of results per page.</param>
        public PagingRequest(uint pageNumber, uint resultPerPage)
        {
            PageNumber = pageNumber;
            ResultsPerPage = resultPerPage;
        }
    }
}
