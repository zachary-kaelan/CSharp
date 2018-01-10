namespace Zipwhip
{
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using TaskBarApp;
    using TaskBarApp.Objects;

    public class TextService
    {
        private RestClient client { get; set; }
        private string session { get; set; }
        private static Dictionary<string, string> FileExts = new Dictionary<string, string>()
        {
            {"image/bmp", ".bmp"},
            {"image/gif", ".gif"},
            {"image/png", ".png"},
            {"image/jpeg", ".jpg"}
        };
        private static DateTime UnixStart = new DateTime(0x7b2, 1, 1);

        public TextService(string userName, string password, string countryCode)
        {
            client = new RestClient("https://api.zipwhip.com/");
            RestRequest request = new RestRequest("user/login", Method.GET);
            
            request.AddParameter("password", password);
            request.AddParameter("remember", "true");
            IRestResponse<UserSession> response = client.Execute<UserSession>(request);
            this.session = response.Data.response;
            client.AddDefaultParameter("session", session);
        }

        public bool AccountLogIn(string userName, string password, string countrycode)
        {
            RestRequest request = new RestRequest("user/login", Method.GET);
            request.AddParameter("username", (countrycode != "USA" ? "+1" : "") + userName);
            request.AddParameter("password", password);
            request.AddParameter("remember", "true");
            IRestResponse<UserSession> response = client.Execute<UserSession>(request);
            session = response.Data.response;
            client.AddDefaultParameter("session", session);
            return Convert.ToBoolean(response.Data.success.ToLower());
        }

        public bool AccountValidate(string userName, string password, string countrycode)
        {
            RestRequest request = new RestRequest("user/login", Method.GET);
            request.AddParameter("username", (countrycode != "USA" ? "+1" : "") + userName);
            request.AddParameter("password", password);
            request.AddParameter("remember", "true");
            return client.Execute<UserSession>(request).StatusDescription == "Unauthorized";
        }

        public IRestResponse<ContactBlockResponse> BlockContact(string mobileNumber)
        {
            RestRequest request = new RestRequest("contact/block", Method.GET);
            request.AddParameter("mobileNumber", mobileNumber);
            
            return client.Execute<ContactBlockResponse>(request);
        }

        public IRestResponse<ConversationDeleteResponse> ConversationDelete(string fingerprint)
        {
            RestRequest request = new RestRequest("conversation/delete", Method.GET);
            request.AddParameter("fingerprint", fingerprint);
            
            return client.Execute<ConversationDeleteResponse>(request);
        }

        public IRestResponse<ContactDeleteResponse> DeleteContact(long contactId)
        {
            RestRequest request = new RestRequest("contact/delete", Method.GET);
            request.AddParameter("contact", contactId);
            
            return client.Execute<ContactDeleteResponse>(request);
        }

        public IRestResponse<TextMessageDeleteResponse> DeleteMessage(long messageID)
        {
            RestRequest request = new RestRequest("message/delete", Method.GET);
            request.AddParameter("message", messageID);
            
            return client.Execute<TextMessageDeleteResponse>(request);
        }

        public string FormatFileExt(string mimeType)
        {
            return FileExts[mimeType];
        }

        public string FormatUnixEpochDate(DateTime dateTime)
        {
            return new TimeSpan(dateTime.ToUniversalTime().Ticks - UnixStart.Ticks).TotalSeconds.ToString("#");
        }

        public Image GetAttachment(string storageKey)
        {
            RestRequest request = new RestRequest("hostedContent/get", Method.GET);
            
            request.AddParameter("storageKey", storageKey);
            return Image.FromStream(new MemoryStream(client.Execute<object>(request).RawBytes));
        }

        public MMSImage GetAttachment(string storageKey, long messageID)
        {
            new List<TextMessageAttachment>();
            MMSImage image = new MMSImage();
            RestRequest request = new RestRequest("messageAttachment/list", Method.GET);
            
            request.AddParameter("messageId", messageID);
            IRestResponse<TextMessageAttachmentList> response = client.Execute<TextMessageAttachmentList>(request);
            if ((response != null) && response.Data.success)
            {
                foreach (TextMessageAttachment attachment in response.Data.response)
                {
                    if (attachment.mimeType.Contains("image/") && (attachment.storageKey == storageKey))
                    {
                        image.storageKey = attachment.storageKey;
                        image.mimeType = attachment.mimeType;
                        image.messageID = messageID;
                        image.ext = this.FormatFileExt(attachment.mimeType);
                        RestRequest request2 = new RestRequest("hostedContent/get", Method.GET);
                        request2.AddParameter("session", session);
                        request2.AddParameter("storageKey", image.storageKey);
                        MemoryStream stream = new MemoryStream(client.Execute<object>(request2).RawBytes);
                        image.image = Image.FromStream(stream);
                    }
                }
            }
            return image;
        }

        public IRestResponse<TextMessageAttachmentList> GetAttachmentList(long messageID)
        {
            RestRequest request = new RestRequest("messageAttachment/list", Method.GET);
            
            request.AddParameter("messageId", messageID);
            return client.Execute<TextMessageAttachmentList>(request);
        }

        public IRestResponse<ContactListResponse> GetContactList(string mobileNumber, int page, int pageLimit)
        {
            RestRequest request = new RestRequest("contact/list", Method.GET);
            request.AddParameter("address", "device:" + mobileNumber);
            
            request.AddParameter("page", page);
            request.AddParameter("pageLimit", pageLimit);
            return client.Execute<ContactListResponse>(request);
        }

        public IRestResponse<ConversationGet> GetConversation(string fingerprint, int start, int limit)
        {
            RestRequest request = new RestRequest("conversation/get", Method.GET);
            request.AddParameter("fingerprint", fingerprint);
            
            request.AddParameter("start", start);
            request.AddParameter("limit", limit);
            return client.Execute<ConversationGet>(request);
        }

        public IRestResponse<ConversationList> GetConversationList(int start, int limit)
        {
            RestRequest request = new RestRequest("conversation/list", Method.GET);
            
            request.AddParameter("start", start);
            request.AddParameter("limit", limit);
            return client.Execute<ConversationList>(request);
        }

        public IRestResponse<TextMessageList> GetMessageList(int start, int limit)
        {
            RestRequest request = new RestRequest("message/list", Method.GET);
            
            request.AddParameter("start", start);
            request.AddParameter("limit", limit);
            return client.Execute<TextMessageList>(request);
        }

        public IRestResponse<UpdateGet> GetUpdates(int lastCheck)
        {
            RestRequest request = new RestRequest("session/update", Method.GET);
            request.AddParameter("_dc", lastCheck);
            
            return client.Execute<UpdateGet>(request);
        }

        public IRestResponse<TextMessageReadResponse> MarkMessageRead(long messageID)
        {
            RestRequest request = new RestRequest("message/read", Method.GET);
            request.AddParameter("message", messageID);
            
            return client.Execute<TextMessageReadResponse>(request);
        }

        public IRestResponse<ContactSaveResponse> SaveContact(string formatedContacts, string firstName, string lastName, string notes)
        {
            RestRequest request = new RestRequest("contact/save", Method.GET);
            request.AddParameter("address", formatedContacts);
            
            request.AddParameter("firstName", firstName);
            request.AddParameter("lastName", lastName);
            request.AddParameter("email", "");
            request.AddParameter("phone", "");
            request.AddParameter("city", "");
            request.AddParameter("state", "");
            request.AddParameter("zipcode", "");
            request.AddParameter("notes", notes);
            return client.Execute<ContactSaveResponse>(request);
        }

        public IRestResponse<TextMessageSendResponse> SendMessage(string body, string formatedContacts, DateTime scheduledDate)
        {
            RestRequest request = new RestRequest("message/send", Method.GET);
            char[] trimChars = new char[] { '\n' };
            body = body.Trim().Trim(trimChars);
            body = body.Asciify();
            
            request.AddParameter("contacts", formatedContacts);
            request.AddParameter("body", body);
            if (scheduledDate > DateTime.Now)
                request.AddParameter("scheduledDate", this.FormatUnixEpochDate(scheduledDate));
            return client.Execute<TextMessageSendResponse>(request);
        }

        public IRestResponse<MMSSendResponse> SendMessageMMS(string strMessage, string strToPhone, string strSession, string strFilePath)
        {
            char[] trimChars = new char[] { '\n', '\r' };
            strMessage = strMessage.Trim().Trim(trimChars);
            strMessage = strMessage.Asciify();
            strToPhone = "+1" + strToPhone;
            string[] textArray1 = new string[] { "messaging/send?session=", strSession, "&to=", strToPhone, "&body=", strMessage };
            RestRequest request = new RestRequest(string.Concat(textArray1), Method.POST) {
                RequestFormat = DataFormat.Json
            };
            char[] separator = new char[] { '\\' };
            request.AddFile(strFilePath.Split(separator).Last<string>(), strFilePath, null);
            return client.Execute<MMSSendResponse>(request);
        }
    }
}

