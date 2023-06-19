using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace HqSoftSale.OrderDetails
{
    public class GetOrderDetailListDto : PagedAndSortedResultRequestDto
    {
        public string? OrderID { get; set; }
    }
}
