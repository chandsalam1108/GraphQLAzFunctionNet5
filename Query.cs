using HotChocolate;
using HotChocolate.Data;
using GraphQLFunction.Models;
using GraphQLFunction.Services;
using System.Collections.Generic;

namespace GraphQLFunction
{
    public class Query
    {
        public User GetProfile([Service]IUserRepository repository, int id) => repository.GetProfile(id);

        [UseFiltering]
        [UseSorting]
        public ICollection<User> GetProfiles([Service] IUserRepository repository) => repository.GetProfiles();
    }
}