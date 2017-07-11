@Echo off
Setlocal
Echo Parameter 1: Path to DirectX  SDK: %~1

:: Get windows Version numbers
For /f "tokens=2 delims=[]" %%G in ('ver') Do (set _version=%%G) 

For /f "tokens=2,3,4 delims=. " %%G in ('echo %_version%') Do (set _major=%%G& set _minor=%%H& set _build=%%I) 

Echo Major version: %_major%  Minor Version: %_minor%.%_build%

if "%_major%"=="5" goto sub5
if "%_major%"=="6" goto sub6
if "%_major%"=="10" goto sub10

Echo unsupported version
goto:eof

:sub5
::Winxp or 2003
if "%_minor%"=="2" goto sub_2003
Echo Windows XP [%PROCESSOR_ARCHITECTURE%]
goto useDirectXSDK
goto:eof

:sub_2003
Echo Windows 2003 or XP 64 bit [%PROCESSOR_ARCHITECTURE%]
goto useDirectXSDK
goto:eof

:sub6
if "%_minor%"=="1" goto sub7
if "%_minor%"=="2" goto sub8
if "%_minor%"=="3" goto sub8_1
if "%_minor%"=="4" goto sub10
Echo Windows Vista or Windows Server 2008 [%PROCESSOR_ARCHITECTURE%]
goto useDirectXSDK
goto:eof

:sub7
Echo Windows 7 or Windows Server 2008 R2 [%PROCESSOR_ARCHITECTURE%]
goto useDirectXSDK
goto:eof

:sub8
Echo Windows 8 or Windows Server 2012 [%PROCESSOR_ARCHITECTURE%]
goto useWinSDK
goto:eof

:sub8_1
Echo Windows 8.1 or Windows Server 2012 R2 [%PROCESSOR_ARCHITECTURE%]
goto useWinSDK
goto:eof

:sub10
Echo Windows 10 [%PROCESSOR_ARCHITECTURE%]
goto useWinSDK
goto:eof

:useDirectXSDK
"%~1Utilities\Bin\x86\fxc.exe" /T ps_3_0 /E main /Fo "%CD%\..\..\Shader\Compiled\FlexiWallEffect.ps" "%CD%\..\..\Shader\Source\FlexiWallEffect.fx"
"%~1Utilities\Bin\x86\fxc.exe" /T ps_3_0 /E main /Fo "%CD%\..\..\Shader\Compiled\ImageWarpingEffect.ps" "%CD%\..\..\Shader\Source\ImageWarpingEffect.fx"
goto:eof

:useWinSDK
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\fxc.exe" /T ps_3_0 /E main /Fo "%CD%\..\..\Shader\Compiled\FlexiWallEffect.ps" "%CD%\..\..\Shader\Source\FlexiWallEffect.fx"
"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\fxc.exe" /T ps_3_0 /E main /Fo "%CD%\..\..\Shader\Compiled\ImageWarpingEffect.ps" "%CD%\..\..\Shader\Source\ImageWarpingEffect.fx"
goto:eof