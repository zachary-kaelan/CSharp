import numpy as np
import csv
import datetime
import json

now = datetime.datetime.now()
list = []

with open("C:\DocUploads\Program Files\NeuralNetworks\Zillow_Training_Data.csv") as file:
    #reader = csv.reader(file);
    #for row in reader:
    #    temp = [row[i] for i in range(4)]
    #    temp.append(datetime.datetime.strptime(row[4], "%m/%d/%Y"))
    list = json.decoder.JSONDecoder.decode(file.read())


        