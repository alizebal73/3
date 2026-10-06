namespace GameNet.Shared.Primitives;

public readonly record struct EntityId(Guid Value)
{
    public static EntityId New() => new(Guid.NewGuid());
}
