import os
import time
import json

# Execute command as `sudo` user
# https://stackoverflow.com/a/13045700/6499748
def execute(command): 
    os.popen("sudo -S %s"%(command), 'w').write(password)

# `sudo` user password
password = json.load(open('config.json'))['sudo-password']

# Turn on power on all USB ports 
execute('/home/pi/uhubctl/uhubctl -a on -l 1-1')

# Waiting for water pump to do his job
time.sleep(3)

# Then turn off power on all USB port again
execute('/home/pi/uhubctl/uhubctl -a off -l 1-1')