using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace TrunkRings
{
    static class ReportCreator
    {
        public static FileStream Create<T>(ILookup<string, T> sheetsData)
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
                    var range = worksheet.Cells["A1"].LoadFromCollection(messages, true);
                    worksheet.Cells[1, 1, 1, range.Columns].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1, 1, range.Columns].Style.Fill.BackgroundColor.SetColor(Color.AliceBlue);
                    worksheet.Cells[1, 1, 1, range.Columns].Style.Font.Bold = true;
                    worksheet.Cells[1, 1, 3, range.Columns].AutoFitColumns(0, 80);
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
    }
}