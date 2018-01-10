'use strict';
const express = require('express');
const request = require('request');

//const app = express()
//app.post()

function send(url, method, headers, body = null)
{
    var options = {
        url: url,
        method: method,
        headers: headers,
        form: body
    };
    var info;
    return request(options,
        function (error, response, body) {
            if (!error && res.statusCode == 200) {
                info = { response: res, body: bod };
            }
        }
    );
    return info;

}

var URL = 'https://is.workwave.com/oauth2/token?scope=openid';
var form = {
    'grant_type': 'password',
    'username': 'pestpacapi@insightpest.com',
    'password': '!Pest6547!'
};
var headers = {
    'content-type': 'application/x-www-form-urlencoded',
    'authorization': 'Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh'
};

var response = send(URL, 'POST', headers, form);

console.log(JSON.parse(response.body).access_token);
