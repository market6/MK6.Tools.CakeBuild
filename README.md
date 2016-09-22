To get started, run the below commands in a powershell console to download and then execute the bootstrapper. Executing this script will download additional scripts needed to setup a fully functional cake build.

Run this command from your solution root...

```powershell
Invoke-WebRequest -Uri https://raw.githubusercontent.com/market6/MK6.Tools.CakeBuild/master/Bootstrapper/bootstrap.ps1 -OutFile bootstrap.ps1; .\bootstrap.ps1
```
