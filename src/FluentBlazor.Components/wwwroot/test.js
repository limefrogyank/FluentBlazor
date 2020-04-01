'use strict';
const { Checkbox, FocusZone, PrimaryButton, Stack } = window.Fabric;
const FFabric = window.Fabric.Fabric;
class BlazorComponent extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        console.log(this.props.componentName);
        let FluentComponent = components[this.props.componentName];
        return (React.createElement(FFabric, null,
            React.createElement(FluentComponent, Object.assign({}, this.props))));
    }
}
const inner = new Map();
function createComponent(id, parameters, serializedEvents, children) {
    const domContainer = document.querySelector("#" + id);
    let restore = false;
    //if (domContainer.innerHTML !== "") {
    inner.set(id, domContainer.innerHTML);
    //}
    restore = true;
    console.log(serializedEvents);
    // make events that callback .net functions
    let moddedParams = incorporateEvents(parameters, serializedEvents);
    // generate proper react children
    let convertedChildren = [];
    if (children) {
        convertedChildren = processChildren(children);
    }
    console.log(moddedParams);
    if (convertedChildren.length > 0) {
        ReactDOM.render(React.createElement(BlazorComponent, moddedParams, ...convertedChildren), domContainer);
    }
    else {
        ReactDOM.render(React.createElement(BlazorComponent, moddedParams), domContainer);
    }
    return restore;
}
function processChildren(children) {
    let convertedChildren = [];
    children.forEach((v, i) => {
        // make events that callback .net functions on the children
        let childModdedParams = incorporateEvents(v.parameters, v.serializedEvents);
        let processedChildren = processChildren(v.children);
        convertedChildren.push(React.createElement(components[v.componentName], childModdedParams, ...processedChildren));
    });
    return convertedChildren;
}
function restoreInnerContent(id) {
    //    inner.forEach((v, k) => {
    const domContainer = document.querySelector("#" + id);
    ReactDOM.unmountComponentAtNode(domContainer);
    domContainer.innerHTML = inner.get(id);
    //    });
}
function incorporateEvents(params, serializedEvents) {
    serializedEvents.forEach((v, i) => {
        params[v.eventName] = () => v.dotNetRef.invokeMethodAsync("InvokableEvent");
    });
    return params;
}
let components = {
    "Checkbox": Checkbox,
    "FocusZone": FocusZone,
    "PrimaryButton": PrimaryButton,
    "Stack": Stack
};
//# sourceMappingURL=test.js.map