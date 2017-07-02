nuget restore
msbuild Elements.sln /m /validate /t:Clean;Build /p:Configuration=Release
