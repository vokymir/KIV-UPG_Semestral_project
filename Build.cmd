@mkdir .\bin
cd src
echo y | dotnet add package Flee
echo y | dotnet add package Scottplot.WinForms
cd ..
echo a | dotnet msbuild ./src/ElectricFieldVis.sln -t:Rebuild -p:Configuration=Release
echo a | xcopy .\src\bin\Release\net8.0-windows\*.* .\bin /S
