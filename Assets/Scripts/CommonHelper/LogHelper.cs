//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ColaFramework
{
    /// <summary>
    /// Unity日志助手类
    /// </summary>
    public class LogHelper : MonoBehaviour
    {
        /// <summary>
        /// 日志文件的输出文件夹
        /// </summary>
        public static string outputPath;

        /// <summary>
        /// 日志文件的完整保存路径(包括文件名+后缀)
        /// </summary>
        public static string filePath;

        /// <summary>
        /// 日志文件的名称
        /// </summary>
        public static string fileName = "gamelog.txt";

        private static Text textComponent;
        private static StringBuilder stringBuilder;

        private void Awake()
        {
            outputPath = Path.Combine(AppConst.AssetPath, "logs");
            filePath = Path.Combine(outputPath, fileName);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
#if SHOW_SCREEN_LOG
            stringBuilder = new StringBuilder();
#endif
            LogSysInfo();
        }

        /// <summary>
        /// LogCallback，Unity的Log回调
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public void LogCallback(string condition, string stackTrace, LogType type)
        {
            //todo:这里可以加一些过滤条件
            LogFileReport(condition, stackTrace, type);
        }

        /// <summary>
        /// 关联Log写到的Text组件
        /// </summary>
        /// <param name="text"></param>
        public void AttachScreenText(Text text)
        {
            textComponent = text;
            //关联后同步一下log
            if (null != textComponent)
            {
                textComponent.text = stringBuilder.ToString();
            }
        }

        public void UnAttachScreenText()
        {
            textComponent = null;
        }

        /// <summary>
        /// 清空Text组件的Log内容
        /// </summary>
        public void ClearSreenLog()
        {
            if (null != stringBuilder)
            {
                stringBuilder.Length = 0;
            }
            if (null != textComponent)
            {
                textComponent.text = "";
            }
        }

        /// <summary>
        /// 向日志文件中写入一条消息
        /// </summary>
        /// <param name="message"></param>
        private static void WriteLog(string message)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                string logContent = string.Format("{0:G}: {1}", System.DateTime.Now, message);
                sw.Write(logContent);

                // write to screen
#if SHOW_SCREEN_LOG
                stringBuilder.Append(logContent);
                if (null != textComponent)
                {
                    textComponent.text = stringBuilder.ToString();
                }
#endif
            }
        }

        /// <summary>
        /// LogFileReport的内部实现
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void LogFileReport(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Log:
                    WriteLog(string.Format("[log] {0} \r\n", condition));
                    break;
                case LogType.Warning:
                    WriteLog(string.Format("[warning] {0} \r\n", condition));
                    break;
                case LogType.Exception:
                    WriteLog(string.Format("[exception] {0}: \r\n{1}", condition, stackTrace));
                    break;
                case LogType.Error:
                    WriteLog(string.Format("[error] {0}: \r\n{1}", condition, stackTrace));
                    break;
                default:
                    WriteLog(string.Format("[unknow] {0} \r\n", condition));
                    break;
            }
        }

        /// <summary>
        /// 向日志中记录当前设备系统信息
        /// </summary>
        private void LogSysInfo()
        {
            WriteLog(string.Format(string.Format("[{0}] {1}", "sysinfo", CommonHelper.GetDeviceInfo())));
        }
    }
}
