using CommunityApiDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunityApiDemo.Interfaces
{
    public interface IPostRepo
    {
        public Task<List<Post>> Search(string? title = null, int? category = null);

        public Task<bool> Create(string title, string content, int categoryid);

        public Task<bool> Update(int postID, string content);

        public Task<bool> Delete(int postID);
    }
}