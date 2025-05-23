using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace GetLogsFile;

enum WordPosition
{
  remote_ip = 0,
  empty_dash = 1,
  remote_user = 2,
  time_local = 3,
  request = 4,
  status = 5,
  body_bytes_sent = 6,
  http_referer = 7,
  http_user_agent = 8,
}

public class GetLogs
{
  struct LogLine
  {
    public LogLine(
      string input_remote_addr,
      string input_time_local,
      string input_request,
      int input_status,
      int input_bytes_sent,
      string input_referer,
      string input_user_agent
    )
    {
      remote_addr = input_remote_addr;
      time_local = input_time_local;
      request = input_request;
      status = input_status;
      bytes_sent = input_bytes_sent;
      referer = input_referer;
      user_agent = input_user_agent;
    }

    public string remote_addr { get; }
    public string time_local { get; }
    public string request { get; }
    public int status { get; }
    public int bytes_sent { get; }
    public string referer { get; }
    public string user_agent { get; }
  }

  class LogLines
  {
    public const string linesName = "lines";
    public List<LogLine> lines;
  }

  private string filePath = "README.md";
  private bool isRead = false;
  private int cursorIndex = 0;

  public GetLogs()
  {
    isRead = ReadLogPathFromEnv();
  }

  public string OnGetLogs(string urlParam = "")
  {
    try
    {
      using StreamReader reader = new(filePath);
      string content = reader.ReadToEnd();
      string[] linesInFile = content.Split('\n');

      if (isRead)
      {
        List<LogLine> lineObjectArray = new List<LogLine>();
        for (int i = 0; i < 2; i++)
        {
          if (i == 1)
          {
            using StreamReader reader1 = new(filePath + ".1");
            content = reader1.ReadToEnd();
            linesInFile = content.Split('\n');
          }
          foreach (string line in linesInFile)
          {
            try
            {
              string remote_addr = GetIpAddr(line.Substring(GetCursorIndex()));
              string time_local = GetTime(line.Substring(GetCursorIndex()));
              string request = GetQuotes(line.Substring(GetCursorIndex()));
              int status = ParseInt(GetNumber(line.Substring(GetCursorIndex())));
              int bytes_sent = ParseInt(GetNumber(line.Substring(GetCursorIndex())));
              string referer = GetQuotes(line.Substring(GetCursorIndex()));
              string user_agent = GetQuotes(line.Substring(GetCursorIndex()));

              var logLine = new LogLine(
                remote_addr,
                time_local,
                request,
                status,
                bytes_sent,
                referer,
                user_agent
              );
              lineObjectArray.Add(logLine);
              SetCursorIndex(0);
            }
            catch (ArgumentOutOfRangeException e)
            {
              using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
              ILogger logger = factory.CreateLogger<Program>();
              logger.LogInformation(e.GetType().Name);
              SetCursorIndex(0);
            }
          }
        }
        var logLines = new LogLines() { lines = lineObjectArray };
        return JsonSerializer.Serialize(lineObjectArray);
      }

      string jsonString = JsonSerializer.Serialize(linesInFile);
      return jsonString;
    }
    catch (IOException e)
    {
      return e.GetType().Name;
    }
  }

  private int GetCursorIndex()
  {
    return cursorIndex;
  }

  private void SetCursorIndex(int newIndex)
  {
    cursorIndex = newIndex;
  }

  private bool ReadLogPathFromEnv()
  {
    using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    ILogger logger = factory.CreateLogger<Program>();

    filePath = Environment.GetEnvironmentVariable("DOT_NET_DIR");

    logger.LogInformation("ReadLogPathFromEnv filePath = {}", filePath);
    logger.LogInformation(
      "ReadLogPathFromEnv filePathE = {}",
      Environment.GetEnvironmentVariable("DOT_NET_DIR")
    );

    return !String.IsNullOrEmpty(filePath);
  }

  private string MatchPatternOrGetEmptyDefault(string inputStr, string pattern)
  {
    var match = Regex.Match(inputStr, pattern);

    if (!match.Success)
    {
      return "-";
    }

    SetCursorIndex(GetCursorIndex() + match.Index + match.Length);

    return match.Value;
  }

  private string GetIpAddr(string inputStr)
  {
    string pattern = "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}";
    return MatchPatternOrGetEmptyDefault(inputStr, pattern);
  }

  private string GetTime(string inputStr)
  {
    string pattern = "\\[[^\\]]*\\]";
    return MatchPatternOrGetEmptyDefault(inputStr, pattern);
  }

  private string GetQuotes(string inputStr)
  {
    string pattern = "\"[^\"]*\"";
    return MatchPatternOrGetEmptyDefault(inputStr, pattern).Replace("\"", "");
  }

  private string GetNumber(string inputStr)
  {
    string pattern = "[0-9]+";
    return MatchPatternOrGetEmptyDefault(inputStr, pattern);
  }

  private int ParseInt(string intpuStr)
  {
    try
    {
      var result = Convert.ToInt32(intpuStr);
      return result;
    }
    catch (OverflowException)
    {
      Console.WriteLine("{0} is outside the range of the Int32 type.", intpuStr);
    }
    catch (FormatException)
    {
      Console.WriteLine(
        "The {0} value '{1}' is not in a recognizable format.",
        intpuStr.GetType().Name,
        intpuStr
      );
    }
    return -1;
  }
}
