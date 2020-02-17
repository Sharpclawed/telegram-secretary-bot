using System;
using System.IO;
using Telegram.Bot;

namespace TelegramBotTry1
{
    public static class ContentSaver
    {
        private static readonly string BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");
        //todo можно сохранять файлы в отдельных потоках
        public static string SaveDocument(TelegramBotClient bot, string fileId, string fileName)
        {
            var dirPath = Path.Combine(BaseDirectory, fileName.Substring(0, 15) + " " + fileId.Substring(0, 10));
            Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dirPath, fileName);

            using (var file = File.Open(filePath, FileMode.Create))
            {
                var path = bot.GetFileAsync(fileId).Result.FilePath;
                bot.DownloadFileAsync(path, file).Wait();
            }
            return filePath;
        }
    }
}