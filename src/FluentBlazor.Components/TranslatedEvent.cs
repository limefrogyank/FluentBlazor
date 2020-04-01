using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FluentBlazor.Components
{
    public class SerializedEvent
    {
        public string eventName { get; set; }
        public DotNetObjectReference<TranslatedEvent> dotNetRef { get; set; }
    }


    public class TranslatedEvent<T>
    {
        public string EventName { get; set; }
        EventCallback<T> callback;

        public TranslatedEvent(string eventName, EventCallback<T> callback)
        {
            this.EventName = eventName;
            this.callback = callback;
        }

        public DotNetObjectReference<TranslatedEvent<T>> GetDotNetObjectReference()
        {
            return DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public void InvokableEvent(T args)
        {
            callback.InvokeAsync(args);
        }

    }

    public class TranslatedEvent : IDisposable
    {
        public string EventName { get; set; }
        EventCallback callback;
        private DotNetObjectReference<TranslatedEvent> dotNetRef;

        public TranslatedEvent(string eventName, EventCallback callback)
        {
            this.EventName = eventName;
            this.callback = callback;
        }

        public DotNetObjectReference<TranslatedEvent> GetDotNetObjectReference()
        {
            dotNetRef = DotNetObjectReference.Create(this);
            return dotNetRef;
        }

        [JSInvokable]
        public void InvokableEvent()
        {
            callback.InvokeAsync(null);
        }

        public void Dispose()
        {
            dotNetRef?.Dispose();
        }
    }
}
