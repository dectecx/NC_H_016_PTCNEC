using NC_H_016_PTCNEC.Extensions;
using NC_H_016_PTCNEC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NC_H_016_PTCNEC.Services;

/// <summary>
/// 檔案服務
/// </summary>
internal class FileService
{
    /// <summary>
    /// 根目錄
    /// </summary>
    private readonly string _rootPath;

    /// <summary>
    /// 基底目錄
    /// </summary>
    private readonly string _basePath;

    /// <summary>
    /// 建構子
    /// </summary>
    internal FileService(string rootPath, string basePath)
    {
        this._rootPath = rootPath;
        this._basePath = basePath;
    }

    /// <summary>
    /// 產生空白檔案
    /// </summary>
    /// <param name="relativePath">資料夾相對路徑</param>
    /// <param name="expiredDate">有效日期</param>
    public Task CreateEmptyFile(string relativePath, string name, DateOnly expiredDate)
    {
        if (name?.Length != 6)
        {
            throw new ArgumentException("長度不符");
        }
        FileVo vo = new()
        {
            Prefix = "IF",
            Name = name,
            Date = expiredDate,
            Dot = ".",
            DayOfWeek = expiredDate.DayOfWeek
        };
        string fullPath = $"{this._rootPath}/{this._basePath}/{relativePath}";
        string fullFilePath = $"{fullPath}/{vo.FullName}";
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        using StreamWriter sw = new(fullFilePath);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 刪除指定路徑
    /// </summary>
    /// <param name="relativePath">資料夾相對路徑</param>
    /// <param name="expiredDay">有效天數</param>
    public void CleanUpOutdatedFile(string relativePath, int expiredDay)
    {
        string fullPath = $"{this._rootPath}/{this._basePath}/{relativePath}";
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        string[] filePathNames = Directory.GetFiles(fullPath);
        foreach (string filePathName in filePathNames)
        {
            string fileName = Path.GetFileName(filePathName);
            ModelResult<FileVo> convertResult = this.ConvertToFileVo(fileName);
            if (!convertResult.IsOk)
            {
                ConsoleColor.White.Write(fileName);
                ConsoleColor.Red.WriteLine($"\t{convertResult.ErrorMsg}");
            }
            FileVo fileVo = convertResult.Data!;
            if (fileVo.Date < DateOnly.FromDateTime(DateTime.Now.AddDays(expiredDay)))
            {
                File.Delete(filePathName);
                ConsoleColor.Cyan.WriteLine($"已刪除\t{fileName}");
            }
            else
            {
                ConsoleColor.Green.WriteLine($"已保留\t{fileName}");
            }
        }
    }

    /// <summary>
    /// 轉換為<see cref="FileVo"/>模型
    /// </summary>
    /// <param name="input">輸入字串</param>
    /// <returns></returns>
    private ModelResult<FileVo> ConvertToFileVo(string input)
    {
        if (input?.Length != 18)
        {
            return new("格式不符:參數長度不等於18");
        }
        Regex regex = new(@"^IF[a-zA-Z0-9]{6}\d{8}.[1-7]{1}$");
        if (!regex.Match(input).Success)
        {
            return new("格式不符:不符合標準格式IFxxxxxxyyyyMMdd.x");
        }

        string prefix = input[0..2];
        string name = input[2..8];
        string year = input[8..12];
        string month = input[12..14];
        string day = input[14..16];
        string dot = input[16..17];
        string week = input[17..18];

        bool parsedDate = DateOnly.TryParse($"{year}/{month}/{day}", out DateOnly date);
        if (!parsedDate)
        {
            return new("格式不符:第9至16碼須符合yyyyMMddHH格式");
        }

        // 星期日
        if (week == "7")
        {
            week = "0";
        }
        bool parsedDayOfWeek = int.TryParse(week, out int weekInt) && Enum.IsDefined(typeof(DayOfWeek), weekInt);
        if (!parsedDayOfWeek)
        {
            return new("格式不符:第18碼須為1-7,代表星期一至日");
        }

        FileVo vo = new()
        {
            Prefix = prefix,
            Name = name,
            Date = date,
            Dot = dot,
            DayOfWeek = DayOfWeek.Monday,
        };
        return new(vo);
    }
}
