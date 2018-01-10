import requests
import urllib.request
from bs4 import BeautifulSoup, SoupStrainer

response = urllib.request.urlopen("https://blogs.msdn.microsoft.com/mssmallbiz/2017/07/11/largest-free-microsoft-ebook-giveaway-im-giving-away-millions-of-free-microsoft-ebooks-again-including-windows-10-office-365-office-2016-power-bi-azure-windows-8-1-office-2013-sharepo/")
soup = BeautifulSoup(response, "html.parser", from_encoding=response.info().get_param('charset'))

for link in soup.find_all('a', href=True):
    if link.get_text() == "PDF":
        urllib.request.urlretrieve(link.get('href'), link.parent.previous_sibling.font.get_text() + ".pdf")