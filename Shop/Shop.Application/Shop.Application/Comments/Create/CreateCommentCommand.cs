using Common.Application;
using Common.Application.Validation;
using FluentValidation;
using Shop.Domain.CommentAgg;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Comments.Create
{
    public record CreateCommentCommand(string Text, long UserId, long ProductId) : IBaseCommand;
    public class CreateCommentCommandHandler : IBaseCommandHandler<CreateCommentCommand>
    {
        private readonly ICommentRepository _repository;

        public CreateCommentCommandHandler(ICommentRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
           var comment=new Comment(request.UserId, request.ProductId,request.Text);
            _repository.Add(comment);
            await _repository.Save();
            return OperationResult.Success();
        }
        public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
        {
            public CreateCommentCommandValidator()
            {
                RuleFor(r => r.Text)
            .NotNull()
            .MinimumLength(5).WithMessage(ValidationMessages.minLength("متن نظر", 5));
            }
        }
    }
}
