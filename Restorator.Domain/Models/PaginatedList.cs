namespace Restorator.Domain.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; }
        public int TotalItems { get; }
        public int ItemsPerPage { get; }
        public bool HasNextPage => (TotalItems - ItemsPerPage * PageIndex) > 0;
        public bool HasPreviousPage => PageIndex > 1;

        public PaginatedList() { } //for JSON
        public PaginatedList(int index, int totalItems, int itemsPerPage, IEnumerable<T> items)
        {
            PageIndex = index;
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            AddRange(items);
        }

        public static PaginatedList<T> Empty() => new(0, 0, 0, []);
    }
}