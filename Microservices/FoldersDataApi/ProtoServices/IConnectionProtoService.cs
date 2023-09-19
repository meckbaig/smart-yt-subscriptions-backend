namespace FoldersDataApi.ProtoServices;

public interface IConnectionProtoService
{
    Task<bool> GetState();
}