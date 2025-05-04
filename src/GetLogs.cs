using System;
using System.IO;
using System.Text.Json;

namespace GetLogsFile;

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

      if (isRead) { }
      return content;
    }
    catch (IOException e)
    {
      return e.GetType().Name;
    }
  }
}
