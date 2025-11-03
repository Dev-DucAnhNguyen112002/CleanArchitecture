using FluentValidation;

namespace CleanArchitectureTest.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(255);
    }
}
