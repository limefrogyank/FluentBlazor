using Microsoft.AspNetCore.Components;

namespace FluentBlazor.Components
{
    internal interface IHasChildren
    {
        RenderFragment ChildContent { get; set; }
    }
}