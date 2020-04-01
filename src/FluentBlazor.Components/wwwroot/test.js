'use strict';
const { Checkbox, FocusZone, PrimaryButton, Stack } = window.Fabric;
const FFabric = window.Fabric.Fabric;
class BlazorComponent extends React.Component {
    constructor(props) {
        super(props);
    }
    render() {
        let FluentComponent = components[this.props.componentName];
        return (React.createElement(FFabric, null,
            React.createElement(FluentComponent, Object.assign({}, this.props))));
    }
}
function createComponent(id, parameters, serializedEvents, children) {
    const domContainer = document.querySelector("#" + id);
    let reactContainer = document.createElement("div");
    domContainer.parentNode.insertBefore(reactContainer, domContainer.nextSibling);
    let restore = false;
    restore = true;
    // make events that callback .net functions
    let moddedParams = incorporateEvents(parameters, serializedEvents);
    // generate proper react children
    let convertedChildren = [];
    if (children) {
        convertedChildren = processChildren(children);
    }
    if (convertedChildren.length > 0) {
        let reactElement = React.createElement(BlazorComponent, moddedParams, ...convertedChildren);
        ReactDOM.render(reactElement, reactContainer);
    }
    else {
        let reactElement = React.createElement(BlazorComponent, moddedParams);
        ReactDOM.render(reactElement, reactContainer);
    }
    return restore;
}
function updateComponent(id, parameters, serializedEvents, children) {
    const domContainer = document.querySelector("#" + id);
    const reactContainer = domContainer.nextSibling;
    // make events that callback .net functions
    let moddedParams = incorporateEvents(parameters, serializedEvents);
    // generate proper react children
    let convertedChildren = [];
    if (children) {
        convertedChildren = processChildren(children);
    }
    //    console.log(moddedParams);
    if (convertedChildren.length > 0) {
        let reactElement = React.createElement(BlazorComponent, moddedParams, ...convertedChildren);
        //        reactElements.set(id, reactElement);
        ReactDOM.render(reactElement, reactContainer);
    }
    else {
        let reactElement = React.createElement(BlazorComponent, moddedParams);
        //        reactElements.set(id, reactElement);
        ReactDOM.render(reactElement, reactContainer);
    }
    return true;
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
//function restoreInnerContent(id :string) {
//    const domContainer = document.querySelector("#" + id);
//}
function incorporateEvents(params, serializedEvents) {
    serializedEvents.forEach((v, i) => {
        params[v.eventName] = () => { v.dotNetRef.invokeMethodAsync("InvokableEvent"); console.log("event invoked"); };
    });
    return params;
}
function getTextContent(element) {
    return element.innerText;
}
let components = {
    "Checkbox": Checkbox,
    "FocusZone": FocusZone,
    "PrimaryButton": PrimaryButton,
    "Stack": Stack,
    "Text": window.Fabric.Text
};
//# sourceMappingURL=test.js.map