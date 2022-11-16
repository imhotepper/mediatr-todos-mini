
using System.Text.Json.Serialization;


namespace mediatr_todos_mini;

public class BaseRequest
{
    [JsonIgnore]public int UserId { get; set; }
}