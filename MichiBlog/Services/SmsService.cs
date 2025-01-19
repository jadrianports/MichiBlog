namespace MichiBlog.WebApp.Services
{
    using Twilio;
    using Twilio.Rest.Api.V2010.Account;
    using Twilio.Types;

    public class SmsService
    {
        public SmsService(string accountSid, string authToken)
        {
            TwilioClient.Init(accountSid, authToken);
        }
        public void SendSms(string toPhoneNumber, string fromPhoneNumber, string message)
        {
           MessageResource.Create(
           body: message,
           from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
           to: new Twilio.Types.PhoneNumber(toPhoneNumber)
           );
        }
    }
}
