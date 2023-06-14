using HqSoftSale.Products;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Blazorise;
using Blazorise.DataGrid;
using System.Linq;
using System.Net.Sockets;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.BlazoriseUI;

namespace HqSoftSale.Blazor.Pages.Products
{
    public partial class ProductList
    {
        protected CreateUpdateProductDto EditingEntity = new();
        private IReadOnlyList<ProductDto> productsList { get; set; }
        private List<ProductDto> selectedRows = new List<ProductDto>();

        private int PageSize { get; set; } = 1000;
        //private int PageSize { get; } = LimitedResultRequestDto.MaxMaxResultCount;
        private int CurrentPage { get; set; }
        private string CurrentSorting { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetProductAsync();
            await SetBreadcrumbItemAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Sales"]));
            return ValueTask.CompletedTask;
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

            productsList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private string _searchString;
        private DataGrid<ProductDto> dataGrid;
        private List<string> _events = new();

        private Task _quickFilter(string e)
        {
            _searchString = e;
            return dataGrid.Reload();
        }
        private bool OnCustomFilter(ProductDto x)
        {
            // We want to accept empty value as valid or otherwise
            // datagrid will not show anything.
            if (string.IsNullOrEmpty(_searchString))
                return true;

            if (x != null && x.ProductID != null && x.ProductID.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x != null && x.ProductName != null && x.ProductName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x != null && x.Type.ToString() != null && x.Type.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (x != null && x.UnitType.ToString() != null && x.UnitType.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{x.Quanity}".Contains(_searchString))
                return true;
            if ($"{x.Price}".Contains(_searchString))
                return true;
            if ($"{x.ExtenedAmount}".Contains(_searchString))
                return true;
            return false;
        }

        //Xóa nhiều dòng trong Datagrid 
        async Task DeleteSelectedRows()
        {
            foreach (var item in selectedRows)
            {
                await AppService.DeleteAsync(item.Id);
            }
            // Refresh lại danh sách sau khi xóa
            await GetProductAsync();
            selectedRows = new List<ProductDto>();
        }

        private void GoToEditPage(ProductDto product)
        {
            NavigationManager.NavigateTo($"products/edit/{product.Id}");
        }
        private void GoToCreatePage()
        {
            NavigationManager.NavigateTo("products/new");
        }

        private bool _hideItem = false;

        private void HideFilterBy()
        {
            _hideItem = !_hideItem;
        }



    }
}
