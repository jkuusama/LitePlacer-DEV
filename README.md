# LitePlacer
LitePlacer Pick and Place machine user interface and control software

**See  http://www.liteplacer.com/ for information.**

[Introduction video on Youtube](https://www.youtube.com/watch?v=3c5Vtuefm7o)

To get the code to compile: 

* Install AForge.NET

* In solution explorer-LitePlacer-References, delete references to Math.Net.Numerics and to HomographyEstimation.dll.

* Add reference to <your LitePlacer software directory>/packages/HomographyEstimation/HomographyEstimation.dll

* Run the following command in the Tools-NuGet Package Manager-Package Manager Console: PM> Install-Package MathNet.Numerics

To avoid issues in debugging, turn off the "Enable property evaluation and other implicit function calls" option in Tools->Options->Debugging 
