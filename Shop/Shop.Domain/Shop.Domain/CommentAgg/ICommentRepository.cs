using Common.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.CommentAgg
{
    public interface ICommentRepository:IBaseRepository<Comment>
    {
        Task DeleteAndSave(Comment comment);
    }
}
