using System.Collections.Generic;

namespace TraineeManagement.Api.DTOs
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<T> Data { get; set; }

        public PagedResponse(int pageNumber, int pageSize, int totalRecords, List<T> data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            Data = data;
        }
    }
}