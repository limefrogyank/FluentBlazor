using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluentBlazor.Components
{
    public abstract class FrozenComponentBase : IComponent, IHandleAfterRender, IHandleEvent
    {
        private readonly RenderFragment _renderFragment;
        private RenderHandle _renderHandle;
        private bool _initialized;
        private bool _hasPendingQueuedRender;
        private bool _hasNeverRendered = true;
        private bool _hasCalledOnAfterRender;

        public FrozenComponentBase()
        {
            _renderFragment = builder =>
            {
                _hasPendingQueuedRender = false;
                _hasNeverRendered = false;
                BuildRenderTree(builder);
            };
        }
        protected virtual void BuildRenderTree(RenderTreeBuilder builder)
        {
            // Developers can either override this method in derived classes, or can use Razor
            // syntax to define a derived class and have the compiler generate the method.

            // Other code within this class should *not* invoke BuildRenderTree directly,
            // but instead should invoke the _renderFragment field.
        }

        public void Attach(RenderHandle renderHandle)
        {
            if (_renderHandle.IsInitialized)
            {
                throw new InvalidOperationException($"The render handle is already set. Cannot initialize a {nameof(ComponentBase)} more than once.");
            }

            _renderHandle = renderHandle;
        }

        public Task HandleEventAsync(EventCallbackWorkItem callback, object arg)
        {
            var task = callback.InvokeAsync(arg);
            var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
                task.Status != TaskStatus.Canceled;

            // After each event, we synchronously re-render (unless !ShouldRender())
            // This just saves the developer the trouble of putting "StateHasChanged();"
            // at the end of every event callback.
            StateHasChanged();

            return shouldAwaitTask ?
                CallStateHasChangedOnAsyncCompletion(task) :
                Task.CompletedTask;
        }

        public Task OnAfterRenderAsync()
        {
            var firstRender = !_hasCalledOnAfterRender;
            _hasCalledOnAfterRender |= true;

            OnAfterRender(firstRender);

            return OnAfterRenderAsync(firstRender);

            // Note that we don't call StateHasChanged to trigger a render after
            // handling this, because that would be an infinite loop. The only
            // reason we have OnAfterRenderAsync is so that the developer doesn't
            // have to use "async void" and do their own exception handling in
            // the case where they want to start an async task.
        }

        public virtual Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            if (!_initialized)
            {
                _initialized = true;

                return RunInitAndSetParametersAsync();
            }
            else
            {
                return CallOnParametersSetAsync();
            }
        }

        private async Task RunInitAndSetParametersAsync()
        {
            OnInitialized();
            var task = OnInitializedAsync();

            if (task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Canceled)
            {
                // Call state has changed here so that we render after the sync part of OnInitAsync has run
                // and wait for it to finish before we continue. If no async work has been done yet, we want
                // to defer calling StateHasChanged up until the first bit of async code happens or until
                // the end. Additionally, we want to avoid calling StateHasChanged if no
                // async work is to be performed.
                StateHasChanged();

                try
                {
                    await task;
                }
                catch // avoiding exception filters for AOT runtime support
                {
                    // Ignore exceptions from task cancellations.
                    // Awaiting a canceled task may produce either an OperationCanceledException (if produced as a consequence of
                    // CancellationToken.ThrowIfCancellationRequested()) or a TaskCanceledException (produced as a consequence of awaiting Task.FromCanceled).
                    // It's much easier to check the state of the Task (i.e. Task.IsCanceled) rather than catch two distinct exceptions.
                    if (!task.IsCanceled)
                    {
                        throw;
                    }
                }

                // Don't call StateHasChanged here. CallOnParametersSetAsync should handle that for us.
            }

            await CallOnParametersSetAsync();
        }

        private Task CallOnParametersSetAsync()
        {
            OnParametersSet();
            var task = OnParametersSetAsync();
            // If no async work is to be performed, i.e. the task has already ran to completion
            // or was canceled by the time we got to inspect it, avoid going async and re-invoking
            // StateHasChanged at the culmination of the async work.
            var shouldAwaitTask = task.Status != TaskStatus.RanToCompletion &&
                task.Status != TaskStatus.Canceled;

            // We always call StateHasChanged here as we want to trigger a rerender after OnParametersSet and
            // the synchronous part of OnParametersSetAsync has run.
            StateHasChanged();

            return shouldAwaitTask ?
                CallStateHasChangedOnAsyncCompletion(task) :
                Task.CompletedTask;
        }

        private async Task CallStateHasChangedOnAsyncCompletion(Task task)
        {
            try
            {
                await task;
            }
            catch // avoiding exception filters for AOT runtime support
            {
                // Ignore exceptions from task cancellations, but don't bother issuing a state change.
                if (task.IsCanceled)
                {
                    return;
                }

                throw;
            }

            StateHasChanged();
        }

        protected void StateHasChanged()
        {
            if (_hasPendingQueuedRender)
            {
                return;
            }

            if (_hasNeverRendered )
            {
                _hasPendingQueuedRender = true;

                try
                {
                    _renderHandle.Render(_renderFragment);
                }
                catch
                {
                    _hasPendingQueuedRender = false;
                    throw;
                }
            }
            else if (ShouldRender())
            {
                OnAfterRenderAsync();
            }
        }

        protected virtual void OnAfterRender(bool firstRender)
        {
        }

        protected virtual Task OnAfterRenderAsync(bool firstRender)
           => Task.CompletedTask;

        protected Task InvokeAsync(Action workItem)
            => _renderHandle.Dispatcher.InvokeAsync(workItem);

        protected Task InvokeAsync(Func<Task> workItem)
            => _renderHandle.Dispatcher.InvokeAsync(workItem);

        protected virtual bool ShouldRender()
            => true;

        protected virtual void OnInitialized()
        {
        }

        protected virtual Task OnInitializedAsync()
            => Task.CompletedTask;

        protected virtual void OnParametersSet()
        {
        }

        protected virtual Task OnParametersSetAsync()
            => Task.CompletedTask;

    }
}
