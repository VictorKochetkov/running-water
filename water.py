import serial
import json
import time

class Worker:
    def __init__(self):
        self.port = None

    def isListening(self):
        return self.port is not None and self.port.isOpen

    def tryListen(self):
        try:
            if self.isListening() is False:
                self.port = serial.Serial("/dev/rfcomm0", baudrate=9600, timeout=1)
        except:
            return False
        else:
            return True

    def loop(self):
        while True:

            while self.tryListen() is False:
                time.sleep(5)
            
            while self.isListening():
                value = self.port.read_all()
                if value:
                    self.handle(value)
                else:
                    time.sleep(5)

    def handle(self, value):
        try:
            jsonValue = json.loads(value)
            command = jsonValue['command']
            data = jsonValue['data']

            print(command)
            print(data['test'])
        except:
            self.write('Json parse error')

    def write(self, text):
        self.port.write((text+'\r\n').encode())

Worker().loop()