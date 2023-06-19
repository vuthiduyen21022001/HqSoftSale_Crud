using AutoMapper;
using HqSoftSale.OrderDetails;
using HqSoftSale.Orders;
using HqSoftSale.Products;

namespace HqSoftSale;

public class HqSoftSaleApplicationAutoMapperProfile : Profile
{
    public HqSoftSaleApplicationAutoMapperProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<CreateUpdateOrderDto, Order>();

        CreateMap<Product, ProductDto>();
        CreateMap<CreateUpdateProductDto, Product>();

        CreateMap<OrderDetail, OrderDetailDto>();
        CreateMap<CreateUpdateOrderDetailDto, OrderDetail>();
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
    }
}
