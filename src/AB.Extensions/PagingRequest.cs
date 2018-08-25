namespace AB.Extensions
{
    /// <summary>
    /// Paging request language does not change across application types.  Default values are framework provided - no opinions built in.
    /// Could possibly be implemented as a struct in the future.
    /// </summary>
    public class PagingRequest
    {
        public uint PageNumber { get; set; }
        public uint ResultsPerPage { get; set; }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public PagingRequest() {}

        public PagingRequest(uint pageNumber, uint resultPerPage)
        {
            PageNumber = pageNumber;
            ResultsPerPage = resultPerPage;
        }
    }
}
