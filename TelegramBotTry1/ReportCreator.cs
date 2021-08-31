using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using Domain.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace TelegramBotTry1
{
    public static class ReportCreator
    {
        public static FileStream Create(ILookup<string, DomainMessage> sheetsData, [NotNull] string[] colNames)
        {
            //todo unhardcoded path
            var tempFileName = $@"c:\temp\temp-{Guid.NewGuid()}.xls";
            var fileStream = new FileStream(tempFileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read, 4096,
                FileOptions.RandomAccess | FileOptions.DeleteOnClose);
            using (var xlPackage = new ExcelPackage(fileStream))
            {
                var sheetNameCounts = new Dictionary<string, int>();

                foreach (var messagesBySheetName in sheetsData)
                {
                    var messages = messagesBySheetName.ToList();
                    if (messages.Count == 0)
                        continue;
                    var sheetName = NormalizeSheetName(messagesBySheetName.Key);

                    if (sheetNameCounts.ContainsKey(sheetName))
                    {
                        sheetNameCounts[sheetName]++;
                        sheetName = sheetName + "(" + sheetNameCounts[sheetName] + ")";
                    }
                    else
                        sheetNameCounts.Add(sheetName, 1);

                    var worksheet = xlPackage.Workbook.Worksheets.Add(sheetName);
                    worksheet.Row(1).Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Value = NormalizeColumnName("npp");
                    for (var i = 1; i <= colNames.Length; i++)
                        worksheet.Cells[1, i + 1].Value = NormalizeColumnName(colNames[i - 1]);

                    for (var row = 1; row <= messages.Count; row++)
                    {
                        var column = 1;
                        worksheet.Cells[row + 1, column].Value = row;
                        foreach (var colName in colNames)
                        {
                            column++;
                            if (CellMapping.ContainsKey(colName))
                                worksheet.Cells[row + 1, column].Value = CellMapping[colName](messages[row - 1]);
                        }
                    }
                    worksheet.Cells[1, 1, 5, colNames.Length + 1].AutoFitColumns();
                }
                xlPackage.Save();
            }

            fileStream.Position = 0;
            return fileStream;
        }

        private static string NormalizeSheetName(string value)
        {
            var correctValue = value
                .Replace(new[] { '#', '%', '@', '!', '?', '*', '\'' }, "")
                .ToLower();
            return correctValue.Length > 30 ? correctValue.Substring(0, 27) : correctValue; //TODO 27 из-за потенциальных страниц с тем же названием
        }

        private static string NormalizeColumnName(string value)
        {
            return "  " + value + "  ";
        }

        private static Dictionary<string, Func<DomainMessage, string>> CellMapping =
            new()
            {
                {nameof(DomainMessage.Date), message => message.Date.ToString("dd.MM.yy h:mm:ss")},
                {nameof(DomainMessage.UserFirstName), message => message.UserFirstName},
                {nameof(DomainMessage.UserLastName), message => message.UserLastName},
                {nameof(DomainMessage.UserName), message => message.UserName},
                {nameof(DomainMessage.UserId), message => message.UserId.ToString()},
                {nameof(DomainMessage.ChatName), message => message.ChatName},
                {nameof(DomainMessage.ChatId), message => message.ChatId.ToString()},
                {nameof(DomainMessage.Message), message => message.Message}
            };
    }
}