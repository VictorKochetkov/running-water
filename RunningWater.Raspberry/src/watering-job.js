const { exec } = require('child_process');

new Promise(function(resolve) {
  exec('sudo /home/pi/uhubctl/uhubctl -a on -l 1-1', () => resolve());
})
.then(function() {
  return new Promise((resolve) => setTimeout(() => resolve(), 3*1000));
})
.then(function() {
  return new Promise((resolve) => exec('sudo /home/pi/uhubctl/uhubctl -a off -l 1-1', () => resolve()));
});
