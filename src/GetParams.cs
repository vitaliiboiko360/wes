using System.Text.Json;

namespace GetParamsFile;

public class GetParams {
  public string Get(string inputUrlParams) {
    var paramObject = new {
      parameters = inputUrlParams
    };
    string jsonString = JsonSerializer.Serialize(paramObject);
    return jsonString;
  }
}