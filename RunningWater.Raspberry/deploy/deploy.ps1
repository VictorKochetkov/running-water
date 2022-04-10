$RemoteHost = '192.168.1.54'
$User = 'rdp'
$Password = 'raspberry'

# Execute command on Raspberry
function SSH-Execute {
    param ($Command)   
    ssh -t $User@$RemoteHost $Command
}

# Configuring Posh-SSH
$secpasswd = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential($User, $secpasswd)

Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
Import-Module Posh-SSH

# Stop service daemon
SSH-Execute '
    sudo systemctl stop RunningWater.service
    rm Desktop/@running-water/src/*
'

# Copy program executable & config.json
Set-SCPItem -ComputerName $RemoteHost -Credential $Credentials -Path "src" -Destination "Desktop/@running-water" -Verbose

# Reload daemon, make file executable & restart service
SSH-Execute '
    sudo systemctl daemon-reload 
    sudo systemctl start RunningWater.service
'