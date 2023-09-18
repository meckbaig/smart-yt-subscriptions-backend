using DataBaseApi.Data;
using DataBaseApi.Models;
using Grpc.Core;

namespace DataBaseApi.Services
{
    public class ConnectionService : ConnectionProto.ConnectionProtoBase
    {
        private AppDbContext _context;
        public ConnectionService(AppDbContext context)
        {
            _context = context;
        }

        public override Task<GetStateResponse> GetState(GetStateRequest request, ServerCallContext context)
        {
            try
            {
                Role? role = _context?.Roles?.FirstOrDefault();
                if (role?.Id != null)
                    return Task.FromResult(new GetStateResponse { Success = true });
                return Task.FromResult(new GetStateResponse { Success = false });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Task.FromResult(new GetStateResponse { Success = false });
            }
        }
    }
}
