namespace congress_cucuta.Data;

internal struct Role (byte id, string title, Permissions permissions) : IID {
    public const byte HEAD_OF_GOVERNMENT = byte.MaxValue;
    public const byte HEAD_OF_STATE = HEAD_OF_GOVERNMENT - 1;

    public readonly byte ID { get; } = id;
    public readonly string Title { get; } = title;
    public Permissions Permissions { get; set; } = permissions;
}
