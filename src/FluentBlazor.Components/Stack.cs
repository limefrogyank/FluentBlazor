﻿using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public class Stack : FluentBase, IHasChildren
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (!FluentComponentAttributes.ContainsKey("componentName"))
                FluentComponentAttributes.Add("componentName", "Stack");
          
        }
    }
}
