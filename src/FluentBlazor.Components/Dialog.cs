using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class Dialog : FluentBase, IHasChildren
    {

        [Parameter]
        public EventCallback OnDismiss { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public object DialogContentProps { get; set; }

        [Parameter]
        public bool Hidden { get; set; }

        protected override void OnInitialized()
        {
            ComponentName = "Dialog";
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            FluentComponentAttributes["dialogContentProps"] = DialogContentProps;
            FluentComponentAttributes["hidden"] = Hidden;

            if (OnDismiss.HasDelegate)
            {
                eventContainer.Add(new TranslatedEvent("onDismiss", OnDismiss));
            }

        }

    }
}
