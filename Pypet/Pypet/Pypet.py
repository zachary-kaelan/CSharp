print("Welcome to Pypet!")

cat = {
    'name': 'Fluffy',
    'hungry': True,
    'weight': 9.5,
    'age': 5,
    'photo': '(=^o.o^=)__'
}

mouse = {
    'name': 'Pinky',
    'hungry': False,
    'weight': 1.5,
    'age': 6,
    'photo': '<;3 )~~~~'
}

fish = {
    'name': 'Squirty',
    'hungry': False,
    'weight': 0.75,
    'age': 2,
    'photo': "<`)))><"
}

def feed(pet):
    if pet['hungry'] == True:
        pet['hungry'] = False
        pet['weight'] += 1
    else:
        print("The Pypet is not hungry!")

