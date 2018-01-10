import bluetooth as bt
import bluetooth.ble as ble
import sys

def inquiry():
    print("Performing inquiry...")
    devices = ble.discover_devices(30, True, True, True)
    print("found %d devices" % len(devices))
    for addr, name in devices:
        try:
            print("  %s - %s" % (addr, name))
        except UnicodeEncodeError:
            print("  %s - %s" % (addr, name.encode('utf-8', 'replace')))
        findServices(addr)

def findServices(target):
    services = ble.find_service(address=target)
    if len(services) > 0:
        print("found " + len(services) + " services")
        print("")
    else:
        print("no services found")

    for svc in services:
        for k, v in svc.items():
            print(k + ": " + v)
        print("")

inquiry()

"""
cont = find_service("JS_DATA","08590F7E-DB05-467E-8757-72F6FAEB13A5")[0]

for k,v in cont.items():
    print(k, "=", v)

sock = BluetoothSocket(RFCOMM)
sock.connect((cont["host"], cont["port"]))

while True:
    sock.recv(1024)
    for dat in sock.received_data:
        print(dat)
"""