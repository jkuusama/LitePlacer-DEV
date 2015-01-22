# LitePlacer
LitePlacer pick andpPlace machine user interface and control software

**See  http://www.liteplacer.com/ for information.**

![TinyG v7 Board](https://www.youtube.com/watch?v=0bYrwi3UA_A)

To get this to compile: 

* Install AForge.NET

* In solution explorer-LitePlacer-References, delete references to Math.Net.Numerics and to HomographyEstimation.dll.

* Add reference to <your LitePlacer software directory>/packages/HomographyEstimation/HomographyEstimation.dll

* Run the following command in the Tools-NuGet Package Manager-Package Manager Console: PM> Install-Package MathNet.Numerics
