import requests
import json
import ngrok

#LOGIN = {'application/x-www-form-urlencoded':{'grant_type':'password',
#         'username':'pestpacapi@insightpest.com',
#         'password':'!Pest6547!'}}
LOGIN = {'grant_type':'password',
         'username':'pestpacapi@insightpest.com',
         'password':'!Pest6547!'}
PARAMS = {'application/x-www-form-urlencoded':'grant_type=password&username=pestpacapi%40insightpest.com&password=!Pest6547!'}
BASE_DATA = {'content-type':'application/x-www-form-urlencoded',
             'authorization':'Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh'}
BASE_URL = 'https://api.workwave.com/pestpac/v1/'

def GetToken():
    BASE_DATA['authorization'] = 'Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh'
    r = requests.post('https://is.workwave.com/oauth2/token?scope=openid', headers=BASE_DATA, data=LOGIN)
    BASE_DATA['authorization'] = 'Bearer ' + r.json()['access_token']

GetToken()
