using GraphQLFunction.Models;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLFunction.Services
{
    public interface IUserRepository
    {
        User GetProfile(int id);

        ICollection<User> GetProfiles();
    }

    public class UserRepository : IUserRepository
    {
        public User GetProfile(int id)
        {
            return GetProfiles().Where(i => i.Id == id).FirstOrDefault();
        }

        public ICollection<User> GetProfiles()
        {
            return new List<User>()
            {
                new User()
                {
                    Id = 1,
                    Title = "Mr",
                    LastName = "Chand",
                    FirstName = "Abdul Salam"
                },
                new User()
                {
                    Id = 2,
                    Title = "Mr",
                    LastName = "Shaik",
                    FirstName = "Mohammed Umar"
                },
                new User()
                {
                    Id = 3,
                    Title = "Mr",
                    LastName = "Shaikh",
                    FirstName = "Faraaz"
                },
                new User()
                {
                    Id = 4,
                    Title = "Mr",
                    LastName = "Syed",
                    FirstName = "Nawaz"
                }
            };
        }
    }
}