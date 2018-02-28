@echo off

%FAKE% %NYX% "target=clean" -st
%FAKE% %NYX% "target=RestoreNugetPackages" -st

IF NOT [%1]==[] (set RELEASE_NUGETKEY="%1")

SET SUMMARY="Cronus.Api"
SET DESCRIPTION="Cronus.Api"

%FAKE% %NYX% appName=Elders.Cronus.Api appSummary=%SUMMARY% appDescription=%DESCRIPTION% nugetPackageName=Cronus.Api nugetkey=%RELEASE_NUGETKEY%
