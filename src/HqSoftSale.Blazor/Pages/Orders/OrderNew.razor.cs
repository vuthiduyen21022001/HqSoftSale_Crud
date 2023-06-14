using Blazorise;

using HqSoftSale.Orders;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Users;
using HqSoftSale.Products;
namespace HqSoftSale.Blazor.Pages.Orders
{
    public partial class OrderNew
    {
        protected Validations CreateValationRef;
        protected CreateUpdateOrderDto NewEntity = new();


        private List<Employee> employeeList;
        public class Employee
        {
            public decimal Salary { get; set; }
            public decimal Tax { get; set; }
        }




        protected override async Task OnInitializedAsync()
        {
            employeeList = new List<Employee>();
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
                    await OrderAppService.CreateAsync(NewEntity);
                    NavigationManager.NavigateTo("orders");

                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void GoToOrderPage()
        {
            NavigationManager.NavigateTo("orders");
        }


        private bool _hideItem = false;

        private void HideFilterBy()
        {
            _hideItem = !_hideItem;


        }
    


    }
}
