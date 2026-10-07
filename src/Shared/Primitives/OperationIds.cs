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

public readonly record struct IdempotencyKey
{
    public string Value { get; }

    public IdempotencyKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Idempotency key is required.", nameof(value));

        if (value.Length > 200)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
