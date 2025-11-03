using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using CleanArchitectureTest.Application.Common.Exceptions;
using CleanArchitectureTest.Contract.Models;
using ValidationException = CleanArchitectureTest.Application.Common.Exceptions.ValidationException;

namespace CleanArchitectureTest.Application.Common.Extentions;

public static partial class GuardClauseExtensions
{
    public static void Duplicate(this IGuardClause guardClause,
                                 bool condition,
                                 string fieldName,
                                 string description = "Giá trị đã tồn tại trong hệ thống",
                                 ErrorCode errorCode = ErrorCode.Duplicated)
    {
        if (condition)
        {
            var failure = new ValidationFailure(fieldName, description)
            {
                ErrorCode = errorCode.ToString()
            };

            throw new ValidationException(new List<ValidationFailure> { failure });
        }
    }

    public static T AppNotFound<T>(
        this IGuardClause guardClause,
        [NotNull][ValidatedNotNull] object key,
        [NotNull][ValidatedNotNull] T? input,
        [CallerArgumentExpression("key")] string? keyName = null)
        where T : class
    {
        if (input is null)
        {
            var cleanKeyName = keyName?.Split('.').Last() ?? "Id";
            throw new AppNotFoundException(typeof(T).Name, cleanKeyName, key);
        }
        return input;
    }

    public static void Forbidden(this IGuardClause guardClause,
                                bool condition,
                                string message = "Access is denied")
    {
        if (condition)
        {
            throw new AppForbiddenException(message);
        }
    }

    public static void BadRequest(this IGuardClause guardClause,
                                 bool condition,
                                 string message)
    {
        if (condition)
        {
            throw new AppBadRequestException(message);
        }
    }
}
