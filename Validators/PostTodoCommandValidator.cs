using FluentValidation;

namespace mediatr_todos_mini;

public class PostTodoCommandValidator : AbstractValidator<PostTodoCommand>
{
    public PostTodoCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Title).MinimumLength(3);
    }
}