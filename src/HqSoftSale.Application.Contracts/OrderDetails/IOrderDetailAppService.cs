using HqSoftSale.Orders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HqSoftSale.OrderDetails;

public interface IOrderDetailAppService :
ICrudAppService< //Defines CRUD methods
    OrderDetailDto, //Used to show order detail
    Guid, //Primary key of the order detail entity
    PagedAndSortedResultRequestDto, //Used for paging/sorting
    CreateUpdateOrderDetailDto> //Used to create/update a order detail
{
   
}