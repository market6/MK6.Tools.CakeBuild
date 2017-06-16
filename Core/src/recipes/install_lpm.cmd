@ECHO OFF
IF NOT EXIST "tools" (md "tools")
IF NOT EXIST "tools\modules" (md "tools\modules")
IF NOT EXIST "tools\modules\Cake.LongPath.Module" (nuget.exe install Cake.LongPath.Module -ExcludeVersion -OutputDirectory "Tools\Modules" -Source https://www.nuget.org/api/v2/)