using NC_H_016_PTCNEC.Models;
using NUnit.Framework;

namespace NC_H_016_PTCNEC.Services.Tests;

[TestFixture]
public partial class FileServiceTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp() { }

    [SetUp]
    public void SetUp() { }

    #region ConvertToFileVoTest
    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.參數長度不等於18))]
    public void ConvertToFileVoTest_Given_參數長度不等於18_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        string expect = "格式不符:參數長度不等於18";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.前2碼前綴不正確))]
    public void ConvertToFileVoTest_Given_前2碼前綴不正確_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        string expect = "格式不符:不符合標準格式IFxxxxxxyyyyMMdd.x";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.第3至8碼包含非英數))]
    public void ConvertToFileVoTest_Given_第3至8碼包含非英數_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        string expect = "格式不符:不符合標準格式IFxxxxxxyyyyMMdd.x";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.第9至16碼不符合yyyyMMddHH格式))]
    public void ConvertToFileVoTest_Given_第9至16碼不符合yyyyMMddHH格式_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        string expect = "格式不符:不符合標準格式IFxxxxxxyyyyMMdd.x";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.第9至16碼日期不合理))]
    public void ConvertToFileVoTest_Given_第9至16碼日期不合理_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        string expect = "格式不符:第9至16碼須符合yyyyMMddHH格式";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.最後1碼不符合星期幾格式))]
    public void ConvertToFileVoTest_Given_最後1碼不符合星期幾格式_When_Then_回傳指定錯誤訊息(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        string expect = "格式不符:不符合標準格式IFxxxxxxyyyyMMdd.x";
        Assert.AreEqual(expect, target.ErrorMsg);
    }

    [Test]
    [TestCaseSource(typeof(測資), nameof(測資.正確資料))]
    public void ConvertToFileVoTest_Given_正確資料_When_Then_回傳成功(string input)
    {
        // Arrange
        FileService fileService = new("Test", "Test");
        // Act
        ModelResult<FileVo> target = fileService.ConvertToFileVo(input);
        // Assert
        bool expectIsOk = true;
        string? expectErrorMsg = null;
        Assert.IsTrue(expectIsOk);
        Assert.IsNull(expectErrorMsg);
        Assert.IsNotNull(target);
    }
    #endregion
}