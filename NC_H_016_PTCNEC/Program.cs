using NC_H_016_PTCNEC.Services;

// 實作題1
/*
1.	參數為一組長度 15碼的條碼，條碼內容為下列資訊所組成：
	商品條碼（5碼）
	保存年月日YYYYMMDD（8碼）
	時間HH（2碼）：EX:14表示14:00到期

2.	依 條碼、系統時間 進行判斷並顯示以下訊息：
系統時間 2022/02/15 22:25
EEEEE2022021523		商品即將到期
	＂商品即將到期＂：系統時間 在 條碼保存期限（年月日時間）的前5分鐘（含）到前1小時顯示。
	＂商品已過期＂：系統時間 在 條碼保存期限（年月日時間）的前5分鐘內開始就判定『已過期』。
 */
string inputNormal = $"ABCDE{DateTime.Now.AddMinutes(120):yyyyMMddHH}";
string inputNotify = $"ABCDE{DateTime.Now.AddMinutes(30):yyyyMMddHH}";
string inputExipred = $"ABCDE{DateTime.Now.AddMinutes(3):yyyyMMddHH}";
string inputNow = $"ABCDE{DateTime.Now:yyyyMMddHH}";
string inputPast = $"ABCDE{DateTime.Now.AddMinutes(-30):yyyyMMddHH}";
BarcodeService barcodeService = new(-5, -60);
barcodeService.DoWork(inputNormal);
barcodeService.DoWork(inputNotify);
barcodeService.DoWork(inputExipred);
barcodeService.DoWork(inputNow);
barcodeService.DoWork(inputPast);

// 實作題2
/*
1.	某目錄一開始為空，每日會產生『IFxxxxxxYYYYMMDD.x』檔案數個：
IFaaaaaa20130101.1
IFbbbbbb20141201.2
IFcccccc20150101.3

附檔名x 代筆意義：
	1表示星期一
	2表示星期二
	...依此類推

2.	假設目前系統日為2015/10/27，呼叫此Function將：
	刪除超過系統日6天以前的檔案
	保留距離系統日6天內檔案
*/
string rootPath = Environment.CurrentDirectory;
string basePath = "TestFiles";
string relativePath = "";
FileService fileService = new(rootPath, basePath);
fileService.CreateEmptyFile(relativePath, "test01", DateOnly.FromDateTime(DateTime.Now));
fileService.CreateEmptyFile(relativePath, "test02", DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
fileService.CreateEmptyFile(relativePath, "test03", DateOnly.FromDateTime(DateTime.Now.AddDays(-5)));
fileService.CreateEmptyFile(relativePath, "test04", DateOnly.FromDateTime(DateTime.Now.AddDays(-7)));
fileService.CreateEmptyFile(relativePath, "test05", DateOnly.FromDateTime(DateTime.Now.AddDays(-10)));
fileService.CleanUpOutdatedFile(relativePath, -6);