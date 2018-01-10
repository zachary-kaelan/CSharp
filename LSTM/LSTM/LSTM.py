import copy, numpy as np
np.random.seed(0)

# compute sigmoid nonlinearity

def sig(x, Deriv=False):
    if (Deriv):
        # convert output of sigmoid function to its derivative
        return x*(1-x)
    return 1/(1+np.exp(-x))


# training dataset generation
int_bin = {}

# constants
bin_dim = 8 # binary
i_dim = 2   # input
#h_dim = 16  # hidden
o_dim = 1   # output

alpha = 0.1

maxi = pow(2, bin_dim)
bin = np.unpackbits(
    np.array(
        [range(maxi)],
        dtype=np.uint8).T,
    axis=1)

for i in range(maxi):
    int_bin[i] = bin[i]

# training logic
for h in range(8):
    conDif = list()

    h_dim = pow(2,h)

    # initialize neural network weights
    syn0 = 2*np.random.random((i_dim,h_dim)) - 1    # placing the inputs into the neural network
    synh = 2*np.random.random((h_dim,h_dim)) - 1    # the meat of the neural network
    syn1 = 2*np.random.random((h_dim,o_dim)) - 1    # getting a singular output

    # setting up arrays of 0s with the sizes of the given synapses
    syn0_upd = np.zeros_like(syn0)
    synh_upd = np.zeros_like(synh)
    syn1_upd = np.zeros_like(syn1)

    for j in range(10000):

        # generate a simple addition problem (a + b = c)
        a_int = np.random.randint(maxi/2) # int version
        a = int_bin[a_int] # binary encoding

        b_int = np.random.randint(maxi/2)
        b = int_bin[b_int]

        # true answer
        c_int = a_int + b_int
        c = int_bin[c_int]

        # storage for best guess (binary encoded)
        d = np.zeros_like(c)

        err = 0

        l2_deltas = list()
        l1_values = list()
        l1_values.append(np.zeros(h_dim))

        # moving along the positions in the binary encoding
        for pos in range(bin_dim):

            # generate I/O
            X = np.array([
                [a[bin_dim - pos - 1],
                 b[bin_dim - pos - 1]]
                ])
            y = np.array([
                [c[bin_dim - pos - 1]]
                ]).T

            # hidden layer (input ~+ prev_hidden)
            l1 = sig(
                np.dot(X, syn0) +
                np.dot(l1_values[-1],synh))

            # output layer (new binary representation)
            l2 = sig(np.dot(l1,syn1))

            # did we miss?... if so,by how much?
            l2_err = y - l2
            l2_deltas.append(
                (l2_err) *
                sig(l2,True)
                )
            err += np.abs(l2_err[0])

            # decode estimate so we can print it out
            d[bin_dim - pos - 1] = np.round(l2[0][0])

            # store hidden layer so we can use it in the next timestep
            l1_values.append(copy.deepcopy(l1))

        future_l1_delta = np.zeros(h_dim)

        for pos in range(bin_dim):

            X = np.array([[a[pos],b[pos]]])
            l1 = l1_values[-pos - 1]
            prev_l1 = l1_values[-pos - 2]

            # error at output layer
            l2_delta = l2_deltas[-pos - 1]
            # error at hidden layer
            l1_delta = (future_l1_delta.dot(synh.T) + 
                        l2_delta.dot(syn1.T)) * sig(l1, True)

            # update weights for next try
            syn1_upd += np.atleast_2d(l1).T.dot(l2_delta)
            synh_upd += np.atleast_2d(prev_l1).T.dot(l1_delta)
            syn0_upd += X.T.dot(l1_delta)

            future_l1_delta = l1_delta

        syn0 += syn0_upd * alpha
        synh += synh_upd * alpha
        syn1 += syn1_upd * alpha

        syn0_upd *= 0
        synh_upd *= 0
        syn1_upd *= 0

        conDif.append(err)
        # print out progress
        """
        if (j % 1000 == 0):
            print("Error: " + str(err))
            print("Pred: " + str(d))
            print("True: " + str(c))
            out = 0
            for i,x in enumerate(reversed(d)):
                out += x*pow(2,i)
            print(str(a_int) + " + " + str(b_int) + " = " + str(out))
            print("------------\n")
        """

    #for con in range(len(conDif) - 1, 0, -1):
        #conDif[con] -= conDif[con-1]
    print("Hidden Dimension: " + str(h_dim))
    print("Convergence Rate: " + str(np.std(conDif)))
    print("~~~~~~~~~~~~~~~~~~~~~~~\n")