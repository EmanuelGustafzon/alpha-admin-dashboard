using Data.Entities;
using Domain.Models;

namespace Data.Interfaces;

public interface IMemberRepository : IRepository<MemberEntity, Member>
{
}
