#
# The goal of this script is to make a nupkg which contains node.exe and a custom npm.cmd
# for use as a CakeBuild tool. This will allow the build server to resolve node version easily.
#

$build_folder=$(Join-path $pwd "build")
$release_uri="https://nodejs.org/dist/v6.8.0/node-v6.8.0-win-x86.zip"
$node_version="6.8.0"
$download_file=$(Join-path $build_folder "node_release.zip")

mkdir $build_folder -Force
cd $build_folder

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

if ( ! (Test-Path $download_file) ) {
    Invoke-WebRequest $release_uri -OutFile $download_file
}

Unzip $download_file $pwd

cp **/node.exe .
cp ../*.nuspec .
cp ../npm.cmd .

nuget pack -Version $node_version

