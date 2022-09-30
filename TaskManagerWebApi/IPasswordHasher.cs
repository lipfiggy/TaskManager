namespace TaskManagerWebApi
{
    public interface IPasswordHasher
    {
        public string GetHashOfAPassword(string password);
    }
}
