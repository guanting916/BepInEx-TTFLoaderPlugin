# BepInEx TTF Font Loader — 1.2.1

本專案透過 **GPT6.1 SOL** 協助建置與編譯。

本專案 Fork 自 [you9you/BepInEx-TTFLoaderPlugin](https://github.com/you9you/BepInEx-TTFLoaderPlugin)。保留原作者的 MIT 授權與 Git 歷史，本 Fork 加入字型替換改善。

## 本 Fork 的改動

- 由設定檔指定字型檔名，不再任意選擇第一個 TTF。
- 直接替換啟用中的 TMP 與 Unity UI 文字元件所用字型。
- 同步更新 TMP 共用材質，避免繼續使用原字形圖。
- 建立 2048×2048 動態 TMP 字形圖，啟用多張字形圖。
- 預設每兩秒處理後來開啟的文字元件。
- 初始化或套用發生錯誤時，停止後續處理並記錄原因。
- 不執行固定文字的啟動字形測試。

本插件不翻譯文字、不修改圖片，也不修改 XUnity/Sakura 翻譯流程。

## 安裝與設定

關閉遊戲，將對應 DLL 放入 `BepInEx/plugins`。保留原插件 GUID `com.github.you9you.ttfloader`，不要同時載入原版與本版。

| 版本 | DLL | 環境 |
|---|---|---|
| Mono | `TTFLoaderMono.dll` | BepInEx 5 |
| IL2CPP | `TTFLoaderIL2CPP.dll` | BepInEx 6、Il2CppInterop 1.5.3 |

字型檔由使用者自行提供。設定檔為 `BepInEx/config/com.github.you9you.ttfloader.cfg`，範例如下：

```ini
[General]
Enabled = true
FontFile = LXGW.ttf
ScanIntervalSeconds = 2
```

`FontFile` 為相對遊戲根目錄的路徑，請改為實際字型檔名。首次預設值：Mono 是 `微軟正黑體.ttf`，IL2CPP 是 `LXGW.ttf`；既有設定不會被覆蓋。更改字型後重新啟動遊戲。

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

保留上游 solution，版本更新為 1.2.1。使用 .NET SDK 時，可用 `-p:UnityReferenceDirectory=...` 指定 Unity/TMP 組件目錄。Mono 與 IL2CPP 必須各自使用正確類型的組件，請分開建置。

上游隨附 `libs` 保持原樣，並非目標遊戲的完整依賴。新增的 UI/UIModule/TextCore 引用需要依環境補齊。

上游 GitHub Actions 尚未用本 Fork 所需依賴完成驗證。已使用 `build.ps1` 建置並發布 [v1.2.1 Pre-release](https://github.com/guanting916/BepInEx-TTFLoaderPlugin/releases/tag/v1.2.1)，附件不包含字型檔。

## 相容性與驗證

- 兩版均使用目前 Unity 6 相關組件編譯，不保證所有 Unity/TMP 版本相容。
- IL2CPP 應使用目標遊戲產生的 interop；尚未完成遊戲內驗證。
- 換字型及材質可能改變字寬、描邊、陰影與排版。
- 本 Fork 未保留上游 README 所述 `Instance.LoadTTF/LoadTMPTTF` 公開 API；依賴這些 API 的外掛需另行處理。

## 來源與授權

原作者：游游（you9you）。上游：[BepInEx-TTFLoaderPlugin](https://github.com/you9you/BepInEx-TTFLoaderPlugin)。

程式碼沿用 [MIT License](LICENSE)。字型、Unity、BepInEx 等依賴適用各自授權。