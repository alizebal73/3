namespace GameNet.Shared.Primitives;

public readonly record struct CorrelationId(string Value)
{
    public static CorrelationId New() => new(Guid.NewGuid().ToString("N"));
    public override string ToString() => Value;
}

public readonly record struct CommandId(Guid Value)
{
    public static CommandId New() => new(Guid.NewGuid());
}

public readonly record struct IdempotencyKey(string Value)
{
    public IdempotencyKey
    {
        if (string.IsNullOrWhiteSpace(Value))
            throw new ArgumentException("Idempotency key is required.", nameof(Value));

        if (Value.Length > 200)
            throw new ArgumentOutOfRangeException(nameof(Value));
    }

    public override string ToString() => Value;
}
