using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CatalogueRepository : RepositoryBase<Catalogue>, ICatalogueRepository
    {
        public CatalogueRepository(AppDbContext context) : base(context)
        {

        }
    }
}
