using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluentBlazor.Components
{
    internal interface IHasTextContent
    {
        RenderFragment ChildContent { get; set; }

        ElementReference TextContainingReference { get; set; }
    }
}
