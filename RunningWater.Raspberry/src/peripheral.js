"use strict";

// Execute bash commands
const { exec } = require('child_process');

// Power off all USB ports by default
exec('sudo /home/pi/uhubctl/uhubctl -a off -l 1-1');

// Disable HDMI
exec('sudo tvservice -o');

// Enable 'power save' mode
exec('echo \"powersave\"| sudo tee /sys/devices/system/cpu/cpu0/cpufreq/scaling_governor');

// Disable bluetooth discovery timeout
exec('sudo sed -i \'s /^#DiscoverableTimeout = .* /DiscoverableTimeout = 0/\' /etc/bluetooth/main.conf');

// Turn on bluetooth
exec('sudo bluetoothctl power on');

// Make bluetooth discoverable
exec('sudo bluetoothctl discoverable on');

// Disable pairing to prevent system popup displaying on mobile app
exec('sudo bluetoothctl pairable off');

// Disable pairing to prevent system popup displaying on mobile app
exec('sudo bluetoothctl agent NoInputNoOutput');

//
// Require bleno peripheral library.
// https://github.com/sandeepmistry/bleno
//
var bleno = require('bleno');

//
// Running water logic
//
var water = require('./running-water');

//
// The BLE Running Water Service!
//
var RunningWaterService = require('./running-water-service');

//
// A name to advertise our Running Water Service.
//
var name = 'Running Water';

var runningWaterService = new RunningWaterService(new water.RunningWater());

//
// Wait until the BLE radio powers on before attempting to advertise.
// If you don't have a BLE radio, then it will never power on!
//
bleno.on('stateChange', function(state) {
  if (state === 'poweredOn') {
    //
    // We will also advertise the service ID in the advertising packet,
    // so it's easier to find.
    //
    bleno.startAdvertising(name, [runningWaterService.uuid], function(err) {
      if (err) {
        console.log(err);
      }
    });
  }
  else {
    bleno.stopAdvertising();
  }
});

bleno.on('advertisingStart', function(err) {
  if (!err) {
    console.log('advertising...');
    //
    // Once we are advertising, it's time to set up our services,
    // along with our characteristics.
    //
    bleno.setServices([
      runningWaterService
    ]);
  }
});
