using System.Net;
using System.Net.Mail;

namespace Plant_BiologyEducation.Service
{
    public class EmailService
    {
        private readonly string _fromEmail = "bmduy090404.nvtroi1922@gmail.com"; 
        private readonly string _appPassword = "gwmwplugwbvxuyzc"; 

        public bool SendVerificationCode(string toEmail, string code)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(_fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = "Mã xác thực đặt lại mật khẩu";
                mail.Body = $"Xin chào,\n\nMã xác thực của bạn là: {code}\n\nVui lòng không chia sẻ mã này với bất kỳ ai.";

                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(_fromEmail, _appPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Gửi email thất bại: " + ex.Message);
                return false;
            }
        }
    }
}
