using FluentValidation;

namespace LM.Orders.Application.Orders.Create;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Status)
            .Must(s => new[] { "Created", "Paid", "Cancelled" }.Contains(s, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status inválido (use Created, Paid, Cancelled)");
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(i =>
        {
            i.RuleFor(x => x.Product).NotEmpty();
            i.RuleFor(x => x.Quantity).GreaterThan(0);
            i.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
