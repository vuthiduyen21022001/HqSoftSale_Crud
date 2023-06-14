using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace HqSoftSale.Products
{
    public class GetProductListDto : PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
    }
}
