using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using AutoMapper.Internal.Mappers;
using Blazorise.DataGrid;
using HqSoftSale.Orders;
using HqSoftSale.Products;
using Volo.Abp.ObjectMapping;

namespace HqSoftSale.Blazor.Pages.Products
{
    public partial class ProductEdit
    {
        protected CreateUpdateProductDto EditingEntity = new();
        protected Validations EditValidationsRef;

        [Parameter]
        public string Id { get; set; }
        public Guid EditingEntityId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetProductAsync();

            await base.OnInitializedAsync();

            EditingEntityId = Guid.Parse(Id);
            var entityDto = await ProductAppService.GetAsync(EditingEntityId);

            EditingEntity = ObjectMapper.Map<ProductDto, CreateUpdateProductDto>(entityDto);

            if (EditValidationsRef != null)
            {
                await EditValidationsRef.ClearAll();
            }
        }

        protected virtual async Task UpdateEntityAsync()
        {
            try
            {
                var validate = true;
                if (EditValidationsRef != null)
                {
                    validate = await EditValidationsRef.ValidateAll();
                }
                if (validate)
                {
                    await ProductAppService.UpdateAsync(EditingEntityId, EditingEntity);

                    NavigationManager.NavigateTo("products");
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        protected virtual async Task DeleteEntityAsync(Guid Id)
        {
            await OrderAppService.DeleteAsync(Id);
            NavigationManager.NavigateTo("products");
        }

        private void GoToOrderPage()
        {
            NavigationManager.NavigateTo("/products");
        }

        private IReadOnlyList<ProductDto> ProductList { get; set; }

        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }

        private async Task SetPermissionsAsync()
        {
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

            ProductList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private bool _hideItem = false;

        private void HideFilterBy()
        {
            _hideItem = !_hideItem;
        }

    }
}
