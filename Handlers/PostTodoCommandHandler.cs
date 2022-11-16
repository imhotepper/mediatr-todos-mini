using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace mediatr_todos_mini;


public class PostTodoCommand:BaseRequest, IRequest<Todo>{
    public string Title { get; set; }
}


public record PostTodoCommandHandler : IRequestHandler<PostTodoCommand, Todo>
{
    TodosService _service;

    public PostTodoCommandHandler(TodosService service) => _service = service;

    public Task<Todo> Handle(PostTodoCommand request, CancellationToken cancellationToken)
        => Task.FromResult(_service.Add(request.Title));
}