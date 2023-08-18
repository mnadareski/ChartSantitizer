namespace ChartSanitizer
{
    /// <summary>
    /// Different types of INI rows being parsed
    /// </summary>
    internal enum IniRowType
    {
        None,
        SectionHeader,
        KeyValue,
        Comment,
        Invalid,
    }
}
