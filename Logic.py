from crontab import CronTab

jobName = 'running-water-job'
cron = CronTab(user='rdp')

def addJob():
    
    removeJob()

    # Create new job
    job = cron.new(
        command='python3 /home/rdp/Desktop/running-water/Job.py >> /home/rdp/Desktop/running-water/logs.txt 2>&1', 
        comment=jobName)

    job.minute.every(1)

    cron.write()

def removeJob():
    
    # Delete existing job
    jobs = cron.find_comment(jobName)
    
    for job in jobs:
        cron.remove(job)

addJob()