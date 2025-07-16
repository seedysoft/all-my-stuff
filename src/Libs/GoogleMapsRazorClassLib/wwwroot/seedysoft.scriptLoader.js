window.seedysoft.scriptLoader = window.seedysoft.scriptLoader || {
  init: (elementId, async, defer, scriptId, source, type, dotNetHelper) => {
    if (source.length === 0) {
      console.error("Invalid source url.");
      return;
    }

    let scriptLoaderElement = document.getElementById(elementId);

    if (scriptLoaderElement == null) {
      window.alert(`Cannot find Element ${elementId}`);
    }
    else {
      let scriptElement = document.createElement("script");

      scriptElement.async = async;

      scriptElement.defer = defer;

      if (scriptId != null)
        scriptElement.id = scriptId;

      if (source != null)
        scriptElement.src = source;

      if (type != null)
        scriptElement.type = type;

      scriptElement.addEventListener("error", (_event) => {
        dotNetHelper.invokeMethodAsync("OnErrorJS", `An error occurred while loading the script: ${source}`);
      });

      scriptElement.addEventListener("load", (_event) => {
        dotNetHelper.invokeMethodAsync("OnLoadJS");
      });

      scriptLoaderElement.appendChild(scriptElement);
    }
  }
}
