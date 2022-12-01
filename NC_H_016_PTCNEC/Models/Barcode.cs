namespace NC_H_016_PTCNEC.Models;

/// <summary>
/// 條碼模型
/// </summary>
public class Barcode
{
    /// <summary>
    /// 商品條碼
    /// </summary>
    public string SerialNumber { get; set; } = null!;

    /// <summary>
    /// 年月日yyyyMMdd
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// 時間HH
    /// </summary>
    public TimeOnly Time { get; set; }

    /// <summary>
    /// 完整日期時間
    /// </summary>
    public DateTime FullDateTime
        => new(this.Date.Year, this.Date.Month, this.Date.Day, this.Time.Hour, this.Time.Minute, this.Time.Second);

    /// <summary>
    /// 完整條碼
    /// </summary>
    public string FullBarCode
        => $"{this.SerialNumber}{this.Date}{this.Time}";
}