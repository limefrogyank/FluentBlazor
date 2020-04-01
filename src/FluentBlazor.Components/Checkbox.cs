using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class Checkbox : FluentBase
    {
        [Parameter]
        public EventCallback OnChange { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (!FluentComponentAttributes.ContainsKey("componentName"))
                FluentComponentAttributes.Add("componentName", "Checkbox");
                                    
            if (OnChange.HasDelegate)
            {
                eventContainer.Add(new TranslatedEvent("onChange", OnChange));
            }

        }
    }
}
