using FluentValidation.Results;
using Krzaq.Attributes.HttpStatus;
using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.WebApi.Core.Exceptions;
using KrzaqTools.Extensions;
using System.Net;

namespace KrzaqRPG.Backend.WebApi.Core.Errors;

public class ErrorHandler : IRequestErrorsHandler
{
    public Task<Exception> Handle(IReadOnlyCollection<ValidationFailure> errors, CancellationToken cancellationToken = default)
    {
        var exceptionErrors = Convert(errors);
        var httpStatus = exceptionErrors.Max(x => x.Code.GetAttribute<HttpStatusAttribute>()?.Code) ?? HttpStatusCode.InternalServerError;

        Exception exception = httpStatus switch
        {
            HttpStatusCode.BadRequest => new BadRequestException(exceptionErrors),
            HttpStatusCode.Unauthorized => new UnauthorizedException(),
            HttpStatusCode.Forbidden => new ForbiddenException(),
            HttpStatusCode.NotFound => new NotFoundException(exceptionErrors),
            HttpStatusCode.Conflict => new ConflictException(exceptionErrors),
            _ => new BadRequestException(exceptionErrors),
        };

        return Task.FromResult(exception);
    }

    private static IEnumerable<ErrorModel> Convert(IReadOnlyCollection<ValidationFailure> errors)
    {
        foreach (var error in errors)
            yield return new ErrorModel(Enum.Parse<ErrorCode>(error.ErrorCode), error.ErrorMessage);
    }
}
