# 開發與建置

## 建置

### 已驗證的 Windows 建置方式

`build.ps1` 使用目標環境現有的依賴，不下載或散布遊戲組件。

```powershell
./build.ps1 -Target Mono -GameRoot 'D:\YourGame'
./build.ps1 -Target IL2CPP -GameRoot 'D:\YourGame' -CompilerPath 'C:\BuildTools\Roslyn\csc.exe'
```

Mono 預設使用 Windows .NET Framework C# 編譯器。有多個 `*_Data/Managed` 時，以 `-ManagedDirectory` 指定。

IL2CPP 需要 Roslyn `csc.exe`、BepInEx 6、目標遊戲產生的 `BepInEx/interop`，以及 .NET 6 runtime DLL。預設 runtime 位於 `GameRoot/dotnet`，可用 `-RuntimeDirectory` 指定其他路徑。

輸出位於 `bin/Mono` 或 `bin/IL2CPP`。

### SDK 專案

保留上游 solution，專案版本以各 `.csproj` 為準。使用 .NET SDK 時，可用 `-p:UnityReferenceDirectory=...` 指定 Unity/TMP 組件目錄。Mono 與 IL2CPP 必須各自使用正確類型的組件，請分開建置。

上游隨附 `libs` 保持原樣，並非目標遊戲的完整依賴。新增的 UI/UIModule/TextCore 引用需要依環境補齊。

上游 GitHub Actions 尚未用本 Fork 所需依賴完成驗證。本機建置使用 `build.ps1`；發布附件不包含字型檔。

## 自動建置的依賴缺口

目前 .csproj 新增了 UnityEngine.UI、UnityEngine.UIModule 與 TextCore 等引用，但上游隨附的 libs 並未包含每個專案所需的全部組件。繼承的 workflow 直接執行 dotnet build，沒有先準備這些依賴；不能將它視為可直接使用的自動發布流程。需要先決定依賴的取得方式與目標 Unity 版本，再驗證 CI。
