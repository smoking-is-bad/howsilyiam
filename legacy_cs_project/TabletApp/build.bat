@echo off

rem Copyright © 2015 by Sensor Networks, Inc. All rights reserved.
rem
rem This is the build script for the NanoSense project
rem This automates the build steps for the nanoSense application
rem The result is a nanoSense.msi file
rem 
rem Syntax is:
rem   build XX.XX.XX.XX
rem where XX.XX.XX.XX is the version number (eg 1.0.0.101)

rem Do the build via msbuild

set WIXBIN="C:\Program Files (x86)\WiX Toolset v3.11\bin"

setlocal

rem pre VS 2017
rem call "%VS140COMNTOOLS%\vsvars32.bat"
rem VS 2017-? replaces vsvars32.bat and does not define anything like VS140COMNTOOLS
call "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\Tools\VsDevCmd.bat"

.\ThirdParty\UpdateVersion -p %1 -i .\Properties\AssemblyInfo.cs -o .\Properties\AssemblyInfo.cs
.\ThirdParty\UpdateVersion -p %1 -v File -i .\Properties\AssemblyInfo.cs -o .\Properties\AssemblyInfo.cs
msbuild .\TabletApp.csproj /target:rebuild /property:Configuration=Release /property:ApplicationVersion=%1

if exist "%WIXBIN%\candle.exe" (
	%WIXBIN%\candle.exe -ext WixUtilExtension -dVersion="%1" .\Installer\nanosense.wxs -out .\Installer\WixOut\nanosense.wixobj
	%WIXBIN%\light.exe -ext WixUIExtension -ext WixUtilExtension -ext WixNetFxExtension .\Installer\WixOut\nanosense.wixobj -out smartPIMS%1.msi
) else (
	type "ERROR: You need to install WiX Toolset. http://wixtoolset.org/releases/"
)
endlocal