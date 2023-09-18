namespace DataBaseApi.Models;

public class Access : BasicClass
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Access() { }
    public Access(AccessEnum @enum)
    {
        Id = (int)@enum;
        Name = @enum.ToString();
    }
    
    public static implicit operator Access(AccessEnum @enum) => new Access(@enum);
    public static implicit operator AccessEnum(Access access) => (AccessEnum)access.Id;
}