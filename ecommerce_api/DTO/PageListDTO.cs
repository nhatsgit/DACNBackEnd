using X.PagedList;

namespace ecommerce_api.DTO
{
    public class PagedListDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public int PageCount { get; set; }

        public PagedListDTO(IPagedList<T> pagedList)
        {
            Items = pagedList;
            PageNumber = pagedList.PageNumber;
            PageSize = pagedList.PageSize;
            TotalItemCount = pagedList.TotalItemCount;
            PageCount = pagedList.PageCount;
        }
    }
}
