
Remove-Item -Path bin\Release\net5.0\publish\RunningWater.Raspberry

$User = "rdp"
$Password = "raspberry"
$RemoteHost = "192.168.1.54"


dotnet publish RunningWater.Raspberry.csproj -p:PublishProfile=FolderProfile


$secpasswd = ConvertTo-SecureString $Password -AsPlainText -Force
$Credentials = New-Object System.Management.Automation.PSCredential($User, $secpasswd)

Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
Import-Module Posh-SSH

Set-SCPItem -ComputerName $RemoteHost -Credential $Credentials -Path bin\Release\net5.0\publish\RunningWater.Raspberry -Destination Desktop -Verbose

IF ($IsWindows)
{
    plink.exe -ssh "$($User)@$($RemoteHost)" -pw $Password -t -batch 'chmod 777 Desktop/./RunningWater.Raspberry ; sudo Desktop/./RunningWater.Raspberry'
}
else
{
    ssh -t rdp@192.168.1.54 'chmod 777 Desktop/./RunningWater.Raspberry ; sudo Desktop/./RunningWater.Raspberry'
}