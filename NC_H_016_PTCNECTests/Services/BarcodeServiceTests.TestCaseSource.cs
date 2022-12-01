using NUnit.Framework;
using System.Collections;

namespace NC_H_016_PTCNEC.Services.Tests;

[TestFixture]
public partial class BarcodeServiceTests
{
    private class 測資
    {
        #region ConvertToBarcodeTest
        /// <summary>
        /// 正確答案
        /// </summary>
        private const string CORRECT_ANSWER = "ABCDEyyyyMMddHH";

        public static IEnumerable 參數長度不等於15()
        {
            string fullString = CORRECT_ANSWER;
            for (int len = 0; len < fullString.Length && len < 15; len++)
            {
                yield return new TestCaseData(fullString[..len])
                    .SetArgDisplayNames($"{nameof(參數長度不等於15)}-長度{len + 1}");
            }
        }

        public static IEnumerable 前5碼包含非英數()
        {
            string after5 = CORRECT_ANSWER[5..];
            yield return new TestCaseData($"#####{after5}")
                .SetArgDisplayNames($"{nameof(前5碼包含非英數)}-全符號");
            yield return new TestCaseData($"##AAA{after5}")
                .SetArgDisplayNames($"{nameof(前5碼包含非英數)}-開頭符號");
            yield return new TestCaseData($"BB#CC{after5}")
                .SetArgDisplayNames($"{nameof(前5碼包含非英數)}-中間符號");
            yield return new TestCaseData($"#D#E#{after5}")
                .SetArgDisplayNames($"{nameof(前5碼包含非英數)}-穿插符號");
            yield return new TestCaseData($"FFF##{after5}")
                .SetArgDisplayNames($"{nameof(前5碼包含非英數)}-結尾符號");
        }

        public static IEnumerable 第6至15碼不符合yyyyMMddHH格式()
        {
            string start5 = CORRECT_ANSWER[..5];
            yield return new TestCaseData($"{start5}yyyyMMddHH")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-英文");
            yield return new TestCaseData($"{start5}年年年年月月日日時時")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-中文");
            yield return new TestCaseData($"{start5}@@@@##$$%%")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-符號");
            yield return new TestCaseData($"{start5}3333120801")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-年份不合理");
            yield return new TestCaseData($"{start5}2022150101")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-月份不合理");
            yield return new TestCaseData($"{start5}2022013501")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-日期不合理");
            yield return new TestCaseData($"{start5}2022010125")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-時間不合理");
            yield return new TestCaseData($"{start5}2022023001")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-二月日期不合理");
            yield return new TestCaseData($"{start5}2022022901")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-非閏年不合理");
            yield return new TestCaseData($"{start5}2100022901")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-每百年非閏年不合理");
            yield return new TestCaseData($"{start5}1900022901")
                .SetArgDisplayNames($"{nameof(第6至15碼不符合yyyyMMddHH格式)}-每百年非閏年不合理");
        }

        public static IEnumerable 正確資料()
        {
            string start5 = CORRECT_ANSWER[..5];
            string after5 = CORRECT_ANSWER[5..];
            // 測試2000年起算50年資料
            int testDays = 365 * 50;
            DateTime date = new(2000, 1, 1);
            for (int offset = 0; offset < testDays; offset += 37)
            {
                string targetDate = date.AddDays(offset).ToString("yyyyMMdd");
                yield return new TestCaseData($"{start5}{targetDate}01")
                    .SetArgDisplayNames($"{nameof(正確資料)}-年月日正常{targetDate}");
            }
            yield return new TestCaseData($"{start5}2020022901")
                .SetArgDisplayNames($"{nameof(正確資料)}-閏年二月合理");
            yield return new TestCaseData($"{start5}2000022901")
                .SetArgDisplayNames($"{nameof(正確資料)}-千禧年是閏年日期合理");

            yield return new TestCaseData($"ABCDE{after5}")
                .SetArgDisplayNames($"{nameof(正確資料)}-大寫英文");
            yield return new TestCaseData($"abcde{after5}")
                .SetArgDisplayNames($"{nameof(正確資料)}-小寫英文");
            yield return new TestCaseData($"ABC99{after5}")
                .SetArgDisplayNames($"{nameof(正確資料)}-大寫英文數字");
            yield return new TestCaseData($"abc88{after5}")
                .SetArgDisplayNames($"{nameof(正確資料)}-小寫英文數字");
            yield return new TestCaseData($"12345{after5}")
                .SetArgDisplayNames($"{nameof(正確資料)}-數字");
        }
        #endregion

        #region AnalyzeTest
        /// <summary>
        /// 基準系統時間
        /// </summary>
        private static DateTime SystemTime = new(2022, 12, 1, 14, 0, 0);

        private static Analyze測試模型 GetAnalyze測試模型_偏差時間(
            int notifyExpireMinutes, int expireMinutes, int offsetDay, int offsetMinutes)
        {
            DateTime mockSystemTime = SystemTime;
            DateOnly date = DateOnly.FromDateTime(mockSystemTime.AddDays(offsetDay).AddMinutes(offsetMinutes));
            TimeOnly time = TimeOnly.FromDateTime(mockSystemTime.AddDays(offsetDay).AddMinutes(offsetMinutes));
            return new()
            {
                NotifyExpireMinutes = notifyExpireMinutes,
                ExpireMinutes = expireMinutes,
                MockSystemTime = mockSystemTime,
                Barcode = new()
                {
                    Date = date,
                    Time = time
                }
            };
        }

        public static IEnumerable 商品正常()
        {
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 10, 0))
                .SetArgDisplayNames($"{nameof(商品正常)}-條碼時間為10天後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 1, 0))
                .SetArgDisplayNames($"{nameof(商品正常)}-條碼時間為1天後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 120))
                .SetArgDisplayNames($"{nameof(商品正常)}-條碼時間為120分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 90))
                .SetArgDisplayNames($"{nameof(商品正常)}-條碼時間為90分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 60))
                .SetArgDisplayNames($"{nameof(商品正常)}-條碼時間為60分鐘後");
        }

        public static IEnumerable 商品即將到期()
        {
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 30))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為30分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 15))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為15分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 10))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為10分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(-60, -5, 0, 5))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為5分鐘後");
        }

        public static IEnumerable 商品已過期()
        {
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, -1, 0))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為1天前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, -5, 0))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為5天前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, 4))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為4分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, 3))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為3分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, 2))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為2分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, 1))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為1分鐘後");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, 0))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為現在");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -1))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為1分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -2))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為2分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -3))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為3分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -4))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為4分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -5))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為5分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -60))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為60分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -100))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為100分鐘前");
            yield return new TestCaseData(GetAnalyze測試模型_偏差時間(0, -5, 0, -200))
                .SetArgDisplayNames($"{nameof(商品已過期)}-條碼時間為200分鐘前");
        }
        #endregion
    }
}