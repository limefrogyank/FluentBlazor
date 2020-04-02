using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class DetailsList: FluentBase
    {
      
        [Parameter]
        public object Items { get; set; }

        [Parameter]
        public object Columns { get; set; }

        protected override void OnInitialized()
        {
            ComponentName = "DetailsList";
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (!FluentComponentAttributes.ContainsKey("componentName"))
                FluentComponentAttributes.Add("componentName", ComponentName);

            FluentComponentAttributes["items"] = Items;
            FluentComponentAttributes["columns"] = Columns;

        }
    }
}
