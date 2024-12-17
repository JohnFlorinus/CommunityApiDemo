using System.Data;
using CommunityApiDemo.Context;
using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Models;
using CommunityApiDemo.Services;
using Dapper;

namespace CommunityApiDemo.Repos
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly DBContext _dbcontext;
        public CategoryRepo(DBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<Category>> GetIDs()
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                return (await conn.QueryAsync<Category>("CommentIDs", commandType: CommandType.StoredProcedure)).ToList();
            }
        }
    }
}
