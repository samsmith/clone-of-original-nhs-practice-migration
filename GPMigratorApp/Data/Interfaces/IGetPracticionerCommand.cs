using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface IGetPracticionerCommand
{
    Task<PracticionerDTO?> GetPracticionerAsync(string originalId, CancellationToken cancellationToken,
        IDbTransaction transaction);

}