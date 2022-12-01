using NUnit.Framework;
using System.Collections;

namespace NC_H_016_PTCNEC.Services.Tests;

[TestFixture]
public partial class FileServiceTests
{
    private class 測資
    {
        #region ConvertToBarcodeTest
        /// <summary>
        /// 正確答案
        /// </summary>
        private const string CORRECT_ANSWER = "IFxxxxxx20220101.1";

        public static IEnumerable 參數長度不等於18()
        {
            string fullString = CORRECT_ANSWER;
            for (int len = 0; len < fullString.Length && len < 18; len++)
            {
                yield return new TestCaseData(fullString[..len])
                    .SetArgDisplayNames($"{nameof(參數長度不等於18)}-長度{len + 1}");
            }
        }

        public static IEnumerable 前2碼前綴不正確()
        {
            string after2 = CORRECT_ANSWER[2..];
            yield return new TestCaseData($"GG{after2}")
                .SetArgDisplayNames($"{nameof(前2碼前綴不正確)}-英文");
            yield return new TestCaseData($"中文{after2}")
                .SetArgDisplayNames($"{nameof(前2碼前綴不正確)}-中文");
            yield return new TestCaseData($"##{after2}")
                .SetArgDisplayNames($"{nameof(前2碼前綴不正確)}-符號");
        }

        public static IEnumerable 第3至8碼包含非英數()
        {
            string start2 = CORRECT_ANSWER[..2];
            string after8 = CORRECT_ANSWER[8..];
            yield return new TestCaseData($"{start2}######{after8}")
                .SetArgDisplayNames($"{nameof(第3至8碼包含非英數)}-全符號");
            yield return new TestCaseData($"{start2}###AAA{after8}")
                .SetArgDisplayNames($"{nameof(第3至8碼包含非英數)}-開頭符號");
            yield return new TestCaseData($"{start2}BB##CC{after8}")
                .SetArgDisplayNames($"{nameof(第3至8碼包含非英數)}-中間符號");
            yield return new TestCaseData($"{start2}#D#E#F{after8}")
                .SetArgDisplayNames($"{nameof(第3至8碼包含非英數)}-穿插符號");
            yield return new TestCaseData($"{start2}GGG###{after8}")
                .SetArgDisplayNames($"{nameof(第3至8碼包含非英數)}-結尾符號");
        }

        public static IEnumerable 第9至16碼日期不合理()
        {
            string start8 = CORRECT_ANSWER[..8];
            string end2 = CORRECT_ANSWER[^2..];
            yield return new TestCaseData($"{start8}33331208{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-年份不合理");
            yield return new TestCaseData($"{start8}20221501{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-月份不合理");
            yield return new TestCaseData($"{start8}20220135{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-日期不合理");
            yield return new TestCaseData($"{start8}20220230{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-二月日期不合理");
            yield return new TestCaseData($"{start8}20220229{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-非閏年不合理");
            yield return new TestCaseData($"{start8}21000229{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-每百年非閏年不合理");
            yield return new TestCaseData($"{start8}19000229{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼日期不合理)}-每百年非閏年不合理");
        }

        public static IEnumerable 第9至16碼不符合yyyyMMddHH格式()
        {
            string start8 = CORRECT_ANSWER[..8];
            string end2 = CORRECT_ANSWER[^2..];
            yield return new TestCaseData($"{start8}yyyyMMdd{end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼不符合yyyyMMddHH格式)}-英文");
            yield return new TestCaseData($"{start8}年年年年月月日日時時")
                .SetArgDisplayNames($"{nameof(第9至16碼不符合yyyyMMddHH格式)}-中文");
            yield return new TestCaseData($"{start8}@@@@##$${end2}")
                .SetArgDisplayNames($"{nameof(第9至16碼不符合yyyyMMddHH格式)}-符號");
        }

        public static IEnumerable 最後1碼不符合星期幾格式()
        {
            string beforeLast = CORRECT_ANSWER[..^1];
            yield return new TestCaseData($"{beforeLast}0")
                .SetArgDisplayNames($"{nameof(最後1碼不符合星期幾格式)}-零");
            yield return new TestCaseData($"{beforeLast}A")
                .SetArgDisplayNames($"{nameof(最後1碼不符合星期幾格式)}-英文");
            yield return new TestCaseData($"{beforeLast}星")
                .SetArgDisplayNames($"{nameof(最後1碼不符合星期幾格式)}-中文");
            yield return new TestCaseData($"{beforeLast}#")
                .SetArgDisplayNames($"{nameof(最後1碼不符合星期幾格式)}-符號");
        }

        public static IEnumerable 正確資料()
        {
            string prefix2 = CORRECT_ANSWER[..2];
            string start8 = CORRECT_ANSWER[..8];
            string after8 = CORRECT_ANSWER[8..];
            // 測試2000年起算50年資料
            int testDays = 365 * 50;
            DateTime date = new(2000, 1, 1);
            for (int offset = 0; offset < testDays; offset += 37)
            {
                string targetDate = date.AddDays(offset).ToString("yyyyMMdd");
                string targetDayOfWeek = date.AddDays(offset).DayOfWeek.ToString();
                if (targetDayOfWeek == "0")
                {
                    targetDayOfWeek = "7";
                }
                yield return new TestCaseData($"{start8}{targetDate}.{targetDayOfWeek}")
                    .SetArgDisplayNames($"{nameof(正確資料)}-年月日正常{targetDate}");
            }
            yield return new TestCaseData($"{start8}20200229.6")
                .SetArgDisplayNames($"{nameof(正確資料)}-閏年二月合理");
            yield return new TestCaseData($"{start8}20000229.2")
                .SetArgDisplayNames($"{nameof(正確資料)}-千禧年是閏年日期合理");

            yield return new TestCaseData($"{prefix2}ABCDEF{after8}")
                .SetArgDisplayNames($"{nameof(正確資料)}-大寫英文");
            yield return new TestCaseData($"{prefix2}abcdef{after8}")
                .SetArgDisplayNames($"{nameof(正確資料)}-小寫英文");
            yield return new TestCaseData($"{prefix2}ABC999{after8}")
                .SetArgDisplayNames($"{nameof(正確資料)}-大寫英文數字");
            yield return new TestCaseData($"{prefix2}abc888{after8}")
                .SetArgDisplayNames($"{nameof(正確資料)}-小寫英文數字");
            yield return new TestCaseData($"{prefix2}123456{after8}")
                .SetArgDisplayNames($"{nameof(正確資料)}-數字");
        }
        #endregion
    }
}