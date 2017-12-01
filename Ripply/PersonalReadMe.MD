
dotnet build
dotnet publish

dotnet run PickAndPayScrapper -T update 
dotnet run MarkoScarpper -T update 
dotnet run DischemScrapper -T update 


"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MsBuild.exe" /t:pack Ripply.csproj

nuget push Ripply.1.0.0.0.nupkg 27262ba5-a080-48f1-9c31-fd6ad8fadbae -Source https://www.nuget.org/api/v2/package