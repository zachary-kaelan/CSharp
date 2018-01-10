using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using TaskBarApp;
using TaskBarApp.Objects;

namespace Zipwhip
{
	public class TextService
	{
		private string strURL = "https://api.zipwhip.com/";

		public bool AccountLogIn(string userName, string password, string countrycode, ref string session)
		{
			bool result = false;
			if (countrycode != "USA")
			{
				userName = "+1" + userName;
			}
			RestClient arg_5F_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("user/login", Method.GET);
			restRequest.AddParameter("userName", userName);
			restRequest.AddParameter("password", password);
			restRequest.AddParameter("remember", "true");
			IRestResponse<UserSession> restResponse = arg_5F_0.Execute<UserSession>(restRequest);
			session = restResponse.Data.response;
			if (restResponse.Data.success == "True")
			{
				result = true;
			}
			return result;
		}

		public bool AccountValidate(string userName, string password, string countrycode)
		{
			bool result = true;
			if (countrycode != "USA")
			{
				userName = "+1" + userName;
			}
			RestClient arg_5F_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("user/login", Method.GET);
			restRequest.AddParameter("userName", userName);
			restRequest.AddParameter("password", password);
			restRequest.AddParameter("remember", "true");
			if (arg_5F_0.Execute<UserSession>(restRequest).StatusDescription == "Unauthorized")
			{
				result = false;
			}
			return result;
		}

		public IRestResponse<UpdateGet> GetUpdates(string session, int lastCheck)
		{
			RestClient arg_37_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("session/update", Method.GET);
			restRequest.AddParameter("_dc", lastCheck);
			restRequest.AddParameter("session", session);
			return arg_37_0.Execute<UpdateGet>(restRequest);
		}

		public IRestResponse<TextMessageList> GetMessageList(string session, int start, int limit)
		{
			RestClient arg_49_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("message/list", Method.GET);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("start", start);
			restRequest.AddParameter("limit", limit);
			return arg_49_0.Execute<TextMessageList>(restRequest);
		}

		public IRestResponse<ConversationList> GetConversationList(string session, int start, int limit)
		{
			RestClient arg_49_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("conversation/list", Method.GET);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("start", start);
			restRequest.AddParameter("limit", limit);
			return arg_49_0.Execute<ConversationList>(restRequest);
		}

		public IRestResponse<ConversationGet> GetConversation(string fingerprint, string session, int start, int limit)
		{
			RestClient arg_57_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("conversation/get", Method.GET);
			restRequest.AddParameter("fingerprint", fingerprint);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("start", start);
			restRequest.AddParameter("limit", limit);
			return arg_57_0.Execute<ConversationGet>(restRequest);
		}

		public IRestResponse<ContactListResponse> GetContactList(string mobileNumber, string session, int page, int pageLimit)
		{
			RestClient arg_61_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("contact/list", Method.GET);
			restRequest.AddParameter("address", "device:" + mobileNumber);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("page", page);
			restRequest.AddParameter("pageLimit", pageLimit);
			return arg_61_0.Execute<ContactListResponse>(restRequest);
		}

		public MMSImage GetAttachment(string session, string storageKey, long messageID)
		{
			RestClient restClient = new RestClient(this.strURL);
			new List<TextMessageAttachment>();
			MMSImage mMSImage = new MMSImage();
			RestRequest restRequest = new RestRequest("messageAttachment/list", Method.GET);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("messageId", messageID);
			IRestResponse<TextMessageAttachmentList> restResponse = restClient.Execute<TextMessageAttachmentList>(restRequest);
			if (restResponse != null && restResponse.Data.success)
			{
				foreach (TextMessageAttachment current in restResponse.Data.response)
				{
					if (current.mimeType.Contains("image/") && current.storageKey == storageKey)
					{
						mMSImage.storageKey = current.storageKey;
						mMSImage.mimeType = current.mimeType;
						mMSImage.messageID = messageID;
						mMSImage.ext = this.FormatFileExt(current.mimeType);
						RestRequest restRequest2 = new RestRequest("hostedContent/get", Method.GET);
						restRequest2.AddParameter("session", session);
						restRequest2.AddParameter("storageKey", mMSImage.storageKey);
						MemoryStream stream = new MemoryStream(restClient.Execute<object>(restRequest2).RawBytes);
						mMSImage.image = Image.FromStream(stream);
					}
				}
			}
			return mMSImage;
		}

		public Image GetAttachment(string session, string storageKey)
		{
			RestClient arg_32_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("hostedContent/get", Method.GET);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("storageKey", storageKey);
			return Image.FromStream(new MemoryStream(arg_32_0.Execute<object>(restRequest).RawBytes));
		}

		public IRestResponse<TextMessageAttachmentList> GetAttachmentList(string session, long messageID)
		{
			RestClient arg_37_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("messageAttachment/list", Method.GET);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("messageId", messageID);
			return arg_37_0.Execute<TextMessageAttachmentList>(restRequest);
		}

		public IRestResponse<ConversationDeleteResponse> ConversationDelete(string fingerprint, string session)
		{
			RestClient arg_32_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("conversation/delete", Method.GET);
			restRequest.AddParameter("fingerprint", fingerprint);
			restRequest.AddParameter("session", session);
			return arg_32_0.Execute<ConversationDeleteResponse>(restRequest);
		}

		public IRestResponse<ContactSaveResponse> SaveContact(string formatedContacts, string session, string firstName, string lastName, string notes)
		{
			RestClient arg_B0_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("contact/save", Method.GET);
			restRequest.AddParameter("address", formatedContacts);
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("firstName", firstName);
			restRequest.AddParameter("lastName", lastName);
			restRequest.AddParameter("email", "");
			restRequest.AddParameter("phone", "");
			restRequest.AddParameter("city", "");
			restRequest.AddParameter("state", "");
			restRequest.AddParameter("zipcode", "");
			restRequest.AddParameter("notes", notes);
			return arg_B0_0.Execute<ContactSaveResponse>(restRequest);
		}

		public IRestResponse<ContactDeleteResponse> DeleteContact(long contactId, string session)
		{
			RestClient arg_37_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("contact/delete", Method.GET);
			restRequest.AddParameter("contact", contactId);
			restRequest.AddParameter("session", session);
			return arg_37_0.Execute<ContactDeleteResponse>(restRequest);
		}

		public IRestResponse<ContactBlockResponse> BlockContact(string mobileNumber, string session)
		{
			RestClient arg_32_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("contact/block", Method.GET);
			restRequest.AddParameter("mobileNumber", mobileNumber);
			restRequest.AddParameter("session", session);
			return arg_32_0.Execute<ContactBlockResponse>(restRequest);
		}

		public IRestResponse<MMSSendResponse> SendMessageMMS(string strMessage, string strToPhone, string strSession, string strFilePath)
		{
			strMessage = strMessage.Trim().Trim(new char[]
			{
				'\n',
				'\r'
			});
			strMessage = strMessage.Asciify();
			strToPhone = "+1" + strToPhone;
			RestClient restClient = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest(string.Concat(new string[]
			{
				"messaging/send?session=",
				strSession,
				"&to=",
				strToPhone,
				"&body=",
				strMessage
			}), Method.POST);
			restRequest.RequestFormat = DataFormat.Json;
			restRequest.AddFile(strFilePath.Split(new char[]
			{
				'\\'
			}).Last<string>(), strFilePath, null);
			return restClient.Execute<MMSSendResponse>(restRequest);
		}

		public IRestResponse<TextMessageSendResponse> SendMessage(string body, string formatedContacts, string session, DateTime scheduledDate)
		{
			RestClient restClient = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("message/send", Method.GET);
			body = body.Trim().Trim(new char[]
			{
				'\n'
			});
			body = body.Asciify();
			restRequest.AddParameter("session", session);
			restRequest.AddParameter("contacts", formatedContacts);
			restRequest.AddParameter("body", body);
			if (scheduledDate > DateTime.Now)
			{
				string value = this.FormatUnixEpochDate(scheduledDate);
				restRequest.AddParameter("scheduledDate", value);
			}
			return restClient.Execute<TextMessageSendResponse>(restRequest);
		}

		public IRestResponse<TextMessageReadResponse> MarkMessageRead(long messageID, string session)
		{
			RestClient arg_37_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("message/read", Method.GET);
			restRequest.AddParameter("message", messageID);
			restRequest.AddParameter("session", session);
			return arg_37_0.Execute<TextMessageReadResponse>(restRequest);
		}

		public IRestResponse<TextMessageDeleteResponse> DeleteMessage(long messageID, string session)
		{
			RestClient arg_37_0 = new RestClient(this.strURL);
			RestRequest restRequest = new RestRequest("message/delete", Method.GET);
			restRequest.AddParameter("message", messageID);
			restRequest.AddParameter("session", session);
			return arg_37_0.Execute<TextMessageDeleteResponse>(restRequest);
		}

		public string FormatUnixEpochDate(DateTime dateTime)
		{
			DateTime dateTime2 = new DateTime(1970, 1, 1);
			TimeSpan timeSpan = new TimeSpan(dateTime.ToUniversalTime().Ticks - dateTime2.Ticks);
			return timeSpan.TotalMilliseconds.ToString("#");
		}

		public string FormatFileExt(string mimeType)
		{
			string result = string.Empty;
			if (mimeType == "image/bmp")
			{
				result = ".bmp";
			}
			else if (mimeType == "image/gif")
			{
				result = ".gif";
			}
			else if (mimeType == "image/png")
			{
				result = ".png";
			}
			else if (mimeType == "image/jpeg")
			{
				result = ".jpg";
			}
			return result;
		}
	}
}
