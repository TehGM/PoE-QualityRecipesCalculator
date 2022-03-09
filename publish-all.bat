@echo off

set outputdir=QualityRecipesCalculator\bin\Output\
set publishdir=QualityRecipesCalculator\bin\Release\net5.0\publish\
set appname=QualityRecipesCalculator

:: clean output directories
if exist %outputdir% rmdir /s /q %outputdir%
if exist %publishdir% rmdir /s /q %publishdir%
mkdir %outputdir%

set name="Portable (Framework Dependent)"
echo Publishing: %name%
dotnet publish -p:PublishProfile=%name%
tar -a -c -f %outputdir%%appname%-Portable.zip -C %publishdir%portable *

set name="Win x86"
echo Publishing: %name%
dotnet publish -p:PublishProfile=%name%
tar -a -c -f %outputdir%%appname%-Win-x86.zip -C %publishdir%win-x86 *

set name="Win x64"
echo Publishing: %name%
dotnet publish -p:PublishProfile=%name%
tar -a -c -f %outputdir%%appname%-Win-x64.zip -C %publishdir%win-x64 *

set name="Linux x64"
echo Publishing: %name%
dotnet publish -p:PublishProfile=%name%
tar -a -c -f %outputdir%%appname%-Linux-x64.zip -C %publishdir%linux-x64 *