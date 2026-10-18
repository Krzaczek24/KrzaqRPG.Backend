using Krzaq.Converters.EnumToString;
using System.Text.Json.Serialization;

namespace KrzaqRPG.Backend.WebApi.Core.Errors;

public class ErrorModel(ErrorCode code) : Krzaq.Errors.Model.ErrorModel<ErrorCode>(code)
{
    [JsonConverter(typeof(EnumToStringConverter<ErrorCode>))]
    public override ErrorCode Code { get; set; }

    public ErrorModel(ErrorCode code, string message) : this(code)
    {
        Message = message;
    }
}
