SignalPlot | Process-signal GUI (“Oscilloscope”) based on the .NET /  UNO-platform 
============
![image](https://github.com/Jonathanio123/SignalPlot/assets/73334350/40eef69d-d2b3-4d4b-90e5-d59762d7fad4)

Written by [Jonathanio123](https://github.com/Jonathanio123) and [goran515](https://github.com/goran515)
-----

The component is meant to visualize live process-signals from the paint control system. This can be considered a multi-channel oscilloscope running on an App and/or in a browser.

Target platforms: Windows 10/11 (WinUI) and Firefox/Chrome (WebAssembly)

[ScottPlot 5 is used as the plotting library](https://github.com/scottplot/scottplot)

## Downloading latest development release:
When new commits are pushed to the main branch a new build is automatically created and published. These are kept for 90 days. Builds are published in release mode. 

*These builds are Self-Contained so no external dependencies should be needed. However, for the wasm build a webserver is needed to host the app* 💡

1. [Link to the latest successful publish action ✔](https://github.com/Jonathanio123/SignalPlot/actions/workflows/publish.yml?query=is%3Asuccess+branch%3Amain)
2. Select the latest build (top of the list)
3. Scroll down to the "Artifacts" section
4. Click on SampleApp.WinUI to download the latest build of the WinUI app 
   or click on SampleApp.Wasm to download the latest build of the Wasm app
5. Unzip the downloaded file to a folder of your choice

* Now for the WinUI app simply run the ```SampleApp.Windows.exe``` to launch the app 🚀 

* For the Wasm build, you need to run a local webserver to serve the app. I recommend [dotnet-serve](https://github.com/natemcmaster/dotnet-serve), but you can use any other webserver. 
    1. Install the dotnet-serve tool, [requires .Net 5 or newer](https://get.dot.net/): ```dotnet tool install -g dotnet-serve```
    2. Run the command ```dotnet serve -p 8080``` while inside the downloaded folder
    3. Open a browser and navigate to ```http://localhost:8080``` 🚀

## Building from source build
1. I suggest following the official Uno [Getting Started](https://platform.uno/docs/articles/get-started.html?tabs=windows) guide
2. Once your environment is setup, Clone the repository: ```git clone https://github.com/Jonathanio123/SignalPlot```
3. Then in your IDE, open the ```SignalPlot.sln``` file
4. Then select your target platform (WinUI or Wasm) and run the project


<!---
## Known Issues ⚠
* None at the moment
    
-->
