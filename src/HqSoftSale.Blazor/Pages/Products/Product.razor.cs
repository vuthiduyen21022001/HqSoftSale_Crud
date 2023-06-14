using Blazorise;
using HqSoftSale.Products;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Users;

namespace HqSoftSale.Blazor.Pages.Products
{
    public partial class Product
    {
        protected Validations CreateValationRef;
        protected CreateUpdateProductDto NewEntity = new();


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (CreateValationRef != null)
            {
                await CreateValationRef.ClearAll();
            }
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
                    await ProductAppService.CreateAsync(NewEntity);
                    NavigationManager.NavigateTo("products");

                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void GoToProductPage()
        {
            NavigationManager.NavigateTo("products");
        }


        private bool _hideItem = false;

        private void HideFilterBy()
        {
            _hideItem = !_hideItem;
        }
    }
}


