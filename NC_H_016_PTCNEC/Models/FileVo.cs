namespace NC_H_016_PTCNEC.Models;

/// <summary>
/// 檔案模型
/// </summary>
internal class FileVo
{
    /// <summary>
    /// 前綴
    /// </summary>
    public string Prefix { get; set; } = null!;

    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 年月日yyyyMMdd
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 句點
    /// </summary>
    public string Dot { get; set; } = null!;

    /// <summary>
    /// 星期WW
    /// </summary>
    public DayOfWeek DayOfWeek { get; set; }

    /// <summary>
    /// 完整檔名
    /// </summary>
    public string FullName
        => $"{this.Prefix}{this.Name}{this.Date:yyyyMMdd}{this.Dot}{(this.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)this.DayOfWeek)}";
}