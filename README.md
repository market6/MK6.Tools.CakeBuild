To get started, run the below commands in a powershell console to download and then execute the bootstrapper. Executing this script will download additional scripts needed to setup a fully functional cake build.

Run this command from your solution root to download the bootstrap script...

```powershell
Invoke-WebRequest -Uri https://raw.githubusercontent.com/market6/MK6.Tools.CakeBuild/master/Bootstrapper/bootstrap.ps1 -OutFile bootstrap.ps1
```

Run this command to execute the bootstrap script
```powershell
.\bootstrap.ps1
```

Once bootstrap.ps1 is executed it will download all necessary scripts along with some documentation on how to run your newly setup cake build!
