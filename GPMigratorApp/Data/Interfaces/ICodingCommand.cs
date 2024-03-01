using System.Data;
using GPMigratorApp.Data.IntermediaryModels;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data.Interfaces;

public interface ICodingCommand
{
    Task<CodeDTO?> GetCodingAsync(string? snomedCode, string? readCode, CancellationToken cancellationToken,
        IDbTransaction transaction);

    Task<Guid> InsertCodeAsync(string? snomedCode, string? readCode, string? description,
        CancellationToken cancellationToken, IDbTransaction transaction);
}