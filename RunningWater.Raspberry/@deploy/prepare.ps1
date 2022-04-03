
ssh -t pi@192.168.1.54 '
    sudo apt-get install libusb-1.0-0-dev;

    git clone https://github.com/mvp/uhubctl;
    cd uhubctl;
    make;

    sudo make install;

    sudo apt-get install bluez-hcidump
    sudo hcidump
'