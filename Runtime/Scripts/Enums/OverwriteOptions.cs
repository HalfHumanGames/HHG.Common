namespace HHG.Common.Runtime
{
    [System.Flags]
    public enum OverwriteOptions
    {
        Default = 0,
        ExcludeFields = 1 << 0,
        ExcludeProperties = 1 << 1,
        ExcludePublic = 1 << 2,
        ExcludeNonPublic  = 1 << 3,
        ExcludeReferences = 1 << 4,
        ExcludeCollections = 1 << 5,
    }
}