namespace UCS.Extensions.Http.Common.Models
{

    public class HttpSenderOptions 
    {
        public bool ValidateErrorsInResponse { get; set; }
        public JsonParseSettings JsonParseSettings = new JsonParseSettings();
    }
}
