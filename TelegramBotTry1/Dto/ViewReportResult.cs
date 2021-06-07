﻿using System.Collections.Generic;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Dto
{
    //todo rename
    public class ViewReportResult: ICommandResult
    {
        //todo implicit operators?
        public List<IMessageDataSet> Messages { get; set; }
        public List<string> Records { get; set; }
        public string Error { get; set; }
        public string Caption { get; set; }
        public string Message { get; set; }
    }
}