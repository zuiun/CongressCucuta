namespace congress_cucuta.Data;

internal class Role (IDType id, string title, Permissions permissions) : IID {
    public static readonly IDType MEMBER = byte.MaxValue;
    public static readonly IDType HEAD_GOVERNMENT = MEMBER - 1;
    public static readonly IDType HEAD_STATE = HEAD_GOVERNMENT - 1;
    public IDType ID { get; } = id;
    public string Title { get; } = title;
    public Permissions Permissions { get; set; } = permissions;
}
