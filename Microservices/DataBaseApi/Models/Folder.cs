using DataBaseApi.Services;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace DataBaseApi.Models;

public class Folder : BasicClass
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public int AccessId { get; set; } = 2;
    public DateTime? LastChannelsUpdate { get; set; }
    [Column(TypeName = "json")]
    public string? SubChannelsJson { get; set; }
    public int ChannelsCount { get; set; } = 0;
    public string? Color { get; set; } = "#ffffff";
    public string? Icon { get; set; }

    public User User { get; set; }
    public Access Access { get; set; }

    public Folder() { }
    public Folder(Guid userId, string name, string? subChannelsJson = "")
    {
        Id = Guid.NewGuid();
        Name = name;
        UserId = userId;
        if (subChannelsJson != "")
            SetSubChannelsJson(subChannelsJson);
    }

    public void SetSubChannelsJson(string subChannelsJson)
    {
        SubChannelsJson = subChannelsJson;
        LastChannelsUpdate = DateTime.UtcNow; //DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }
}