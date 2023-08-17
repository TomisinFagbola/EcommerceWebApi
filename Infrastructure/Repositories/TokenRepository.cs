using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using System;

namespace Infrastructure.Repositories
{
    public class TokenRepository : RepositoryBase<Token>, ITokenRepository
    {
        public TokenRepository(AppDbContext context) : base(context)
        {

        }
    }
}
