using HqSoftSale.Orders;
using HqSoftSale.Products;
using HqSoftSale.OrderDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace HqSoftSale.OrderDetails;

[RemoteService(IsEnabled = true)]
public class OrderDetailAppService :
    CrudAppService<
OrderDetail, //The Order entity
        OrderDetailDto, //Used to show Orders
        Guid, //Primary key of the Order entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateOrderDetailDto>, //Used to create/update a Order
    IOrderDetailAppService //implement the IOrderAppService
{
    public OrderDetailAppService(
        IRepository<OrderDetail, Guid> repository)
    : base(repository)
    {
    }

    public override async Task<OrderDetailDto> GetAsync(Guid id)
    {
        //Get the IQueryable<Order> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join Orders and authors
        var query = from OrderDetail in queryable
                    where OrderDetail.Id == id
                    select new { OrderDetail };

        //Execute the query and get the Order with author
        var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
        if (queryResult == null)
        {
            throw new EntityNotFoundException(typeof(OrderDetail), id);
        }

        var OrderDto = ObjectMapper.Map<OrderDetail, OrderDetailDto>(queryResult.OrderDetail);
        return OrderDto;
    }

    public override async Task<PagedResultDto<OrderDetailDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        //Get the IQueryable<Order> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join Orders and authors
        var query = from OrderDetail in queryable
                    select new { OrderDetail };

        //Paging
        query = query
            .OrderBy(NormalizeSorting(input.Sorting))
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        //Execute the query and get a list
        var queryResult = await AsyncExecuter.ToListAsync(query);

        //Convert the query result to a list of OrderDto objects
        var OrderDtos = queryResult.Select(x =>
        {
            var OrderDto = ObjectMapper.Map<OrderDetail, OrderDetailDto>(x.OrderDetail);
            return OrderDto;
        }).ToList();

        //Get the total count with another query
        var totalCount = await Repository.GetCountAsync();

        return new PagedResultDto<OrderDetailDto>(
            totalCount,
            OrderDtos
        );
    }

    private static string NormalizeSorting(string sorting)
    {
        if (sorting.IsNullOrEmpty())
        {
            return $"OrderDetail.{nameof(OrderDetail.Id)}";
        }

        return $"OrderDetail.{sorting}";
    }
}