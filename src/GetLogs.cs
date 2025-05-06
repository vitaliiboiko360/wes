using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
  class LogLines
  {
    public const string linesName = "lines";
    public List<Dictionary<string, string>> lines;
  }

  private string filePath = "README.md";
  private bool isRead = false;

  public GetLogs()
  {
    isRead = ReadLogPathFromEnv();
  }

  public string OnGetLogs(string urlParam = "")
  {
    try
    {
      using StreamReader reader = new(Environment.GetEnvironmentVariable("DOT_NET_DIR"));
      string content = reader.ReadToEnd();
      string[] linesInFile = content.Split('\n');

      if (isRead)
      {
        List<Dictionary<string, string>> lineObjectArray = new List<Dictionary<string, string>>();
        foreach (string line in linesInFile)
        {
          string[] wordsOfLine = line.Split(' ');
          int wordIndex = 0;
          Dictionary<string, string> map = new Dictionary<string, string>();
          foreach (string word in wordsOfLine)
          {
            if (wordIndex != (int)WordPosition.empty_dash)
            {
              map.Add(((WordPosition)wordIndex).ToString(), word);
            }
            wordIndex++;
          }
          lineObjectArray.Add(map);
        }
        var logLines = new LogLines() { lines = lineObjectArray };
        return JsonSerializer.Serialize(logLines);
      }

      string jsonString = JsonSerializer.Serialize(linesInFile);
      return jsonString;
    }
    catch (IOException e)
    {
      return e.GetType().Name;
    }
  }

  private bool ReadLogPathFromEnv()
  {
    using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    ILogger logger = factory.CreateLogger<Program>();

    filePath = Environment.GetEnvironmentVariable("DOT_NET_DIR");

    logger.LogInformation("ReadLogPathFromEnv filePath = {}", filePath);
    logger.LogInformation("ReadLogPathFromEnv filePathE = {}", Environment.GetEnvironmentVariable("DOT_NET_DIR"));

    return !String.IsNullOrEmpty(filePath);
  }
}
