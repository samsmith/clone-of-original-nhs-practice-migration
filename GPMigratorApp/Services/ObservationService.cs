using System.Data;
using GPMigratorApp.Data;
using GPMigratorApp.Data.Interfaces;
using GPMigratorApp.DTOs;
using GPMigratorApp.Services.Interfaces;

namespace GPMigratorApp.Services;

public class ObservationService: IObservationService
{
    private readonly ICodingService _codingService;
    public ObservationService(ICodingService codingService)
    {
        _codingService = codingService;
    }
    
    public async Task PutObservations(IEnumerable<ObservationDTO> observations, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var observationCommand = new ObservationCommand(connection);
        foreach (var observation in observations)
        {
            if (observation.Code is not null)
                observation.Code.Id = await _codingService.PutCoding(observation.Code, connection, transaction, cancellationToken);
            
            var existingRecord =
                await observationCommand.GetObservationAsync(observation.OriginalId, cancellationToken, transaction);
            if (existingRecord is null)
            {
                await observationCommand.InsertObservationAsync(observation, cancellationToken, transaction);
            }
            else
            {
                existingRecord.OriginalId = observation.OriginalId;
                existingRecord.Status = observation.Status;
                existingRecord.Category = observation.Category;
                existingRecord.Code = observation.Code;
                existingRecord.EffectiveDate = observation.EffectiveDate;
                existingRecord.EffectiveDateFrom = observation.EffectiveDateFrom;
                existingRecord.EffectiveDateTo = observation.EffectiveDateTo;
                existingRecord.Issued = observation.Issued;
                existingRecord.Interpretation = observation.Interpretation;
                existingRecord.DataAbsentReason = observation.DataAbsentReason;
                existingRecord.Comment = observation.Comment;
                existingRecord.BodySite = observation.BodySite;
                existingRecord.Method = observation.Method;
                existingRecord.ReferenceRangeLow = observation.ReferenceRangeLow;
                existingRecord.ReferenceRangeHigh = observation.ReferenceRangeHigh;
                existingRecord.ReferenceRangeType = observation.ReferenceRangeType;
                existingRecord.ReferenceRangeAppliesTo = observation.ReferenceRangeAppliesTo;
                existingRecord.ReferenceRangeAgeHigh = observation.ReferenceRangeAgeHigh;
                existingRecord.ReferenceRangeAgeLow = observation.ReferenceRangeAgeLow;
                existingRecord.EntityId = observation.EntityId;
                
                await observationCommand.UpdateObservationAsync(existingRecord, cancellationToken, transaction);
            }
        }

    }
}