using Common.Application;
using Common.Application.Validation;
using FluentValidation;
using Shop.Domain.CommentAgg;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Comments.Edit
{
    public record EditCommentCommand(long CommentId, string Text, long UserId) : IBaseCommand;
    public class EditCommentCommandHandler : IBaseCommandHandler<EditCommentCommand>
    {
        private readonly ICommentRepository _repository;

        public EditCommentCommandHandler(ICommentRepository repository)
        {
            _repository = repository;
        }
        public async Task<OperationResult> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            var comment=await _repository.GetTracking(request.CommentId);
            if(comment == null ||comment.UserId!=request.UserId)
                return OperationResult.NotFound();
            comment.Edit(request.Text);
            await _repository.Save();
            return OperationResult.Success();
        }
        public class EditCommentCommandValidator : AbstractValidator<EditCommentCommand>
        {
            public EditCommentCommandValidator()
            {
                RuleFor(r => r.Text)
            .NotNull()
            .MinimumLength(5).WithMessage(ValidationMessages.minLength("متن نظر", 5));
            }
        }
    }
}   
