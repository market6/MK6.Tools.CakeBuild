To get started, run the below command in a powershell console to download and execute the bootstrapper. This script will download additional scripts needed to setup a fully functional cake build.

```powershell
Invoke-WebRequest -Uri https://raw.githubusercontent.com/market6/MK6.Tools.CakeBuild/master/Bootstrapper/bootstrap.ps1 -OutFile bootstrap.ps1; .\bootstrap.ps1
```