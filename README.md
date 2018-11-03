# AB.Extensions
This is an extensions library I've built up over time, free of external dependencies, with constantly increasing test coverage and benchmarks.  Constant strings, simple enums, and missing LINQ methods all included.  

The source now lives in Azure DevOps, but is always pushed to Github via Continuous Integration in Azure.



__Feature Build__

[![Primary Build in Azure Pipelines](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/AB.Extensions%20Github%20Project)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build?definitionId=2)


__CI Sync to Github__

[![CI Sync](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/Sync%20to%20Github)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build/latest?definitionId=3)

Import easily by editing your Csproj:

``<PackageReference Include="AB.Extensions" Version="2.1.0" />``

Alternatively with dotnet CLI:

``dotnet add package AB.Extensions``