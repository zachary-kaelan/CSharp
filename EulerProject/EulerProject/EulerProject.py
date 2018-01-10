import numpy as np
np.random.seed(1)

L = 1800

class division:
  def __init__(self, n):
    self.n = n
    self.boats = np.array()
    for b in range(self.n):
      self.boats[b] = 40 * b
      
  def move(self):
     speeds = -np.log10(np.random.uniform(0, 1.0, (1,13)))
     
      
    
class boat:
  def __init__(self, x):
    self.riverXPos = x
    self.divPos = 0
    self.bumped = None

def fac(n):
    if n == 1:
        return 1
    else:
        return n * fac(n-1)

def div(n, sizeLim=True, printAll=False):
    pairs = list()
    upper = None
    upper = np.sqrt(np.array([n], dtype=int))[0]
    upper += 1.0
    for a in range(1, int(upper)):
        b = int(n // a)
        if (n % b == 0 and ((not sizeLim) or a <= b)):
            pairs.append((a, b))
        if(printAll):
            print((a,b))
    return pairs

def C(n):
    pairs = div(n)
    divs = 0
    for pair in pairs:
        if (len(div(pair[0]))*2 == len(div(pair[1]))*2):
            print(str(pair))
            divs += 1
    return divs

#print(str(C(48)))
#print(str(C(fac(100))))

def CFac(n):
    divs = 0
    set = range(1,n+1)
    pairs = list()
    for i in range(1, n+1):
        for j in range(i+1, n+1):
            pairs.append(set[i], set[j])

#print(str(CFac(100)))
#print(str(C(100)))

counts = [0 for i in range(10)]
for n in range(1, 11):
    print(len(div(n)))
    #counts[len(div(n))] += 1
#print(counts)