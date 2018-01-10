ships = list()
for n in range(int(input())):
    ships.append([int(n) for n in input().split()])

def runSim(tf, rate0, drate, shipArgs):
    steps = 100
    dt = 1/steps
    t = 0

    ship = Ship(shipArgs)
    while(not_landed):
        if t >= tf:
            rate = rate0 + drate * (t - tf)
            rate = min(max(rate, 0), 100)
        else:
            rate = 0

        rate *= dt

        for step in range(steps):
            ship.simStep(rate, dt)
            

        t += 1 # time passed (in seconds)

class Ship():
    MOON = 1737100  # 1737.1km radius
    GRAV = 1.622    # m/s^2 acceleration
    EXHAUST = 2800  # m/s speed
    DT = 0.01       # simulation step
    LANDSPEED = 5   # max landing speed

    def __init__(self, m, mf, h, s):
        self.mass = m   # mass of ship
        self.height = h # current altitude
        self.speed = s  # current speed
        self.fuel = mf  # mass of remaining fuel
        self.landed = False
        self.thrust_weight = EXHAUST / (m + mf)
        #self.firing = False

    def simStep(self, rate, dt):
        self.fuel -= rate
        self.height += EXHAUST * rate * dt

