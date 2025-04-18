using Data.Entities;

namespace Business.Factories;

public static class MemberFactory
{
    public static MemberEntity CreateEntity(string firstName, string lastName, string email)
    {
        return new MemberEntity
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
        };
    }
}
