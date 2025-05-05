using System;
using System.IO;
using System.Text.Json;

namespace GetLogsFile;

enum WordPosition
{
  remote_ip = 0,
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
  private string filePath;
  private bool isRead;

  public GetLogs()
  {
    filePath = Environment.GetEnvironmentVariable("DOT_NET_DIR");
    if (!String.IsNullOrEmpty(filePath))
    {
      filePath = "README.md";
      isRead = false;
    }
    else
    {
      isRead = true;
    }
  }

  public string OnGetLogs(string urlParam)
  {
    try
    {
      using StreamReader reader = new(filePath);
      string content = reader.ReadToEnd();
      string[] linesInFile = content.Split('\n');

      if (isRead)
      {
        foreach (string line in linesInFile)
        {
          string[] wordsOfLine = line.Split(' ');
          int wordIndex = 0;
          foreach (string word in wordsOfLine)
          {
            wordIndex++;
          }
        }

        string jsonString = JsonSerializer.Serialize(linesInFile);
        return jsonString;
      }

      string jsonString = JsonSerializer.Serialize(linesInFile);
      return jsonString;
    }
    catch (IOException e)
    {
      return e.GetType().Name;
    }
  }
}
