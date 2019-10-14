namespace Otc.Caching.Tests
{
    public class User
    {
        public User()
        {

        }

        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
