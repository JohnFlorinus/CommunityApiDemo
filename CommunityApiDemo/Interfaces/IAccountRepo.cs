namespace CommunityApiDemo.Interfaces
{
    public interface IAccountRepo
    {
        public Task<bool> Register(string email, string name, string password);

        public Task<string> Login(string name, string password);

        public Task<string> ChangePassword(string newpassword);

        public Task<bool> Delete(string password);
    }
}
