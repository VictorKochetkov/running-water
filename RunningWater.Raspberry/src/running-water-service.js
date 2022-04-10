var util = require('util');
var bleno = require('bleno');

var RunningWaterJobsCharacteristic = require('./running-water-jobs-characteristic');

function RunningWaterService(water) {
    bleno.PrimaryService.call(this, {
        uuid: '13333333-3333-3333-3333-333333333337',
        characteristics: [
            new RunningWaterJobsCharacteristic(water),
        ]
    });
}

util.inherits(RunningWaterService, bleno.PrimaryService);

module.exports = RunningWaterService;
