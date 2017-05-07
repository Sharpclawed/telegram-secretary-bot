using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace TelegramBotTry1
{
    public static class FileCreator
    {
        public static string SendFeedback(Dictionary<long, List<MessageDataSet>> messageDataSets, long userId)
        {
            var tempFile = new FileInfo("temp" + userId + ".xls");
            using (var xlPackage = new ExcelPackage(tempFile))
            {
                ClearSheets(xlPackage);

                foreach (var dataSetKeyValuePair in messageDataSets)
                {
                    var messageDataSetList = dataSetKeyValuePair.Value;
                    if (messageDataSetList.Count == 0) continue;
                    var messageDataSet = messageDataSetList.OrderBy(x => x.Date).ToArray();

                    //TODO выводить всю информацию в один лист
                    var chatName = GetExcelSheetCorrectName(messageDataSet[messageDataSet.Length - 1].ChatName);
                    var worksheet = xlPackage.Workbook.Worksheets.Add(chatName);
                    var properties = new[]
                    {
                        " npp ", "  Date  ", " UserFirstName  ", " UserLastName  ", " Message ", "  UserName  ", " UserID "
                    };
                    for (var i = 0; i < properties.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = properties[i];
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    }

                    for (var index = 1; index <= messageDataSet.Length; index++)
                    {
                        worksheet.Cells[index + 1, 1].Value = index;
                        worksheet.Cells[index + 1, 2].Value = messageDataSet[index - 1].Date;
                        worksheet.Cells[index + 1, 2].Style.Numberformat.Format = "dd-mm-yy h:mm:ss";
                        worksheet.Cells[index + 1, 3].Value = messageDataSet[index - 1].UserFirstName;
                        worksheet.Cells[index + 1, 4].Value = messageDataSet[index - 1].UserLastName;
                        worksheet.Cells[index + 1, 5].Value = messageDataSet[index - 1].Message;
                        worksheet.Cells[index + 1, 6].Value = messageDataSet[index - 1].UserName;
                        worksheet.Cells[index + 1, 7].Value = messageDataSet[index - 1].UserId;
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }
                xlPackage.SaveAs(tempFile);
                return xlPackage.File.Name;
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

        private static string GetExcelSheetCorrectName(string value)
        {
            var correctValue = value.Replace(new[] { '#', '%', '@', '!', '?', '*', '\'' }, "");
            return correctValue.Length > 30 ? correctValue.Substring(1, 30) : correctValue;
        }
    }
}