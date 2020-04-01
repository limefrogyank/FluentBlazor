using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class PrimaryButton : FluentBase, IHasChildren
    {
        [Parameter]
        public EventCallback OnClick { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {            
            base.OnInitialized();
        }


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (!FluentComponentAttributes.ContainsKey("componentName"))
                FluentComponentAttributes.Add("componentName", "PrimaryButton");
            

            //eventContainer.Clear();  //memory leaks maybe, need to dispose dotnetobject refs
            
            if (OnClick.HasDelegate)
            {
                eventContainer.Add(new TranslatedEvent("onClick", OnClick));
            }
            
        }

    }
}
