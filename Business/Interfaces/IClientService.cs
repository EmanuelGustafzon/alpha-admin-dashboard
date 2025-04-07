using Business.Models;
using Domain.Models;

namespace Business.Interfaces;

public interface IClientService
{
    public Task<ServiceResult<Client>> CreateClientAsync(string name);
}
