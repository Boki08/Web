using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentApp.Persistance.Repository
{
    public class CommentRepository : Repository<Comment, int>, ICommentRepository
    {
        public CommentRepository(RADBContext context) : base(context)
        {
        }
        protected RADBContext DemoContext { get { return context as RADBContext; } }
        public IEnumerable<Comment> GetServiceComments(int serviceId)
        {
            return DemoContext.Comments.Where(cm => cm.Order.Vehicle.RentServiceId == serviceId);
        }
    }
}