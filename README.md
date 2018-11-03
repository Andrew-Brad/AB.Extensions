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


## CI Packaging
The VSTS feed which hosts the packages uploaded by CI is publically available at: https://zep519.pkgs.visualstudio.com/_packaging/Ab.Extensions-CI/nuget/v3/index.json

Release packages are uploaded to Nuget.org under the following conditions:
- master branch
- manual trigger of build
- all previous steps succeeded in the build
- when manually queuing the build, a variable of name ```PushReleaseNuget``` is provided with value ```confirm```

This means that when making code modifications, I need to respect the rules of SemVer in the csproj metadata.