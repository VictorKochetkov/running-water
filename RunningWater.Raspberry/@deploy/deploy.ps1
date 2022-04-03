$RemoteHost = '192.168.1.54'
$User = 'pi'
$Password = 'raspberry'

# Execute command on Raspberry
function SSH-Execute {
    param (
        $Command
    )
    
    ssh -t $User@$RemoteHost $Command
}

if (Test-Path -Path bin\Release\net6.0\publish\RunningWater.Raspberry)
{
    # Delete previous build to make sure file will not be trasferred to Raspberry when build or publish failed 
    Remove-Item -Path bin\Release\net6.0\publish\RunningWater.Raspberry
}


# Build binaries
dotnet build RunningWater.Raspberry.csproj -p:Configuration=Release

# Publish binaries into one executable file
dotnet publish RunningWater.Raspberry.csproj -p:PublishProfile=FolderProfile

$secpasswd = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential($User, $secpasswd)

Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
Import-Module Posh-SSH

#Get-SSHTrustedHost | Remove-SSHTrustedHost

# Generate key for Mac ssh connection 
#ssh-keygen -R 192.168.1.54

# Stop service daemon
SSH-Execute 'sudo systemctl stop RunningWater'

# Copy program executable & config.json
Set-SCPItem -ComputerName $RemoteHost -Credential $Credentials -Path bin\Release\net6.0\publish\RunningWater.Raspberry -Destination . -Verbose
Set-SCPItem -ComputerName $RemoteHost -Credential $Credentials -Path bin\Release\net6.0\publish\appsettings.json -Destination . -Verbose

# Reload daemon, make file executable & restart service
SSH-Execute '
    sudo systemctl daemon-reload 
    chmod 777 ./RunningWater.Raspberry 
    sudo systemctl start RunningWater 
'