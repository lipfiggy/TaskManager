namespace TaskManagerModels
{
    public class GroupUser
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
        public bool IsCreator { get; set; }
    }
}
