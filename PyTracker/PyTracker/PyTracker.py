import requests
import bs4
import webbrowser as wb
import re
from robobrowser import RoboBrowser

browser = RoboBrowser(history=False)
browser.open('https://myvantagetracker.com/login')
#userbox, passbox = browser.get_form(id="form_username"), browser.get_form(id="form_password")


form = browser.get_form(action='/login_check')
form
form['form[username]'].value = 'zac.johnso'
form['form[password]'].value = 'I15Zac$0208'
browser.submit_form(form)

branch = browser.get_form(id="office_select")
print(branch['office_select[office]'].options)
branch['office_select[office]'].value = branch['office_select[office]'].options[12]
browser.submit_form(branch)

browser.open('https://myvantagetracker.com/message-board')
search = browser.select('.nav-search-input')
search.value = 'Julie Ostrander'
searches = browser.select('.dropdown-menu')
browser.follow_link(searches[0])

noteTable = browser.find(id='notes-table')
print(noteTable)
notes = noteTable(style='word-wrap: break-word')
print(notes)

def login(br, cred):
    form = br.get_form(method='post')
    for i in range(cred.count):
        print(form[i].value)
        form[i].value = cred[i]
    br.submit_form(form)
    return br
