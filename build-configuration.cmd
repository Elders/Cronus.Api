@echo off

@powershell -File .nyx\build.ps1 '--appname=Elders.Cronus.Api' '--nugetPackageName=Cronus.Api'
@powershell -File .nyx\build.ps1 '--appname=Elders.Cronus.Dashboard' '--nugetPackageName=Cronus.Dashboard'

