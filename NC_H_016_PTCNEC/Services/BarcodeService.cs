using NC_H_016_PTCNEC.Extensions;
using NC_H_016_PTCNEC.Models;
using System.Text.RegularExpressions;

namespace NC_H_016_PTCNEC.Services;

/// <summary>
/// 條碼服務
/// </summary>
internal class BarcodeService
{
    /// <summary>
    /// 通知時間
    /// </summary>
    private readonly int _notifyExpireMinutes;

    /// <summary>
    /// 到期時間
    /// </summary>
    private readonly int _expireMinutes;

    /// <summary>
    /// 建構子
    /// </summary>
    internal BarcodeService(int notifyExpireMinutes, int expireMinutes)
    {
        this._notifyExpireMinutes = notifyExpireMinutes;
        this._expireMinutes = expireMinutes;
    }

    /// <summary>
    /// 執行
    /// </summary>
    /// <param name="input">輸入字串</param>
    public void DoWork(string input)
    {
        // 系統時間
        DateTime systemTime = DateTime.Now;
        string systemTimeStr = $"系統時間 {systemTime:yyyy/MM/dd HH:mm}";

        ModelResult<Barcode> convertResult = this.ConvertToBarcode(input);
        if (!convertResult.IsOk)
        {
            ConsoleColor.White.WriteLine(systemTimeStr);
            ConsoleColor.White.Write(input);
            ConsoleColor.Red.WriteLine($"\t{convertResult.ErrorMsg}");
            return;
        }
        Barcode barcode = convertResult.Data!;
        ModelResult analyzeResult = this.Analyze(barcode, systemTime);
        if (!analyzeResult.IsOk)
        {
            ConsoleColor.White.WriteLine(systemTimeStr);
            ConsoleColor.White.Write(input);
            ConsoleColor.Red.WriteLine($"\t{analyzeResult.ErrorMsg}");
            return;
        }
        ConsoleColor.White.WriteLine(systemTimeStr);
        ConsoleColor.White.WriteLine(input);
        return;
    }

    /// <summary>
    /// 轉換為<see cref="Barcode"/>模型
    /// </summary>
    /// <param name="input">輸入字串</param>
    /// <returns></returns>
    private ModelResult<Barcode> ConvertToBarcode(string input)
    {
        if (input?.Length != 15)
        {
            return new("格式不符:參數長度不等於15");
        }

        string serialNumber = input[0..5];
        string year = input[5..9];
        string month = input[9..11];
        string day = input[11..13];
        string hour = input[13..15];

        Regex regex = new(@"^[a-zA-Z0-9]{5}$");
        if (!regex.Match(serialNumber).Success)
        {
            return new("格式不符:前5碼須為英數");
        }

        bool parsedDate = DateOnly.TryParse($"{year}/{month}/{day}", out DateOnly date);
        bool parsedHour = TimeOnly.TryParse($"{hour}:00", out TimeOnly time);
        if (!parsedDate || !parsedHour)
        {
            return new("格式不符:第6至15碼須符合yyyyMMddHH格式");
        }

        Barcode barcode = new()
        {
            SerialNumber = serialNumber,
            Date = date,
            Time = time
        };
        return new(barcode);
    }

    /// <summary>
    /// 商業邏輯
    /// </summary>
    /// <param name="barcode">條碼模型</param>
    /// <param name="systemTime">系統時間</param>
    /// <returns></returns>
    private ModelResult Analyze(Barcode barcode, DateTime systemTime)
    {
        // 商品即將到期:前5分鐘(含)到前1小時
        if (systemTime > barcode.FullDateTime.AddMinutes(this._notifyExpireMinutes) &&
            systemTime <= barcode.FullDateTime.AddMinutes(this._expireMinutes))
        {
            return new("商品即將到期");
        }
        // 商品已過期:前5分鐘內開始就判定已過期
        if (systemTime > barcode.FullDateTime.AddMinutes(this._expireMinutes))
        {
            return new("商品已過期");
        }
        return new();
    }
}
