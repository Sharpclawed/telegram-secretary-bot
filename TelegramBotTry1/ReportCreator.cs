using System;
using System.Collections.Generic;
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
        public static FileStream Create(IEnumerable<KeyValuePair<string, List<DomainMessage>>> sheetsData, string[] colNames)
        {
            var tempFileName = "temp-" + Guid.NewGuid() + ".xls";
            var fileStream = new FileStream(tempFileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read, 4096,
                FileOptions.RandomAccess | FileOptions.DeleteOnClose);
            using (var xlPackage = new ExcelPackage(fileStream))
            {
                ClearSheets(xlPackage);

                var sheetNameCounts = new Dictionary<string, int>();
                colNames = NormilizeColNames(colNames);
                var colNamesWithIndexes = SetColumnSortNumbers(colNames);

                foreach (var messagesBySheetName in sheetsData)
                {
                    var messages = messagesBySheetName.Value;
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
                    for (var i = 0; i < colNames.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = NormalizeColumnName(colNames[i]);
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    for (var index = 1; index <= messages.Count; index++)
                    {
                        if (colNamesWithIndexes.ContainsKey("npp"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["npp"]].Value = index;
                        if (colNamesWithIndexes.ContainsKey("Date"))
                        {
                            worksheet.Cells[index + 1, colNamesWithIndexes["Date"]].Value = messages[index - 1].Date;
                            worksheet.Cells[index + 1, colNamesWithIndexes["Date"]].Style.Numberformat.Format = "dd-mm-yy h:mm:ss";
                        }
                        if (colNamesWithIndexes.ContainsKey("UserFirstName"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["UserFirstName"]].Value = messages[index - 1].UserFirstName;
                        if (colNamesWithIndexes.ContainsKey("UserLastName"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["UserLastName"]].Value = messages[index - 1].UserLastName;
                        if (colNamesWithIndexes.ContainsKey("Message"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["Message"]].Value = messages[index - 1].Message;
                        if (colNamesWithIndexes.ContainsKey("UserName"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["UserName"]].Value = messages[index - 1].UserName;
                        if (colNamesWithIndexes.ContainsKey("UserId"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["UserId"]].Value = messages[index - 1].UserId;
                        if (colNamesWithIndexes.ContainsKey("ChatName"))
                            worksheet.Cells[index + 1, colNamesWithIndexes["ChatName"]].Value = messages[index - 1].ChatName;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }
                xlPackage.Save();
            }

            fileStream.Position = 0;
            return fileStream;
        }

        private static void ClearSheets(ExcelPackage xlPackage)
        {
            var worksheetsCount = xlPackage.Workbook.Worksheets.Count;
            if (worksheetsCount > 0)
            {
                for (var i = 1; i <= worksheetsCount; i++)
                {
                    xlPackage.Workbook.Worksheets.Delete(1);
                }
            }
        }

        private static string NormalizeSheetName(string value)
        {
            var correctValue = value.Replace(new[] { '#', '%', '@', '!', '?', '*', '\'' }, "");
            return correctValue.Length > 30 ? correctValue.Substring(1, 27) : correctValue; //TODO 27 из-за потенциальных страниц с тем же названием
        }

        private static string NormalizeColumnName(string value)
        {
            return "  " + value + "  ";
        }

        private static string[] GetDefaultColNames()
        {
            return new[]
            {
                nameof(DomainMessage.Date),
                nameof(DomainMessage.UserFirstName),
                nameof(DomainMessage.UserLastName),
                nameof(DomainMessage.Message),
                nameof(DomainMessage.UserName),
                nameof(DomainMessage.UserId)
            };
        }

        private static string[] GetPossibleColNames()
        {
            return new[]
            {
                nameof(DomainMessage.Date),
                nameof(DomainMessage.UserFirstName),
                nameof(DomainMessage.UserLastName),
                nameof(DomainMessage.UserName),
                nameof(DomainMessage.UserId),
                nameof(DomainMessage.ChatName),
                nameof(DomainMessage.ChatId),
                nameof(DomainMessage.Message)
            };
        }

        private static string[] NormilizeColNames(string[] colNames)
        {
            if (colNames == null || colNames.Length == 0)
                colNames = GetDefaultColNames();
            var filtered = colNames.Intersect(GetPossibleColNames()).Distinct();
            var result = new List<string> {"npp"};
            result.AddRange(filtered);
            return result.ToArray();
        }

        private static Dictionary<string, int> SetColumnSortNumbers(string[] colNames)
        {
            var result = Enumerable.Range(0, colNames.Length).ToDictionary(x => colNames[x], x => x + 1);
            return result;
        }
    }
}