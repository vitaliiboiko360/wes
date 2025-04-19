using System.Text.Json;

class GetParams {
  public string Get(string params) {
    var paramObject = new {
      params => params,
    };
    string jsonString = JsonSerializer.Serialize(paramObject);
    return
  }
}