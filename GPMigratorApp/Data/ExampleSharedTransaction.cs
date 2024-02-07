using System.Data;
using Dapper;
using GPConnect.Provider.AcceptanceTests.Logger;
using GPMigratorApp.DTOs;
using Microsoft.Data.SqlClient;

namespace GPMigratorApp.Data;


public class ExampleSharedTransaction
{
    private readonly string _connectionString;
    private readonly PatientCommand _patientCommand;
    private readonly ExampleTable2 _exampleTable2;
    
    public ExampleSharedTransaction(string connectionString,PatientCommand patientCommand, ExampleTable2 exampleTable2)
    {
        _connectionString = connectionString;
        _patientCommand = patientCommand;
        _exampleTable2 = exampleTable2;
    }
    
    public async Task AddNewEntry(CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var transaction = connection.BeginTransaction();

        try
        {
            var patientId = await _patientCommand.CreatePatientAsync(new PatientDTO(),cancellationToken, transaction);
            await _exampleTable2.Add(cancellationToken, transaction);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DataException ex)
        {
            await transaction.RollbackAsync();
            Log.WriteLine(ex.Message);
        }

    }
}