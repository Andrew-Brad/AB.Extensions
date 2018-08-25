namespace AB.Extensions
{
    /// <summary>
    /// Paging response language does not change across application types.
    /// Could possibly be implemented as a struct in the future.
    /// </summary>
    public class PagingResponse
    {
        public uint CurrentPage { get; set; }
        public uint TotalPages { get; set; }

        /// <summary>
        /// The count of results for the given paged response collection.
        /// </summary>
        public uint ResultCount { get; set; }
        /// <summary>
        /// Grand total of records in the underlying collection.
        /// </summary>
        public uint Total { get; set; }

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public PagingResponse() {}

        /// <summary>
        /// Constructor with all parameters.
        /// </summary>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="resultCount">The count of objects being returned in the associated response.</param>
        /// <param name="grandTotal">The grand total of objects in the underlying collection.</param>
        public PagingResponse(uint pageNumber, uint resultCount, uint grandTotal)
        {
            CurrentPage = pageNumber;
            ResultCount = resultCount;
            Total = grandTotal;
        }
    }    
}
