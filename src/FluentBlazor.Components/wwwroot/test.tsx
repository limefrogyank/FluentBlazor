'use strict';
//const e = React.createElement;

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
        console.log(this.props.componentName);
        let FluentComponent = components[this.props.componentName];


        return (
            <FFabric>
                <FluentComponent {...this.props} />
            </FFabric>
        );
    }
}

const inner: Map<string, string> = new Map<string, string>();

function createComponent(id: string, parameters: any, serializedEvents: SerializedEvent[], children: IChild[]) {
    const domContainer = document.querySelector("#" + id);
    let restore : boolean = false;
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
    } else {
        ReactDOM.render(React.createElement(BlazorComponent, moddedParams), domContainer);
    }
    return restore;
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

function restoreInnerContent(id :string) {
//    inner.forEach((v, k) => {
    const domContainer = document.querySelector("#" + id);
    ReactDOM.unmountComponentAtNode(domContainer);
    domContainer.innerHTML = inner.get(id);
//    });
}

function incorporateEvents(params: any, serializedEvents: SerializedEvent[]) : any {

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
}