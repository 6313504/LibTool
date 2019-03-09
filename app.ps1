#  set-ExecutionPolicy RemoteSigned  -Scope CurrentUser
$key = "482fe812-8202-4365-b06f-c610f5ee51f7"
$PWD
$csprojPath = "LibTool/"
$projectName = "LibTool"
$binPath = $projectName + "/bin/Release/"
$binPath
$csprojFile = $csprojPath + $projectName + ".csproj"
$csprojFile

$content = Get-Content -Path $csprojFile -Encoding UTF8
$regex = [regex]"Version>([^<]+)"
$version = $regex.Matches($content)[0].Groups[1].Value
$nupkg = $binPath + "Feebool." + $projectName + "." + $version + ".nupkg"
$symbolsNupkg = $binPath + "Feebool." + $projectName + "." + $version + ".symbols.nupkg"

$cmd = "dotnet pack " + $csprojFile + " -c Release --include-source --include-symbols"
$cmd 
cmd /c $cmd 

$cmd = "nuget.exe push " + $nupkg + " " + $key + " -Source https://www.myget.org/F/feebool/api/v2/package"
$cmd 
cmd /c $cmd 

$cmd = "nuget.exe push " + $symbolsNupkg + " " + $key + " -Source https://www.myget.org/F/feebool/symbols/api/v2/package"
$cmd 
cmd /c $cmd
    