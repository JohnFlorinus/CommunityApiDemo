using System.Data;
using CommunityApiDemo.Context;
using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Models;
using CommunityApiDemo.Services;
using Dapper;
using Microsoft.Identity.Client;

namespace CommunityApiDemo.Repos
{
    public class PostRepo : IPostRepo
    {
        private readonly DBContext _db;
        private readonly JWTService _jwt;
        public PostRepo(DBContext db, JWTService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<List<Post>> Search(string? title = null, int? category = null)
        {
            using (IDbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Title", title);
                parameters.Add("CategoryID", category);
                return (await conn.QueryAsync<Post>("PostSearch", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
        }

        public async Task<bool> Create(string title, string content, int categoryid)
        {
            using (IDbConnection conn = _db.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("Title", title);
                parameters.Add("Content", content);
                parameters.Add("CategoryID", categoryid);
                parameters.Add("AccountID", _jwt.GetJwtClaim("accountid"));
                await conn.ExecuteAsync("PostCreate", parameters, commandType: CommandType.StoredProcedure);
                return true;
            }
        }

        public async Task<bool> Update(int postID, string content)
        {
            if (await CheckOwnership(postID) == true)
            {
                using (IDbConnection conn = _db.CreateConnection())
                {
                    conn.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("PostID", postID);
                    parameters.Add("Content", content);
                    await conn.ExecuteAsync("PostUpdate", parameters, commandType: CommandType.StoredProcedure);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> Delete(int postID)
        {
            if (await CheckOwnership(postID) == true)
            {
                using (IDbConnection conn = _db.CreateConnection())
                {
                    conn.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("PostID", postID);
                    await conn.ExecuteAsync("PostDelete", parameters, commandType: CommandType.StoredProcedure);
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
            using (IDbConnection conn = _db.CreateConnection())
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