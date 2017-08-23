@echo off

echo Started

SET currentPath=%~dp0
SET vkServicePath=%~dp0..\backend\HashtagAggregatorVk.Service

setx ASPNETCORE_ENVIRONMENT dev

cd %vkServicePath%

echo %vkServicePath%

dotnet run --no-launch-profile

cd %currentPath%

echo Finished


