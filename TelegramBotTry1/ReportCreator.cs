using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public static class ReportCreator
    {
        //TODO чистилку сделать для темп файла. Плюс учесть многопоточность. МБ помечать файлы, что используются, пока не отправили
        public static FileInfo Create(IEnumerable<KeyValuePair<string, List<IMessageDataSet>>> sheetsData, long userId)
        {
            var tempFile = new FileInfo("temp" + userId + ".xls");
            using (var xlPackage = new ExcelPackage(tempFile))
            {
                ClearSheets(xlPackage);

                //TODO выводить всю информацию в один лист с группировками
                var sheetNameCounts = new Dictionary<string, int>();

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
                    var properties = new[]
                    {
                        " npp ", "  Date  ", " UserFirstName  ", " UserLastName  ", " Message ", "  UserName  ", " UserID ", " ChatName "
                    };
                    for (var i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = properties[i];
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    for (var index = 1; index <= messages.Count; index++)
                    {
                        worksheet.Cells[index + 1, 1].Value = index;
                        worksheet.Cells[index + 1, 2].Value = messages[index - 1].Date;
                        worksheet.Cells[index + 1, 2].Style.Numberformat.Format = "dd-mm-yy h:mm:ss";
                        worksheet.Cells[index + 1, 3].Value = messages[index - 1].UserFirstName;
                        worksheet.Cells[index + 1, 4].Value = messages[index - 1].UserLastName;
                        worksheet.Cells[index + 1, 5].Value = messages[index - 1].Message;
                        worksheet.Cells[index + 1, 6].Value = messages[index - 1].UserName;
                        worksheet.Cells[index + 1, 7].Value = messages[index - 1].UserId;
                        worksheet.Cells[index + 1, 8].Value = messages[index - 1].ChatName;
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
    }
}