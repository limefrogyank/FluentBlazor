using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FluentBlazor.Components
{
    public class FluentBase : ComponentBase, IDisposable
    {
        protected string id = "G" + Guid.NewGuid().ToString();

        [CascadingParameter(Name = "FluentParent")]
        private FluentBase FluentParent { get; set; }

        [Inject] 
        protected IJSRuntime JSRuntime { get; set; }

        public Dictionary<string, object> FluentComponentAttributes { get; set; } = new Dictionary<string, object>();

        [Parameter]
        public string ComponentName { get; set; }

        protected List<TranslatedEvent> eventContainer = new List<TranslatedEvent>();

        protected List<ReactChild> reactChildren = new List<ReactChild>();
        public ReactChild SelfChild { get; set; }

        private bool needsRestoreInnerChildren = false;

        private bool shouldRerender = false;



        protected override async Task OnParametersSetAsync()
        {
            reactChildren.Clear();

            if (!FluentComponentAttributes.ContainsKey("componentName"))
                FluentComponentAttributes.Add("componentName", ComponentName);

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (SelfChild != null)
            {
                SelfChild.parameters = FluentComponentAttributes;
                SelfChild.serializedEvents = GetSerializedEvents();
            }

            if (FluentParent == null)
            {
                //Process IHasTextContent for current component
                if (this is IHasTextContent)
                {
                    var text = await JSRuntime.InvokeAsync<string>("getTextContent", (this as IHasTextContent).TextContainingReference);
                    FluentComponentAttributes["children"] = text;
                }
                //Add text content to any reactChildren that implement IHasTextContent
                await ProcessTextContentAsync(reactChildren);

                if (firstRender)
                {
                    needsRestoreInnerChildren = await JSRuntime.InvokeAsync<bool>("createComponent", id, FluentComponentAttributes, GetSerializedEvents(), reactChildren);
                }
                else
                {
                    needsRestoreInnerChildren = await JSRuntime.InvokeAsync<bool>("updateComponent", id, FluentComponentAttributes, GetSerializedEvents(), reactChildren);
                }

            }
            await base.OnAfterRenderAsync(firstRender);
        }


        private IEnumerable<SerializedEvent> GetSerializedEvents()
        {
            return eventContainer.Select(x => new SerializedEvent{ eventName = x.EventName, dotNetRef = x.GetDotNetObjectReference() });
        }

        public ReactChild CreateReactChildComponent(FluentBase self, Dictionary<string, object> childParameters, IEnumerable<SerializedEvent> childEvents, ReactChild parent = null)
        {
            ReactChild child = null;
            if (parent == null)
            {
                child = new ReactChild(self) { componentName = (string)childParameters["componentName"], parameters = childParameters, serializedEvents = childEvents };
                reactChildren.Add(child);
            }
            else
            {
                child = new ReactChild(self) { componentName = (string)childParameters["componentName"], parameters = childParameters, serializedEvents = childEvents };
                parent.children.Add(child);
            }
            return child;
        }

        private async Task ProcessTextContentAsync(IEnumerable<ReactChild> children)
        {
            foreach (var child in children)
            {
                if (child.Self is IHasTextContent)
                {
                    var text = await JSRuntime.InvokeAsync<string>("getTextContent", (child.Self as IHasTextContent).TextContainingReference);
                    child.parameters["children"] = text;
                }
                await ProcessTextContentAsync(child.children);
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            builder.OpenComponent<CascadingValue<FluentBase>>(0);
            builder.AddAttribute(1, "Name", "FluentParent");
            builder.AddAttribute(2, "Value", this);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)(builder2 =>
            {
                if (FluentParent == null)
                {
                    builder2.OpenElement(4, "div");
                    builder2.AddAttribute(5, "id", id);
                    builder2.AddAttribute(6, "style", "display:none;");
                    if (this is IHasChildren)
                    {
                        builder2.AddContent(7, (this as IHasChildren).ChildContent);
                    }
                    else if (this is IHasTextContent)
                    {
                        builder2.AddContent(7, (this as IHasTextContent).ChildContent);
                        builder2.AddElementReferenceCapture(8, elementRef => (this as IHasTextContent).TextContainingReference = elementRef);
                    }
                    builder2.CloseElement();
                }
                else
                {
                    ////Create react component on parent.
                    SelfChild = FluentParent.CreateReactChildComponent(this, FluentComponentAttributes, GetSerializedEvents(), FluentParent.SelfChild);
                    builder2.OpenElement(4, "div");
                    if (this is IHasChildren)
                    {
                        builder2.AddContent(5, (this as IHasChildren).ChildContent);
                    }
                    else if (this is IHasTextContent)
                    {
                        builder2.AddContent(5, (this as IHasTextContent).ChildContent);
                        builder2.AddElementReferenceCapture(6, elementRef =>
                        {
                            (this as IHasTextContent).TextContainingReference = elementRef;
                        });
                    }
                    builder2.CloseElement();

                }
            }));

            builder.CloseComponent();
        }

        public void Dispose()
        {
            
        }
    }
}
