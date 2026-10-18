using FluentValidation;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using KrzaqRPG.Backend.WebApi.Core.Extensions;
using KrzaqTools.Extensions;

namespace KrzaqRPG.Backend.WebApi.Core.Extensions;

public static class FluentValidationExtension
{
    public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(this IRuleBuilderOptions<T, TProperty> builder, ErrorCode errorCode, params IEnumerable<object> @params)
    {
        return builder
            .WithErrorCode(errorCode.ToString())
            //.WithMessage(string.Format(errorCode.GetDescription()!, [.. @params]));
            .WithMessage(errorCode.GetDescription());
    }

    public static IRuleBuilderOptions<T, TProperty> WithErrorCodeAndMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> builder, ErrorCode errorCode, string message)
    {
        return builder
            .WithErrorCode(errorCode.ToString())
            .WithMessage(message);
    }
}
