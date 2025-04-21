using Brainbay.RickAndMorty.Application.Dtos.Request;
using Brainbay.RickAndMorty.Domain.Enums;
using FluentValidation;

namespace Brainbay.RickAndMorty.WebApp.Validators;


public class CreateCharacterRequestValidator : AbstractValidator<CreateCharacterRequest>
{
    public CreateCharacterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Species)
            .NotEmpty().WithMessage("Species is required.")
            .MaximumLength(50);

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => status == CharacterStatus.Alive)
            .WithMessage("Status must be Alive.");
    }
}