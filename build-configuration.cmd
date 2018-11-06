@echo off

set nugetserver=https://marketvision.myget.org/F/qore-deployment/api/v2/package

@powershell -File .nyx\build.ps1 '--appname=Elders.Cronus.Api' '--nugetPackageName=Cronus.Api' '--type=app'

