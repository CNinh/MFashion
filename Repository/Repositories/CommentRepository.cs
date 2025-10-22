using DataAccessObject;
using DataAccessObject.Model;
using Repository.GenericRepository;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(MFashionStoreDBContext context) : base(context) { }
    }
}
