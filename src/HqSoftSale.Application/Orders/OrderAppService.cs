using HqSoftSale.OrderDetails;
using HqSoftSale.Orders;
using HqSoftSale.Products;
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
using Volo.Abp.Uow;

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
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<OrderDetail> _orderDetailRepository;

    public OrderAppService(
        IRepository<Order, Guid> repository,
         IRepository<Order> orderRepository,
        IRepository<Product> productRepository,
          IUnitOfWorkManager unitOfWorkManager,
        IRepository<OrderDetail> orderDetailRepository)
        : base(repository)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderDetailRepository = orderDetailRepository;
    }
    public async Task<List<ProductDto>> GetProductsByOrderDetail(string orderId)
    {
        var orderDetails = await _orderDetailRepository.GetListAsync(od => od.OrderID == orderId);

        var products = new List<ProductDto>();


        // Vòng lặp để truy vấn danh sách sản phẩm tương ứng với mỗi chi tiết đơn hàng trong danh sách
        foreach (var orderDetail in orderDetails)
        {
            var product = await _productRepository.FirstOrDefaultAsync(p => p.ProductID == orderDetail.ProductID);

            if (product != null)
            {
                products.Add(ObjectMapper.Map<Product, ProductDto>(product));
            }
        }
        return products;
    }

    // Tạo ra một mã đơn hàng mới.
    public async Task<string?> GenerateOrderIdAsync()
    {
        using (var unitOfWork = _unitOfWorkManager.Begin())
        {
            var lastOrder = _orderRepository
                .GetListAsync()
                .Result
                .OrderByDescending(x => x.OrderNumber)
                .FirstOrDefault();
            var newOrderId = "00000X";
            if (lastOrder != null)
            {
                var lastOrderId = lastOrder.OrderNumber;
                var lastOrderNumber = int.Parse(lastOrderId.Substring(2));
                newOrderId = "SO" + (lastOrderNumber + 1).ToString("D8");
            }
            // `Any`: Kiểm tra mã đơn hàng mới có bị trùng với mã đơn hàng nào trong CSDL ko 
            while (_orderRepository.GetListAsync().Result.Any(x => x.OrderNumber == newOrderId))
            {
                newOrderId = "SO" + (int.Parse(newOrderId.Substring(2)) + 1).ToString("D8");
            }
            await unitOfWork.SaveChangesAsync();
            return newOrderId;
        }
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
    public async Task<Guid> CreateOrderAndOrderDetails(CreateUpdateOrderDto orderDto, CreateUpdateOrderDetailDto orderDetailDto)
    {
        var order = new Order
        {
            OrderNumber = orderDto.OrderNumber,
            OrderDate = orderDto.OrderDate,
            Customer = orderDto.Customer,
            //Quanity = orderDto.Quanity,
            //ExtenedAmount = orderDto.ExtenedAmount,
            OrderStatus = orderDto.OrderStatus,
        };
        var orderDetail = new OrderDetail
        {
            OrderID = order.OrderNumber,
            ProductID = orderDetailDto.ProductID,
            ProductName = orderDetailDto.ProductName,
            UnitType = orderDetailDto.UnitType,
            Type = orderDetailDto.Type,
            Quantity = orderDetailDto.Quantity,
            Price = orderDetailDto.Price,
            ExAmount = orderDetailDto.ExAmount
        };
        await _orderRepository.InsertAsync(order);
        await _orderDetailRepository.InsertAsync(orderDetail);
        return order.Id;
    }

}

