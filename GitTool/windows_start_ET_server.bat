@Echo off

cd ..\ET_framework\ET\Bin

@Echo on

dotnet Server.dll --Process=1 --Console=1

@Echo off

pause
exit

@Echo on