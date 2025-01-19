namespace MichiBlog.WebApp.Interfaces
{
    public interface ICaptchaService
    {
        (string CaptchaCode, byte[] CaptchaImage) GenerateCaptcha();
    }
}
