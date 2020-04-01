using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class Checkbox : FluentBase
    {
        [Parameter]
        public bool Checked { 
            get; 
            set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        protected override void OnInitialized()
        {
            ComponentName = "Checkbox";
            base.OnInitialized();
        }
            
        protected override async Task OnParametersSetAsync()
        {
            eventContainer.Clear();


            if (!FluentComponentAttributes.ContainsKey("componentName"))
                FluentComponentAttributes.Add("componentName", "Checkbox");

            FluentComponentAttributes["checked"] = Checked;
            FluentComponentAttributes["label"] = Label;

            if (OnChange.HasDelegate)
            {
                eventContainer.Add(new TranslatedEvent("onChange", OnChange));
            }


            await base.OnParametersSetAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            base.OnAfterRender(firstRender);
        }
    }
}
