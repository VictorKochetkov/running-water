const { exec } = require('child_process');

// Command line arguments
const args = require('minimist')(process.argv.slice(2))

// Latency for USB ports power on/off
var latency = parseInt(args['latency']) || 500;

// USB ports power duration
var duration = parseInt(args['duration']) || 3000;

// Actual watering duration
var actualDuration = Math.max(0, duration - latency);

console.log(`Latency = ${latency}`);
console.log(`Duration = ${duration}`);
console.log(`Actual duration = ${actualDuration}`);

new Promise(function(resolve) {
  exec('sudo /home/pi/uhubctl/uhubctl -a on -l 1-1', () => resolve());
})
.then(function() {
  return new Promise((resolve) => setTimeout(() => resolve(), actualDuration));
})
.then(function() {
  return new Promise((resolve) => exec('sudo /home/pi/uhubctl/uhubctl -a off -l 1-1', () => resolve()));
});
