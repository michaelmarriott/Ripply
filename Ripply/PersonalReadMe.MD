
dotnet build
dotnet publish

dotnet run PickAndPayScrapper -T update 
dotnet run MarkoScarpper -T update 
dotnet run DischemScrapper -T update 


dotnet pack Ripply.csproj

C:\Apps\nuget push bin\debug\Ripply.0.0.2.nupkg 27262ba5-a080-48f1-9c31-fd6ad8fadbae -Source https://www.nuget.org/api/v2/package