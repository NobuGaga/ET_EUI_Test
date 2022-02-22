@Echo off

cd ..\ET_framework\ET\Bin

@Echo on

dotnet Tools.dll --AppType=ExcelExporter

@Echo off

pause
exit

@Echo on