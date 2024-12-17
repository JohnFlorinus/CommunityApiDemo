using CommunityApiDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunityApiDemo.Interfaces
{
    public interface ICommentRepo
    {
        public Task<List<Comment>> GetComments(int postID);

        public Task<bool> Create(int postID, string content);
    }
}