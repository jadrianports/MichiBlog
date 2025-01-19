using MichiBlog.WebApp.Interfaces;
using SkiaSharp;
using System;
using System.Linq;

namespace MichiBlog.WebApp.Services
{
    public class CaptchaService : ICaptchaService
    {
        public (string CaptchaCode, byte[] CaptchaImage) GenerateCaptcha()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var captchaCode = new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());

            using var bitmap = new SKBitmap(300, 100);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.LightGray);

            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,

            };
            var font = new SKFont
            {
                Size = 32,
                Typeface = SKTypeface.FromFamilyName("Verdana")
            };

            var distortionPaint = new SKPaint
            {
                Color = SKColors.Gray,
                StrokeWidth = 2,
                IsAntialias = true
            };

            for (int i = 0; i < 5; i++)
            {
                float startX = random.Next(0, bitmap.Width);
                float startY = random.Next(0, bitmap.Height);
                float endX = random.Next(0, bitmap.Width);
                float endY = random.Next(0, bitmap.Height);
                canvas.DrawLine(startX, startY, endX, endY, distortionPaint);
            }

            // Add noise (random dots)
            for (int i = 0; i < 200; i++)
            {
                float x = random.Next(0, bitmap.Width);
                float y = random.Next(0, bitmap.Height);
                canvas.DrawCircle(x, y, 1, distortionPaint);
            }

            // Draw the CAPTCHA code
            // Draw the CAPTCHA text with slight random offsets for distortion
            var textPositionX = 10f;
            foreach (var character in captchaCode)
            {
                var offsetX = random.Next(-5, 5);
                var offsetY = random.Next(-5, 5);
                canvas.DrawText(character.ToString(), textPositionX + offsetX, 70 + offsetY, SKTextAlign.Left, font, paint);
                textPositionX += 30; // Adjust spacing between characters
            }
            canvas.Flush();

            // Convert to byte array
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return (captchaCode, data.ToArray());
        }
    }
}
