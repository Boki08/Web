using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class CommentRepository : Repository<Comment, int>, ICommentRepository
    {
        public CommentRepository(DbContext context) : base(context)
        {
        }
        protected RADBContext DemoContext { get { return context as RADBContext; } }
        public IEnumerable<Comment> GetServiceComments(int serviceId)
        {
            return DemoContext.Comments.Where(cm => cm.Order.Vehicle.RentServiceId == serviceId);
        }
        public Comment GetCommentWithOrder(int commentId)
        {
            return DemoContext.Comments.Include(x =>x.Order).Where(x => x.CommentId == commentId).FirstOrDefault();
        }
        //public Comment GetCommentWithOrderUser(int commentId)
        //{
        //    return DemoContext.Comments.Include(x => x.Order).Include(x=>x.User).Where(x => x.CommentId == commentId).FirstOrDefault();
        //}
    }
}