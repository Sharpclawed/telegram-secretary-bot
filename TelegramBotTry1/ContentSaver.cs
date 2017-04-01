﻿using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBotTry1
{
    public static class ContentSaver
    {
        private static readonly string BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");
        public static async Task<string> SaveDocument(TelegramBotClient bot, string fileId, string fileName)
        {
            var dirPath = Path.Combine(BaseDirectory, fileId);
            Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dirPath, fileName);

            using (var file = File.OpenWrite(filePath))
            {
                await bot.GetFileAsync(fileId, file);
            }
            return filePath;
        }

        public static string GetPath(string fileId, string fileName)
        {
            return Path.Combine(BaseDirectory, fileId, fileName);
        }
    }
}