namespace congress_cucuta.Data;

internal class Role (IDType id, string titleSingular, string titlePlural) : IID {
    public static readonly IDType MEMBER = byte.MaxValue;
    public static readonly IDType HEAD_GOVERNMENT = MEMBER - 1;
    public static readonly IDType HEAD_STATE = HEAD_GOVERNMENT - 1;

    public IDType ID => id;
    public string TitleSingular => titleSingular;
    public string TitlePlural => titlePlural;
}
