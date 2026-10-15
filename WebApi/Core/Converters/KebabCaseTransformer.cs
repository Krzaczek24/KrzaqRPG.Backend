using Krzaq.Extensions.String.Notation;

namespace KrzaqRPG.Backend.WebApi.Core.Converters
{
    public class KebabCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            string? text = value?.ToString();

            if (string.IsNullOrEmpty(text))
                return text;

            return text.ToKebabCase();
        }
    }
}
