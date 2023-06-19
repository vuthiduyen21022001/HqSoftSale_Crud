﻿using Blazorise;

using HqSoftSale.Orders;
using HqSoftSale.OrderDetails;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Users;
using HqSoftSale.Products;
using System.Linq;

namespace HqSoftSale.Blazor.Pages.Orders
{
    public partial class OrderNew
    {
        private IReadOnlyList<OrderDto> orders { get; set; }
        private IReadOnlyList<OrderDetailDto> orderDetails { get; set; }
        private IReadOnlyList<ProductDto> products { get; set; }

        protected Validations CreateValationRef;
        protected CreateUpdateOrderDto NewEntity = new();
        protected CreateUpdateOrderDetailDto NewDetailEntity = new();
        private int PageSize { get; set; } = 1000;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (CreateValationRef != null)
            {
                await CreateValationRef.ClearAll();
            }

            await CalculatePrice();
            await GetProductAsync();
        }

        protected virtual async Task CreateEntityAsync()
        {
            try
            {
                var validate = true;
                if (CreateValationRef != null)
                {
                    validate = await CreateValationRef.ValidateAll();
                }
                if (validate)
                {
                    OrderAppService.CreateAsync(NewEntity);
                    OrderDetailAppService.CreateAsync(NewDetailEntity);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task GetProductAsync()
        {
            var result = await ProductAppService.GetListAsync(
                new GetProductListDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = CurrentPage * PageSize,
                    Sorting = CurrentSorting
                }
            );

            products = result.Items;
            TotalCount = (int)result.TotalCount;
        }



        //protected virtual async Task CreateProductEntityAsync()
        //{
        //    try
        //    {
        //        var validate = true;
        //        if (CreateValationRef != null)
        //        {
        //            validate = await CreateValationRef.ValidateAll();
        //        }
        //        if (validate)
        //        {
        //            await ProductAppService.CreateAsync(NewEntityProduct);
        //            //NavigationManager.NavigateTo("products");
        //            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        //        }
        //    }
        //    catch (Exception ex)
        //    catch (Exception ex)
        //    {
        //        await HandleErrorAsync(ex);
        //    }
        //}

        private void UpdateTotal(int quantity)
        {
            NewDetailEntity.Quantity = quantity;
            NewDetailEntity.ExAmount = NewDetailEntity.Quantity * NewDetailEntity.Price;
        }

        protected virtual async Task CalculatePrice()
        {
            if (!string.IsNullOrEmpty(NewDetailEntity.ProductID))
            {
                var product = products.FirstOrDefault(p => p.ProductID == NewDetailEntity.ProductID);
                if (product != null)
                {
                    NewDetailEntity.Price = NewDetailEntity.Quantity * product.Price;
                }
            }
        }

    }
}
