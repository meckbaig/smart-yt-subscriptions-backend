using DataBaseApi.Data;
using DataBaseApi.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DataBaseApi.Services;

public class FolderService : FolderProto.FolderProtoBase
{
    private AppDbContext _context;
    public FolderService(AppDbContext context)
    {
        _context = context;
    }


    public override async Task<GetFolderResponse> GetFolder(GetFolderRequest request, ServerCallContext context)
    {
        try
        {
            Folder folder = _context.Folders.FirstOrDefault(f => f.Id == Guid.Parse(request.Id));
            if (folder != null)
            {
                if (folder.UserId == Guid.Parse(request.UserId)
                    || (!request.Edit && (AccessEnum)folder.AccessId == AccessEnum.LinkAccess)
                    || (!request.Edit && (AccessEnum)folder.AccessId == AccessEnum.Public))
                {
                    return new GetFolderResponse { JsonString = folder.ToJsonString() };
                }
            }
            return new GetFolderResponse { };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new GetFolderResponse { };
        }
    }

    public override async Task<CreateFolderResponse> CreateFolder(CreateFolderRequest request, ServerCallContext context)
    {
        Folder folder = new Folder(Guid.Parse(request.UserId), request.Name);
        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();

        return new CreateFolderResponse { JsonString = folder.ToJsonString() };
    }

    public override async Task<UpdateFolderResponse> UpdateFolder(UpdateFolderRequest request, ServerCallContext context)
    {
        try
        {
            var jsonFolder = JsonSerializer.Deserialize<JsonElement>(request.JsonString);
            Guid id = Guid.Parse(jsonFolder.GetProperty("id").ToString());
            Folder newFolder = new Folder();
            Folder folder = _context.Folders.FirstOrDefault(f => f.Id == id);
            if (folder != null)
            {
                request.JsonString.DeserializeSafely(ref folder);
                folder.LastChannelsUpdate = DateTime.UtcNow; //DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                              
                await _context.SaveChangesAsync();
                return new UpdateFolderResponse { JsonString = folder.ToJsonString() };
            }
            return new UpdateFolderResponse { };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return new UpdateFolderResponse { };
        }

    }

    public override async Task<DeleteFolderResponse> DeleteFolder(DeleteFolderRequest request, ServerCallContext context)
    {
        try
        {
            Folder folder = _context.Folders.FirstOrDefault(f => f.UserId == Guid.Parse(request.UserId) && f.Id == Guid.Parse(request.Id));
            if (folder != null)
            {
                _context.Folders.Remove(folder);
                await _context.SaveChangesAsync();
                return new DeleteFolderResponse { Success = true };
            }
            return new DeleteFolderResponse { Success = false };
        }
        catch (Exception ex )
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new DeleteFolderResponse { Success = false };
        }
    }

    public override async Task<GetPublicFoldersResponse> GetPublicFolders(GetPublicFoldersRequest request, ServerCallContext context)
    {
        try
        {
            Guid id = request.UserId == "" ? Guid.NewGuid() : Guid.Parse(request.UserId);
            List<Folder> folders = _context.Folders.Where(f => f.UserId != id
                && (AccessEnum)f.AccessId == AccessEnum.Public).ToList().Shuffle().Take(200).ToList();
            StringBuilder foldersJson = new StringBuilder("[");
            foreach (var folder in folders)
            {
                foldersJson.Append(folder.ToJsonString(new[] { "SubChannelsJson" }) + ",");
            }
            foldersJson.Remove(foldersJson.Length - 1, 1);
            if (foldersJson.Length > 0)
                foldersJson.Append(']');

            return new GetPublicFoldersResponse { JsonString = foldersJson.ToString() };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new GetPublicFoldersResponse { JsonString = "" };
        }
    }
}