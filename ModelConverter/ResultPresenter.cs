using ModelConverter.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModelConverter
{
    public class ResultPresenter
    {
        private const String LogExtension = ".txt";
        private const String LogFileDir = "Logs";
        private const String LogFileName = "ModelConverter-";

        private static String _logDirPath;

        static ResultPresenter()
        {
            _logDirPath = Path.Combine(Environment.CurrentDirectory, LogFileDir);
            Directory.CreateDirectory(_logDirPath);
        }

        private static String LogPath
        {
            get
            {
                return Path.Combine(
                    _logDirPath,
                    LogFileName + DateTime.Now.ToString("yyyy'-'MM'-'dd'-'T'-'HH'-'mm'-'ss") + LogExtension);
            }
        }

        public static ResultBarViewModel ShowResult(IResult result, EventHandler callback)
        {
            ResultBarViewModel resultBar = new ResultBarViewModel();
            resultBar.Message = result.Message;
            if (result.IsSuccess)
            {
                resultBar.MessageStyle = MessageStyle.INFO;
            }
            else
            {
                resultBar.MessageStyle = MessageStyle.ERROR;
                resultBar.LogFilePath = WriteResultToFile(result);
            }
            resultBar.ResultsCleared += callback;
            return resultBar;
        }

        private static String WriteResultToFile(IResult result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ProcessResult(result, stringBuilder);
            String logPath = LogPath;
            File.WriteAllText(logPath, stringBuilder.ToString());
            return logPath;
        }

        private static void ProcessResult(IResult result, StringBuilder stringBuilder, String padding = null)
        {
            if (!String.IsNullOrEmpty(result.ModelPath))
            {
                stringBuilder.Append(padding);
                stringBuilder.Append(result.ModelPath);
                stringBuilder.Append(Environment.NewLine);
                padding += "\t";
            }
            stringBuilder.Append(padding);
            stringBuilder.Append(result.Message);
            stringBuilder.Append(Environment.NewLine);
            if(result.Exception != null)
            {
                stringBuilder.Append(padding);
                stringBuilder.Append(result.Exception.StackTrace);
                stringBuilder.Append(Environment.NewLine);
            }
            foreach(IResult subresult in result.InternalResults)
            {
                ProcessResult(subresult, stringBuilder, padding);
            }
        }
    }
}
