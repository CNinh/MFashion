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
    public class CategoryRepository : GenericRepository<ProductCategory>, ICategoryRepository
    {
        public CategoryRepository(MFashionStoreDBContext context) : base(context) { }
    }
}
