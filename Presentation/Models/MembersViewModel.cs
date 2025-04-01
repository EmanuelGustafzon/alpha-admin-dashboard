using Domain.Models;

namespace Presentation.Models;

public class MembersViewModel
{
    public IEnumerable<Member> Members { get; set; } = [];
}
