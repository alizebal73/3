using GameNet.Server.Modules.Stations.Domain;

namespace GameNet.Server.Modules.Stations.Application;

public interface IStationRepository
{
    Task<Station?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Station>> ListAsync(CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken);
    Task AddAsync(Station station, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}