# LitePlacer
LitePlacer machine user interface and control software

To get this to compile: 
-install AForge.NET
-In solution explorer - LitePlacer - References, delete references to Math.Net.Numerics and to HomographyEstimation.dll.
-Add reference to <your LitePlacer software directory>/packages/HomographyEstimation/HomographyEstimation.dll
-run the following command in the Tools - NuGet Package Manager - Package Manager Console: PM> Install-Package MathNet.Numerics
