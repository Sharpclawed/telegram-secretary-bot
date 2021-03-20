using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public static class ReportCreator
    {
        //TODO чистилку сделать для темп файла. Плюс учесть многопоточность. МБ помечать файлы, что используются, пока не отправили
        //todo userId - дичь
        public static FileInfo Create(IEnumerable<KeyValuePair<string, List<IMessageDataSet>>> sheetsData, string fileName, string[] colNames)
        {
            var tempFile = new FileInfo((fileName ?? "temp" + Guid.NewGuid()) + ".xls");
            using (var xlPackage = new ExcelPackage(tempFile))
            {
                ClearSheets(xlPackage);

                //TODO выводить всю информацию в один лист с группировками
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
                xlPackage.SaveAs(tempFile);
                return xlPackage.File;
            }
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
                nameof(IMessageDataSet.Date),
                nameof(IMessageDataSet.UserFirstName),
                nameof(IMessageDataSet.UserLastName),
                nameof(IMessageDataSet.Message),
                nameof(IMessageDataSet.UserName),
                nameof(IMessageDataSet.UserId)
            };
        }

        private static string[] GetPossibleColNames()
        {
            return new[]
            {
                nameof(IMessageDataSet.Date),
                nameof(IMessageDataSet.UserFirstName),
                nameof(IMessageDataSet.UserLastName),
                nameof(IMessageDataSet.UserName),
                nameof(IMessageDataSet.UserId),
                nameof(IMessageDataSet.ChatName),
                nameof(IMessageDataSet.ChatId),
                nameof(IMessageDataSet.Message)
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