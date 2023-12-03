$date = Get-Date
$version = $date.ToString("yyyy-dd-M--HH-mm-ss")
$filename = "S3Liquid-" + $version + ".zip"
cd .\S3Liquid\src\S3Liquid
dotnet lambda package ..\..\..\Packages\$filename --configuration Release -frun dotnet6 -farch arm64
cd ..\..\..