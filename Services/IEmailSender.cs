namespace RCMS_web.Services
{
    public interface IEmailSender // don't forget the public modifier
    {
        void Send(string toAddress, string firstname, string subject, string body, bool sendAsync = true);

    }
}