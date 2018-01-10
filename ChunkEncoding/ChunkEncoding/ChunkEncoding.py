from sys import *
import os
#import string
#from string._string import *

#os.chdir("C:\\Users\ZACH-G~1\AppData\Local\Temp\VantageTracker")
f = open("C:\\Users\ZACH-G~1\AppData\Local\Temp\VantageTracker\RawFile.htm", "r")

encdata = f.read()
f.close()
offset = 0
#newdata = ""
offset = str.find(encdata, "\r\n\r\n") + 4

pdf = open("C:\\Users\ZACH-G~1\AppData\Local\Temp\VantageTracker\decoded.pdf", "w")

encdata = encdata[offset:]
try:
    while (encdata != ''):
        off = int(encdata[:str.find(encdata, "\r\n")], 16)
        if off == 0:
            break
        encdata = encdata[str.find(encdata, "\r\n") + 2:]
        pdf.write(encdata[:off])
        encdata = encdata[off+2:]
except:
    pass

pdf.close()