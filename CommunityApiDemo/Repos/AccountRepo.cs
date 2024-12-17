using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CommunityApiDemo.Context;
using CommunityApiDemo.Interfaces;
using CommunityApiDemo.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace CommunityApiDemo.Repos
{
    public class AccountRepo : IAccountRepo
    {
        private readonly DBContext _dbcontext;
        private readonly JWTService _jwt;
        public AccountRepo(DBContext dbcontext, JWTService jwt)
        {
            _dbcontext = dbcontext;
            _jwt = jwt;
        }

        public async Task<string> Login(string name, string password)
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AccountName", name);
                parameters.Add("AccountPassword", password);
                int accountID = await conn.QuerySingleOrDefaultAsync<int>("AccountLogin", parameters, commandType: CommandType.StoredProcedure);
                if (accountID != 0)
                {
                    return await _jwt.CreateJwtToken(accountID, name);
                }
                else
                {
                    return "unauthorized";
                }
            }
        }

        public async Task<bool> Register(string email, string name, string password)
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("AccountEmail", email);
                parameters.Add("AccountName", name);
                parameters.Add("AccountPassword", password);
                bool result = await conn.QuerySingleOrDefaultAsync<bool>("AccountRegister", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<string> ChangePassword(string newpassword)
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("accountID", _jwt.GetJwtClaim("accountid"));
                parameters.Add("newpassword", newpassword);
                await conn.ExecuteAsync("AccountUpdate", parameters, commandType: CommandType.StoredProcedure);
                return "Successfully changed password";
            }
        }

        public async Task<bool> Delete(string password)
        {
            using (IDbConnection conn = _dbcontext.CreateConnection())
            {
                conn.Open();
                var parameters = new DynamicParameters();
                parameters.Add("accountID", _jwt.GetJwtClaim("accountid"));
                parameters.Add("AccountPassword", password);
                bool result = await conn.QuerySingleOrDefaultAsync<bool>("AccountDelete", parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}