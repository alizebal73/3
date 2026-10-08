using GameNet.Server.Modules.Stations.Application;
using GameNet.Server.Modules.Stations.Domain;
using GameNet.Server.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameNet.Server.Modules.Stations.Infrastructure;

public sealed class EfStationRepository(GameNetDbContext db) : IStationRepository
{
    public Task<Station?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Stations.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Station>> ListAsync(CancellationToken cancellationToken) =>
        await db.Stations
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);

    public Task<bool> CodeExistsAsync(
        string code,
        Guid? excludingId,
        CancellationToken cancellationToken) =>
        db.Stations.AnyAsync(
            x => x.Code == code && (!excludingId.HasValue || x.Id != excludingId.Value),
            cancellationToken);

    public Task AddAsync(Station station, CancellationToken cancellationToken)
    {
        db.Stations.Add(station);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}