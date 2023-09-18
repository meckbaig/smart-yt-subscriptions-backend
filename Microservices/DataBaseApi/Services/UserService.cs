using DataBaseApi.Data;
using DataBaseApi.Models;
using Grpc.Core;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataBaseApi.Services;

public class UserService : UserProto.UserProtoBase
{
    private AppDbContext _context;
    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<GetUserDataResponse> GetUserData(GetUserDataRequest request, ServerCallContext context)
    {
        try
        {
            User? user = _context?.Users?.FirstOrDefault(u => u.Email == request.Email);
            //User? user = _context?.Users?.FirstOrDefault(u => u.Email == request.Email && u.Id == request.Id);
            if (user == null)
            {
                user = new User("", request.Email);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (user.YoutubeId != request.YoutubeId && request.YoutubeId != "")
            {
                user.YoutubeId = request.YoutubeId;
                await _context.SaveChangesAsync();
            }
            user.Role = _context.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            user.Folders = _context.Folders.Where(f => f.UserId == user.Id).ToList();

            List<Folder> folders = user?.Folders?.ToList() ?? new List<Folder>();
            StringBuilder foldersJson = new StringBuilder("[");
            foreach (var folder in folders)
            {
                foldersJson.Append(folder.ToJsonString(new[] { "SubChannelsJson" }) + ",");
            }
            foldersJson.Remove(foldersJson.Length - 1, 1);
            if (foldersJson.Length > 0)
                foldersJson.Append("]");
            return new GetUserDataResponse
            {
                Id = user.Id.ToString(),
                YoutubeId = user.YoutubeId,
                Role = user.Role.Name,
                SubChannelsJson = user.SubChannelsJson ?? "",
                LastChannelsUpdate = user?.LastChannelsUpdate?.ToString() ?? "",
                Folders = foldersJson.ToString()
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new GetUserDataResponse { };
        }
    }

    public override async Task<UpdateSubChannelsResponse> UpdateSubChannels(UpdateSubChannelsRequest request, ServerCallContext context)
    {
        try
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(request.Id));
            if (user != null)
            {
                user.SetSubChannelsJson(request.SubChannelsJson);
                await _context.SaveChangesAsync();
                return new UpdateSubChannelsResponse { LastChannelsUpdate = user.LastChannelsUpdate.ToString() };
            }
            else throw new MemberAccessException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new UpdateSubChannelsResponse { };
        }
    }

    public override async Task<UpdateYoutubeIdResponse> UpdateYoutubeId(UpdateYoutubeIdRequest request, ServerCallContext context)
    {
        try
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(request.Id));
            if (user != null)
            {
                user.YoutubeId = request.YoutubeId;
                await _context.SaveChangesAsync();
                return new UpdateYoutubeIdResponse { Success = true };
            }
            else throw new MemberAccessException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new UpdateYoutubeIdResponse { Success = false };
        }
    }
}

