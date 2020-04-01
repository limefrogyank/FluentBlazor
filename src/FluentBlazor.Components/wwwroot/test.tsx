'use strict';

interface DotNetReferenceType {
    //invokeMethod<T>(methodIdentifier: string, ...args: any[]): T;
    invokeMethodAsync<T>(methodIdentifier: string, ...args: any[]): Promise<T>;
}
interface SerializedEvent {
    eventName: string;
    dotNetRef: DotNetReferenceType;
}
interface IChild {
    componentName: string,
    parameters: any,
    serializedEvents: SerializedEvent[]
    children: IChild[];
}


const { Checkbox, FocusZone, PrimaryButton, Stack } = (window as any).Fabric;
const FFabric = (window as any).Fabric.Fabric;

class BlazorComponent extends React.Component<any,any> {
    constructor(props) {
        super(props);
    }

    render() {
        let FluentComponent = components[this.props.componentName];

        return (
            <FFabric>
                <FluentComponent {...this.props} />
            </FFabric>
        );
    }
}


function createComponent(id: string, parameters: any, serializedEvents: SerializedEvent[], children: IChild[]) {
    const domContainer = document.querySelector("#" + id);
    let reactContainer = document.createElement("div");
    domContainer.parentNode.insertBefore(reactContainer, domContainer.nextSibling);
    let restore : boolean = false;
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
    } else {
        let reactElement = React.createElement(BlazorComponent, moddedParams);
        ReactDOM.render(reactElement, reactContainer);
    }
    return restore;
}

function updateComponent(id: string, parameters: any, serializedEvents: SerializedEvent[], children: IChild[]) {
    const domContainer = document.querySelector("#" + id);
    const reactContainer = domContainer.nextSibling as Element;

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
    } else {
        let reactElement = React.createElement(BlazorComponent, moddedParams);
//        reactElements.set(id, reactElement);
        ReactDOM.render(reactElement, reactContainer);
    }
    return true;
}

function processChildren(children: IChild[]): any[] {
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

function incorporateEvents(params: any, serializedEvents: SerializedEvent[]) : any {
    serializedEvents.forEach((v, i) => {
        params[v.eventName] = () => { v.dotNetRef.invokeMethodAsync("InvokableEvent"); console.log("event invoked"); };
    });
    return params;
}

let components = {
    "Checkbox": Checkbox,
    "FocusZone": FocusZone,
    "PrimaryButton": PrimaryButton,
    "Stack": Stack
}