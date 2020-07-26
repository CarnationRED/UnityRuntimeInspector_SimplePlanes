using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
[DefaultExecutionOrder(-10000)]
public abstract class MonoSingletonManager<T> : MonoBehaviour where T : MonoSingletonManager<T>
{
    protected static T instance = null;

    public static T GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>();
        }

        return instance;
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}
/*
————————————————
版权声明：本文为CSDN博主「chu358177」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
原文链接：https://blog.csdn.net/chu358177/java/article/details/81873122*/
[DefaultExecutionOrder(-10000)]
public class DebugLogger : MonoSingletonManager<DebugLogger>
{
    public LogLevel SaveLogLevel;
    public bool IsSameFile;
    Queue<LogItem> m_vLogs;
    FileInfo m_logFileInfo;
    static bool m_isInited;
    string logFolder;
    string logFilename;
    string path;

    private Dictionary<LogType, LogLevel> logTypeLevelDict = null;
    private void Awake()
    {
        if (m_isInited)
        {
            Debug.Log("existed logfile object，break");
            enabled = false;
            Destroy(gameObject);
            return;
        }
        Init();
    }
    private void OnDestroy()
    {
        //Application.logMessageReceived -= OnLogMessage;
        Application.logMessageReceivedThreaded -= OnLogMessageThread;
    }
    void FixedUpdate()
    {
        if (m_isInited)
        {
            this.Refresh(Time.fixedDeltaTime);
        }
    }

    public void Init()
    {
        if (m_isInited)
        {
            return;
        }
        m_isInited = true;
        DontDestroyOnLoad(gameObject);
        logTypeLevelDict = new Dictionary<LogType, LogLevel>() {
                { LogType.Log, LogLevel.LOG },
                { LogType.Warning, LogLevel.WARNING },
                { LogType.Assert, LogLevel.ERROR },
                { LogType.Error, LogLevel.ERROR },
                { LogType.Exception, LogLevel.ERROR }};
        // 创建文件
        DateTime timeNow = DateTime.Now;
        logFolder = Application.dataPath + "/..";
        if (IsSameFile)
        {
            path = logFolder + "/Log.txt";
        }
        else
        {
            path = logFolder + "/Log" + timeNow.ToString("yyyyMMddHHmmss") + ".txt";
        }

        m_logFileInfo = new FileInfo(path);
        var sw = m_logFileInfo.CreateText();
        sw.WriteLine("[{0}] - {1}", Application.productName, timeNow.ToString("yyyy/MM/dd HH:mm:ss"));
        sw.Close();
        Debug.Log("Log File Started：" + path);

        // 注册回调
        m_vLogs = new Queue<LogItem>();
        //Application.logMessageReceived += OnLogMessage;
        Application.logMessageReceivedThreaded += OnLogMessageThread;
        Debug.Log("Start Log Output");
    }


    public void Refresh(float dt)
    {
        if (m_vLogs.Count > 0)
        {
            try
            {
                var sw = m_logFileInfo.AppendText();
                var item = m_vLogs.Peek(); // 取队首元素但先不移除
                var timeStr = item.time.ToString("HH:mm:ss.ff");
                var logHeadStr = string.Format("{0}-[{1}] ", timeStr, item.logType);
                string logMsg;
                //if (item.logType.Equals(LogType.Error))
                if (logTypeLevelDict[item.logType].Equals(LogLevel.ERROR))
                {
                    logMsg = string.Format("{0}\n==>==>==>\n{1}", item.messageString, item.stackTrace);
                }
                else
                    logMsg = item.messageString;
                MultiLineLog(logHeadStr, logMsg, sw);
                sw.Close();
                m_vLogs.Dequeue(); // 成功执行了再移除队首元素
            }
            catch (IOException ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
    void MultiLineLog(string header, string message, StreamWriter sw)
    {
        if (message.IndexOf('\n') >= 0)
        {
            var lines = message.Split(Environment.NewLine.ToCharArray());
            for (int i = 0; i < lines.Length; i++)
                sw.WriteLine(string.Format("{0}{1}", header, lines[i]));
        }
        else
            sw.WriteLine(string.Format("{0}{1}", header, message));
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (logTypeLevelDict[type] >= SaveLogLevel)
        {
            m_vLogs.Enqueue(new LogItem()
            {
                messageString = condition,
                stackTrace = stackTrace,
                logType = type,
                time = DateTime.Now
            });
        }
    }

    void OnLogMessageThread(string condition, string stackTrace, LogType type)
    {
        if (logTypeLevelDict[type] >= SaveLogLevel)
        {
            m_vLogs.Enqueue(new LogItem()
            {
                messageString = condition,
                stackTrace = stackTrace,
                logType = type,
                time = DateTime.Now
            });
        }
    }
}
