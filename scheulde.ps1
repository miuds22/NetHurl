$path = $(pwd).Path
#en caso de que no exista, creamos la carpeta PS en C:
if (-not (Test-Path "C:\NETExplorer")) {
    New-Item -Path "C:\" -Name "NETExplorer" -ItemType "directory"
}   
#copiamos el exe a la carpeta de windows 
Copy-Item -Path "$path\HurliNet.exe" -Destination "C:\NETExplorer\"
#creamos una tarea programada que se ejecute cada 5 minutos
$trigger = New-ScheduledTaskTrigger -Once -At (Get-Date).AddMinutes(1)
$action = New-ScheduledTaskAction -Execute "C:\NETExplorer\HurliNet.exe"
Register-ScheduledTask -TaskName "HurliNet" -Trigger $trigger -Action $action