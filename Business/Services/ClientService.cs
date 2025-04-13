using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Business.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;
    public async Task<ServiceResult<Client>> CreateClientAsync(string name)
    {
        try
        {
            var existingEntityResult = await _clientRepository.GetAsync(x => x.ClientName == name);
            if (existingEntityResult.Result is ClientEntity)
                return ServiceResult<Client>.Ok(existingEntityResult.Result.MapTo<Client>());

            var createResult = await _clientRepository.CreateAsync(new ClientEntity { ClientName = name });
            if(createResult.Succeeded)
            {
                return ServiceResult<Client>.Created(createResult.Result.MapTo<Client>());
            }
            return ServiceResult<Client>.Error("Failed to create client");
        } catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return ServiceResult<Client>.Error("Failed to create client");
        }
    }
}
