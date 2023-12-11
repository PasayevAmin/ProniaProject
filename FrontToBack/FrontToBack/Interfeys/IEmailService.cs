namespace FrontToBack.Interfeys
{
    public interface IEmailService
    {
         Task SendEmailAsync(string emailto, string body, string subject, bool ishtml = false);
       
    }
}
