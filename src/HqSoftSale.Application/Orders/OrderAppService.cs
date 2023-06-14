using HqSoftSale.Orders;
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

namespace HqSoftSale.Orders;

[RemoteService(IsEnabled = true)]
public class OrderAppService :
    CrudAppService<
        Order, //The Order entity
        OrderDto, //Used to show Orders
        Guid, //Primary key of the Order entity
        PagedAndSortedResultRequestDto, //Used for paging/sorting
        CreateUpdateOrderDto>, //Used to create/update a Order
    IOrderAppService //implement the IOrderAppService
{
    public OrderAppService(
        IRepository<Order, Guid> repository)
        : base(repository)
    {
    }

    public override async Task<OrderDto> GetAsync(Guid id)
    {
        //Get the IQueryable<Order> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join Orders and authors
        var query = from Order in queryable
                    where Order.Id == id
                    select new { Order };

        //Execute the query and get the Order with author
        var queryResult = await AsyncExecuter.FirstOrDefaultAsync(query);
        if (queryResult == null)
        {
            throw new EntityNotFoundException(typeof(Order), id);
        }

        var OrderDto = ObjectMapper.Map<Order, OrderDto>(queryResult.Order);
        return OrderDto;
    }

    public override async Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        //Get the IQueryable<Order> from the repository
        var queryable = await Repository.GetQueryableAsync();

        //Prepare a query to join Orders and authors
        var query = from Order in queryable
                    select new { Order };

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
            var OrderDto = ObjectMapper.Map<Order, OrderDto>(x.Order);
            return OrderDto;
        }).ToList();

        //Get the total count with another query
        var totalCount = await Repository.GetCountAsync();

        return new PagedResultDto<OrderDto>(
            totalCount,
            OrderDtos
        );
    }

    private static string NormalizeSorting(string sorting)
    {
        if (sorting.IsNullOrEmpty())
        {
            return $"Order.{nameof(Order.OrderNumber)}";
        }

        return $"Order.{sorting}";
    }
}
