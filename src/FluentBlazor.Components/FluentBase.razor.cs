using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public partial class FluentBase : ComponentBase, IDisposable
    {
        protected string id = "G" + Guid.NewGuid().ToString();

        [CascadingParameter(Name = "FluentParent")]
        private FluentBase FluentParent { get; set; }

        [Inject] 
        protected IJSRuntime JSRuntime { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> FluentComponentAttributes { get; set; } = new Dictionary<string, object>();

        protected List<TranslatedEvent> eventContainer = new List<TranslatedEvent>();

        protected List<ReactChild> reactChildren = new List<ReactChild>();
        public ReactChild SelfChild { get; set; }

        private bool needsRestoreInnerChildren = false;

        private bool shouldRerender = false;



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (this is IHasChildren)
                {
                    if (FluentComponentAttributes == null)
                        FluentComponentAttributes = new Dictionary<string, object>();
                    //FluentComponentAttributes.Add("children", reactChildren);
                }
                if (FluentParent == null)
                    needsRestoreInnerChildren = await JSRuntime.InvokeAsync<bool>("createComponent", id, FluentComponentAttributes, GetSerializedEvents(), reactChildren);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (SelfChild != null)
            {
                SelfChild.parameters = FluentComponentAttributes;
                SelfChild.serializedEvents = GetSerializedEvents();
            }
            //if (shouldRender)
            //{
            //    foreach (var ev in eventContainer)
            //    {
            //        ev.Dispose();
            //    }

            //    eventContainer.Clear();

            //    if (needsRestoreInnerChildren)
            //    {
            //        await JSRuntime.InvokeVoidAsync("restoreInnerContent", id);
            //        needsRestoreInnerChildren = false;
            //    }

            //    reactChildren.Clear();
            //}
            await base.OnParametersSetAsync();
        }

        protected override bool ShouldRender()
        {
            //if (shouldRerender)
            {
                shouldRerender = false;

                // need to traverse the reactChildren list and invoke updates that way.
                if (reactChildren.Count > 0)
                {
                    UpdateComponentAsync().ContinueWith(task =>
                    {
                        RerenderReactComponent();
                    });
                }

                //// don't need this recursion because only the "root" components get updated.
                //if (FluentParent != null && SelfChild != null)
                //{
                //    SelfChild.parameters = FluentComponentAttributes;
                //    SelfChild.serializedEvents = GetSerializedEvents();
                //    FluentParent.RerenderReactComponent();
                //}
                
                return false;
            }
            return false;

            //return base.ShouldRender();
        }

        public async Task UpdateComponentAsync()
        {
            List<Task> tasks = new List<Task>();
            reactChildren.ForEach(child =>
            {
                tasks.Add(child.Self.UpdateComponentAsync());
            });
            tasks.Add(OnParametersSetAsync());
            await Task.WhenAll(tasks);
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

        public async void RerenderReactComponent()
        {
            if (FluentParent != null)
            {
                //FluentParent.RerenderReactComponent();
            }
            else
            {
                needsRestoreInnerChildren = await JSRuntime.InvokeAsync<bool>("createComponent", id, FluentComponentAttributes, GetSerializedEvents(), reactChildren);
            }
        }
        public void Dispose()
        {
            
        }
    }
}
