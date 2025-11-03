using System;
using FluentValidation;

namespace CleanArchitectureTest.Application.Features.Users.Commands.UpdateUserInformation;

public class UpdateUserInformationValidator : AbstractValidator<UpdateUserInformationCommand>
{
    public UpdateUserInformationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(255);
        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
    }
}
