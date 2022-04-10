var util = require('util');
var bleno = require('bleno');
var water = require('./running-water');

function RunningWaterJobsCharacteristic(water) {
  bleno.Characteristic.call(this, {
    uuid: '13333333-3333-3333-3333-333333330002',
    properties: ['read', 'write'],
    descriptors: [
      new bleno.Descriptor({
        uuid: '2901',
        value: 'Gets or sets collection of watering jobs.'
      })
    ]
  });

  this.water = water;
}

util.inherits(RunningWaterJobsCharacteristic, bleno.Characteristic);

RunningWaterJobsCharacteristic.prototype.onWriteRequest = function(data, offset, withoutResponse, callback) {
  if (offset) {
    callback(this.RESULT_ATTR_NOT_LONG);
  }
  else {
    var value = JSON.parse(data.toString());
    console.log(value);

    this.water.setJobs(value.jobs);
    callback(this.RESULT_SUCCESS);
  }
};

RunningWaterJobsCharacteristic.prototype.onReadRequest = function(offset, callback) {
  if (offset) {
    callback(this.RESULT_ATTR_NOT_LONG, null);
  }
  else {
    this.water.getJobs((jobs) => {
      console.log(jobs);
      callback(this.RESULT_SUCCESS, Buffer.from(JSON.stringify(jobs)));
    });
  }
};

module.exports = RunningWaterJobsCharacteristic;