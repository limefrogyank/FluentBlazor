using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class FocusZone : FluentBase, IHasChildren
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            ComponentName = "FocusZone";
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            //if (!FluentComponentAttributes.ContainsKey("componentName"))
            //    FluentComponentAttributes.Add("componentName", "FocusZone");
        }

    }
}
