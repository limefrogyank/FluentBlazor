using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class FluentText : FluentBase, IHasTextContent
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Variant { get; set; }

        public ElementReference TextContainingReference { get; set; }

        protected override void OnInitialized()
        {
            ComponentName = "Text";
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            FluentComponentAttributes["variant"] = Variant;

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
