using System.Data;
using CommunityApiDemo.Context;
using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Models;
using CommunityApiDemo.Services;
using Dapper;
using Microsoft.Identity.Client;

namespace CommunityApiDemo.Repos
{
    public class CommentRepo : ICommentRepo
    {
        private readonly DBContext _dbcontext;
        private readonly JWTService _jwt;
        public CommentRepo(DBContext dbcontext, JWTService jWTContext)
        {
            _dbcontext = dbcontext;
            _jwt = jWTContext;
        }

        public async Task<List<Comment>> GetComments(int postID)
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("PostID", postID);
                return (await conn.QueryAsync<Comment>("CommentGet", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task<bool> Create(int postID, string content)
        {
            if (!await CheckOwnership(postID))
            {
                using (IDbConnection conn = _dbcontext.CreateConnection())
                {
                    conn.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("PostID", postID);
                    parameters.Add("Content", content);
                    parameters.Add("AccountID", _jwt.GetJwtClaim("accountid"));
                    await conn.ExecuteAsync("CommentCreate", parameters, commandType: CommandType.StoredProcedure);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> CheckOwnership(int postID)
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("PostID", postID);
                int postaccountid = await conn.QuerySingleOrDefaultAsync<int>("PostAccountID", parameters, commandType: CommandType.StoredProcedure);
                int currentaccountid = Convert.ToInt32(_jwt.GetJwtClaim("accountid"));
                if (postaccountid == currentaccountid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}