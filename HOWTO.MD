# How to Build

- Open PowerShell; browse to root of the repository
- Note: you have to be online for this to work; since it does fresh package restore
- run ```.\build.ps1```; you should see output as follows:

```PowerShell
PS C:\Projects\Github\RulesRUs> .\build.ps1
Preparing to run build script...
Running build script...

========================================
Clean
========================================

========================================
Restore-NuGet-Packages
========================================
  Restore completed in 70.63 ms 
.
.

========================================
Build
========================================
Microsoft (R) Build Engine version 15.6.84.34536 for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

.
.

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:20.17

========================================
Run-Unit-Tests
========================================
running unit tests
.
.
.
========================================
Default
========================================

Task                          Duration
--------------------------------------------------
Clean                         00:00:00.1291243
Restore-NuGet-Packages        00:00:07.0229522
Build                         00:00:21.3961425
Run-Unit-Tests                00:00:12.9261100
--------------------------------------------------
Total:                        00:00:41.4879563
```


# How to Build Nuget Package

- Open PowerShell; change directory to root of the repository
- Make sure version, release notes, etc. had been updated as and where needed
- create or verify local directory for the output (like c:\nuget-local)
- run ```dotnet pack .\src\RuleEngine\RuleEngine.csproj -c Release -o C:\nuget-local\ --include-symbols -p:SymbolPackageFormat=snupkg```; you should see ouput like below:

```PowerShell
PS C:\Projects\Github\RulesRUs> dotnet pack .\src\RuleEngine\RuleEngine.csproj  -c Release -o C:\nuget-local\  --include-symbols -p:SymbolPackageFormat=snupkg --version-suffix "-pre3"
Microsoft (R) Build Engine version 15.9.20+g88f5fadfbe for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restoring packages for C:\Projects\Github\RulesRUs\src\RuleEngine\RuleEngine.csproj...
  Restore completed in 707.19 ms for C:\Projects\Github\RulesRUs\src\RuleEngine\RuleEngine.csproj.
  RuleEngine -> C:\Projects\Github\RulesRUs\src\RuleEngine\bin\Release\netstandard2.0\RuleEngine.dll
  Successfully created package 'C:\nuget-local\RuleEngine.___.nupkg'.
C:\Program Files\dotnet\sdk\2.1.502\Sdks\NuGet.Build.Tasks.Pack\build\NuGet.Build.Tasks.Pack.targets(202,5): warning NU5125: The 'licenseUrl' element will be deprecated. Consider using the 'license' element instead. [C:\Projects\Github\RulesRUs\src\RuleEngine\RuleEngine.csproj]
  Successfully created package 'C:\nuget-local\RuleEngine.___.snupkg'.
PS C:\Projects\Github\RulesRUs>
```

- include ```--version-suffix "-pre1"``` to create a pre-release version


