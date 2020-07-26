using System;
using UnityEngine;

public struct LogItem
{
    /// <summary>
    /// 日志内容
    /// </summary>
    public string messageString;

    /// <summary>
    /// 调用堆栈
    /// </summary>
    public string stackTrace;

    /// <summary>
    /// 日志类型
    /// </summary>
    public LogType logType;

    /// <summary>
    /// 记录时间
    /// </summary>
    public DateTime time;
}
