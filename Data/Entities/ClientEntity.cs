namespace Data.Entities;

public class ClientEntity
{
    public int Id { get; set; }

    public ICollection<ProjectEntity> Projects { get; set; } = new List<ProjectEntity>();

    public string Name { get; set; } = null!;
}
