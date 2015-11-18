using System;
using System.Diagnostics;
using System.IO;


public class Logger
{
    static Object logLock = new Object();
    private FileStream logFile;
    private string filePath;
    private string source;

    public Logger(string source)
    {
        Logger_(source, "");
    }

    public Logger(string source,string path)
    {
        Logger_(source, path);
    }

    private void Logger_(string source,string path)
    {
        string path_ = path.EndsWith("\\") || path=="" ? path : path + "\\";
        this.source = source;
        this.filePath = path_ + source + ".log";
        if (!EventLog.SourceExists(source))
        {
            EventLog.CreateEventSource(source, "Application");
        }
    }

    public enum LogType
    {
        INFO,
        WARNING,
        ERROR
    }

    public void log(string msg)
    {
        _log(msg, LogType.INFO,0);
    }

    public void log(string msg, LogType logType)
    {
        _log(msg, logType,0);
    }

    public void log(string msg, LogType logType,int eventId)
    {
        _log(msg, logType, eventId);
    }

    private string placeZero(int n)
    {
        string res="";
        if(n<100)
        {
            res = "0" + res;
        }
        if(n<10)
        {
            res = "0" + res;
        }
        res = res + n;
        return res;
    }

    private void _log(string msg, LogType logType,int eventId)
    {
        DateTime utcNow;
        lock (logLock)
        {
            utcNow = DateTime.UtcNow;
            string fileLog = logType + " - [" + utcNow +"."+placeZero(utcNow.Millisecond)+ " UTC]: " + msg;
            string consoleLog = this.source+" - "+logType + ": " + msg;
            logFile = File.Open(filePath, FileMode.Append);
            using (var sw = new StreamWriter(logFile))
            {
                sw.WriteLine(fileLog);
            }
            logFile.Close();
            EventLog.WriteEntry(this.source, msg, getEventType(logType),eventId);
            Console.WriteLine(consoleLog);
        }
    }
    private EventLogEntryType getEventType(LogType logType)
    {
        switch (logType)
        {
            case LogType.INFO:
                return EventLogEntryType.Information;
            case LogType.WARNING:
                return EventLogEntryType.Warning;
            case LogType.ERROR:
                return EventLogEntryType.Error;
            default:
                return EventLogEntryType.Information;
        }
    }
}

