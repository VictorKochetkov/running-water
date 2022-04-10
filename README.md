# Running Water
Project for automatic plants watering built with Raspberry Pi

<img src="https://user-images.githubusercontent.com/11313401/162638362-a589bd70-db7e-440f-a44c-4ae84e8cfeac.jpeg" width="400"> <img src="https://user-images.githubusercontent.com/11313401/162638372-e85811f5-6869-4dca-8196-b096cd798040.jpeg" width="400">
<img src="https://user-images.githubusercontent.com/11313401/162638375-0a08c129-1cf0-4783-8cdc-c226ebdace41.jpeg" width="400">
<img src="https://user-images.githubusercontent.com/11313401/162638379-94ef9f66-11b5-49d4-af86-8f2529e94a9f.jpeg" width="400">
<img src="https://user-images.githubusercontent.com/11313401/162638384-2a2faa52-25ec-47f8-8e1d-086426a7d202.jpeg" width="400">

# Composition
This project has following software parts:

## Bluetooth GATT server
Written on Node.js using [bleno](https://github.com/noble/bleno) library and hosted as [systemd service](https://github.com/VictorKochetkov/running-water/blob/main/RunningWater.Raspberry/deploy/RunningWater.service) on Raspberry Pi. This provides communication API for mobile app via BLE. 

## Watering job
Node.js [script](https://github.com/VictorKochetkov/running-water/blob/main/RunningWater.Raspberry/src/watering-job.js) scheduled as cron job which powers on/off USB ports that pumps connected to. For manage USB ports [uhubctl](https://github.com/mvp/uhubctl) library is used.

## Mobile application (iOS & Android)
Written on Xamarin.Forms for iOS & Android. User-friendly interface to configure watering schedule. Communicates with GATT server via BLE.

## Total hardware 💪 
- Raspberry Pi
- water pump with USB connector & silicone tube
- power bank (optional of course)
