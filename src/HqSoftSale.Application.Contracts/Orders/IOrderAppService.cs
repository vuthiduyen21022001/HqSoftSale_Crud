using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HqSoftSale.Orders;

public interface IOrderAppService :
    ICrudAppService< //Defines CRUD methods
        OrderDto, //Used to show books
        Guid, //Primary key of the book entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateOrderDto> //Used to create/update a book
{

}
