using System.Data;
using GPMigratorApp.DTOs;

namespace GPMigratorApp.Services.Interfaces;

public interface ICodingService
{
    Task<Guid> PutCoding(CodeDTO code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);

}