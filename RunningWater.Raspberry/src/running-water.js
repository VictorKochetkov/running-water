var util = require('util');
var events = require('events');
var crontab = require('crontab');
var parser = require('cron-parser');

function RunningWater() {
  events.EventEmitter.call(this);
}

util.inherits(RunningWater, events.EventEmitter);

RunningWater.prototype.setJobs = function(dates) {
  crontab.load(function(err, crontab) {

    // show all jobs
    var jobs = crontab.jobs();
  
    // remove all existing jobs
    jobs.forEach(function(job) {
      crontab.remove(job);
    });

    // create cron job for each date
    dates.forEach(function(unitTimeSeconds){
      // create with Date
      crontab.create(
        'sudo node /home/rdp/Desktop/@running-water/src/watering-job.js', 
        new Date(unitTimeSeconds * 1000));
    });

    // save changes
    crontab.save(function(err, crontab) {
      
    });

  });
};

RunningWater.prototype.getJobs = function(callback) {
  crontab.load(function(err, crontab) {
    var dates = [];

    crontab.jobs().forEach(function(job) {
      // get cron
      var cron = job.render().match(/((((\d+,)+\d+|(\d+(\/|-)\d+)|\d+|\*) ?){5,7})/)[0];

      // convert cron to date
      var date = new Date(parser.parseExpression(cron).next().toString());

      // convert date to unix time seconds
      dates.push(Math.floor(date.getTime() / 1000))
    });

    callback(dates);
  });
};

module.exports.RunningWater = RunningWater;
