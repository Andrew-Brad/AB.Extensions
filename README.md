# AB.Extensions

This is a C# extensions library containing things like string constants, simple enums, and additional LINQ methods. I maintain this library free of any dependencies, and maintain benchmarking and unit testing projects.

I also keep my library build/pack template in here, which I like to extend to other projects for a given use case.

Source code now officially lives in Azure DevOps, but is continuously pushed to Github via Azure Pipelines CI.

## Builds in Azure DevOps

[![Primary Build in Azure Pipelines](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/AB.Extensions%20Github%20Project)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build?definitionId=2)

## CI Sync to Github

[![CI Sync](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_apis/build/status/Sync%20to%20Github)](https://zep519.visualstudio.com/AB.Extensions%20Github%20Project/_build/latest?definitionId=3)

Import easily by editing your csproj:

``<PackageReference Include="AB.Extensions" Version="3.0.0" />``

Alternatively with dotnet CLI:

``dotnet add package AB.Extensions``

## CI Packaging

The Azure Artifacts feed which hosts the prerelease packages (uploaded by CI) is publically available [here](https://zep519.pkgs.visualstudio.com/_packaging/Ab.Extensions-CI/nuget/v3/index.json).

If you prefer Myget, those are located [here](https://www.myget.org/F/andrew-ci/api/v3/index.json).

Release versions are automatically uploaded to Nuget.org by CI under the following conditions:

- master branch
- all previous steps succeeded in the build
- manual queue of build
- when manually queuing the build, a variable of name ```PushReleaseNuget``` should be provided with value ```confirm```

This means that when making new branches for code modifications, it's a good practice to immediately identify the desired SemVer in the csproj metadata, and ensure the code change adheres accordingly.
