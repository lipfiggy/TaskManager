namespace TaskManagerModels
{
    public class PostUser
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
