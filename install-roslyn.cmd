mkdir roslyn
cd roslyn
powershell.exe -executionpolicy Unrestricted -Command "& {Invoke-WebRequest -OutFile nuget.exe https://dist.nuget.org/win-x86-commandline/latest/nuget.exe}"
cd roslyn
nuget install Microsoft.Net.Compilers
nuget install Microsoft.CodeAnalysis 
