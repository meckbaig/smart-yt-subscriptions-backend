using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseApi.Models;

public class User : BasicClass
{
    public Guid Id { get; set; }
    public string YoutubeId { get; set; }
    public int RoleId { get; set; } = 1;
    public string Email { get; set; }
    public DateTime? LastChannelsUpdate { get; set; }
    [Column(TypeName = "json")]
    public string? SubChannelsJson { get; set; }

    public Role Role { get; set; }
    public List<Folder> Folders { get; set; }
    
    public User() { }
    public User(string youtubeId, string email)
    {
        Id = Guid.NewGuid();
        YoutubeId = youtubeId;
        Email = email;
    }

    public void SetSubChannelsJson(string subChannelsJson)
    {
        SubChannelsJson = subChannelsJson;
        LastChannelsUpdate = DateTime.UtcNow; //DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }
}