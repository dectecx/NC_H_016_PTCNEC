using NC_H_016_PTCNEC.Models;
using NUnit.Framework;

namespace NC_H_016_PTCNEC.Services.Tests;

[TestFixture]
public partial class BarcodeServiceTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp() { }

    [SetUp]
    public void SetUp() { }

    #region ConvertToBarcodeTest
    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.參數長度不等於15))]
    public void ConvertToBarcodeTest_Given_參數長度不等於15_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        BarcodeService barcodeService = new(0, 0);
        // Act
        ModelResult<Barcode> target = barcodeService.ConvertToBarcode(input);
        // Assert
        string expect = "格式不符:參數長度不等於15";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.前5碼包含非英數))]
    public void ConvertToBarcodeTest_Given_前5碼包含非英數_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        BarcodeService barcodeService = new(0, 0);
        // Act
        ModelResult<Barcode> target = barcodeService.ConvertToBarcode(input);
        // Assert
        string expect = "格式不符:前5碼須為英數";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.第6至15碼不符合yyyyMMddHH格式))]
    public void ConvertToBarcodeTest_Given_第6至15碼須符合yyyyMMddHH格式_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        BarcodeService barcodeService = new(0, 0);
        // Act
        ModelResult<Barcode> target = barcodeService.ConvertToBarcode(input);
        // Assert
        string expect = "格式不符:第6至15碼須符合yyyyMMddHH格式";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.正確資料))]
    public void ConvertToBarcodeTest_Given_正確資料_When_Then_回傳成功(string input)
    {
        // Arrange
        BarcodeService barcodeService = new(0, 0);
        // Act
        ModelResult<Barcode> target = barcodeService.ConvertToBarcode(input);
        // Assert
        bool expectIsOk = true;
        string? expectErrorMsg = null;
        Assert.IsTrue(expectIsOk);
        Assert.IsNull(expectErrorMsg);
        Assert.IsNotNull(target);
    }
    #endregion

    #region AnalyzeTest
    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.商品即將到期))]
    public void AnalyzeTest_Given_商品即將到期_When_Then_回傳指定錯誤訊息(Analyze測試模型 input)
    {
        // Arrange
        DateTime systemTime = input.MockSystemTime;
        Barcode barcode = input.Barcode;
        BarcodeService barcodeService = new(input.NotifyExpireMinutes, input.ExpireMinutes);
        // Act
        ModelResult target = barcodeService.Analyze(barcode, systemTime);
        // Assert
        string expect = "商品即將到期";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.商品已過期))]
    public void AnalyzeTest_Given_商品已過期_When_Then_回傳指定錯誤訊息(Analyze測試模型 input)
    {
        // Arrange
        DateTime systemTime = input.MockSystemTime;
        Barcode barcode = input.Barcode;
        BarcodeService barcodeService = new(input.NotifyExpireMinutes, input.ExpireMinutes);
        // Act
        ModelResult target = barcodeService.Analyze(barcode, systemTime);
        // Assert
        string expect = "商品已過期";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.商品正常))]
    public void AnalyzeTest_Given_商品正常_When_Then_回傳成功(Analyze測試模型 input)
    {
        // Arrange
        DateTime systemTime = input.MockSystemTime;
        Barcode barcode = input.Barcode;
        BarcodeService barcodeService = new(input.NotifyExpireMinutes, input.ExpireMinutes);
        // Act
        ModelResult target = barcodeService.Analyze(barcode, systemTime);
        // Assert
        bool expectIsOk = true;
        string? expectErrorMsg = null;
        Assert.IsTrue(expectIsOk);
        Assert.IsNull(expectErrorMsg);
        Assert.IsNotNull(target);
    }

    public class Analyze測試模型
    {
        /// <summary>
        /// 通知時間
        /// </summary>
        public int NotifyExpireMinutes { get; set; }

        /// <summary>
        /// 到期時間
        /// </summary>
        public int ExpireMinutes { get; set; }

        /// <summary>
        /// mock系統時間
        /// </summary>
        public DateTime MockSystemTime { get; set; }

        /// <summary>
        /// 條碼模型
        /// </summary>
        public Barcode Barcode { get; set; } = null!;
    }
    #endregion
}