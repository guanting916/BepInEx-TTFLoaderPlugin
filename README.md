# BepInEx TTF Font Loader

替 Unity 遊戲的文字元件指定 TTF 字型。這是字型替換插件，不是翻譯器；它不會把日文翻成中文，也不處理圖片中的文字。

本專案透過 **GPT6.1 SOL** 協助建置與編譯。Fork 自 [you9you/BepInEx-TTFLoaderPlugin](https://github.com/you9you/BepInEx-TTFLoaderPlugin)，保留原作者授權與上游歷史。

## 1. 安裝前先準備

- Windows Unity 遊戲，以及適用該遊戲的 BepInEx 環境。
- 一個你自行準備、含所需中文字形的 `.ttf` 字型檔。**本專案及 Release 不附字型。**
- 確認遊戲使用 Mono 或 IL2CPP，下載對應 DLL。

若還沒安裝 BepInEx，先依其[官方安裝說明](https://docs.bepinex.dev/)完成安裝並啟動遊戲一次。能產生 `BepInEx/LogOutput.log` 後，再安裝本插件；遊戲本身不會自動具備 BepInEx。

| 遊戲環境 | 使用的檔案 |
|---|---|
| BepInEx 5 Mono | `TTFLoaderMono.dll` |
| BepInEx 6 IL2CPP / Il2CppInterop 1.5.3 | `TTFLoaderIL2CPP.dll` |

遊戲根目錄有 `GameAssembly.dll` 通常是 IL2CPP；有 `遊戲名_Data/Managed/Assembly-CSharp.dll` 通常是 Mono。不確定時，先確認該遊戲適用的 BepInEx 類型，不要把兩份 DLL 都放進去。

## 2. 下載及放置 DLL

到 [Release 下載頁](https://github.com/guanting916/BepInEx-TTFLoaderPlugin/releases/tag/v1.2.2)，從 **Assets** 下載對應 DLL。一般安裝不需要下載 GitHub 自動附上的 `Source code`，也不需要自行編譯。

關閉遊戲，將 DLL 放入該遊戲的 `BepInEx/plugins`。資料夾不存在時可自行建立。若已有舊版字型插件，先移到遊戲目錄外備份，避免同時載入相同插件。

## 3. 放置字型檔

這份說明的「遊戲根目錄」是**遊戲 `.exe` 所在的資料夾**，不是存檔位置，也不是 `BepInEx/plugins`。

例如你準備的字型檔叫 `MyFont.ttf`，請這樣放：

```text
遊戲資料夾/
├── Game.exe
├── MyFont.ttf                ← 自行準備的字型
└── BepInEx/
    ├── plugins/
    │   └── 對應的字型插件.dll
    └── config/
        └── com.github.you9you.ttfloader.cfg
```

上圖的 `Game.exe`、`MyFont.ttf` 和「對應的字型插件.dll」是示意名稱，請使用實際遊戲、字型及 DLL 檔名。字型不必改名成某個指定名稱。

## 4. 產生及修改設定

1. 放好 DLL 後，啟動遊戲一次，再正常關閉。
2. 插件會自動建立 `BepInEx/config/com.github.you9you.ttfloader.cfg`，**通常不需要手動建立設定檔**。
3. 用記事本開啟該檔案，找到 `[General]`，將 `FontFile` 改成實際字型檔名。
4. 儲存後重新啟動遊戲。

若字型檔為上面的 `MyFont.ttf`，設定應為：

```ini
[General]
Enabled = true
FontFile = MyFont.ttf
ScanIntervalSeconds = 2
```

- `Enabled`：`true` 啟用，`false` 停用。
- `FontFile`：相對遊戲根目錄的字型路徑。兩版首次預設均留空；未填或只有空白時，跳過字型替換，並記錄一次設定提示。填寫後重新啟動遊戲。
- `ScanIntervalSeconds`：檢查新文字元件的間隔秒數，通常維持 `2` 即可。

**上面是已完成設定的範例，不代表附件包含 `MyFont.ttf`。**v1.2.2 首次產生的設定為 `FontFile =`，不會自行選擇任何字型。若已有舊設定，新版會沿用，不會自動清空或替換使用者的檔名。

字型若放在 `遊戲資料夾/Fonts/MyFont.ttf`，則填 `FontFile = Fonts/MyFont.ttf`。修改後都需要重開遊戲。

## 5. 確認是否生效

進入有文字的畫面後稍候數秒，插件會替換目前啟用的 TMP／Unity UI 文字元件字型。字型替換在執行時進行，不會修改遊戲原始資源檔。

可用記事本開啟 `BepInEx/LogOutput.log`，搜尋 `TTF Font Loader`、`Font loader`、`Font applied` 或 `Font initialization failed`，確認載入與錯誤原因。

## 常見問題

| 狀況 | 檢查方式 |
|---|---|
| 沒有產生設定檔 | 先確認 BepInEx 正常載入，DLL 放對位置且類型正確，再查看 `LogOutput.log`。 |
| 出現找不到字型 | 核對 `FontFile`、副檔名與實際位置；注意不要變成 `MyFont.ttf.ttf`。修正後重開遊戲。 |
| 尚未設定字型 | `FontFile` 留空時是正常跳過，不視為字型載入錯誤。填好實際檔名、儲存並重開遊戲。 |
| 設定後沒變 | 確認已重開遊戲且 `Enabled = true`；查看 LOG。圖片字或不支援的文字框架不會因此更換字型。 |
| 還是有缺字 | 字型本身需包含該字形；並非每種裁切、逐字顯示問題都能靠換字型解決。 |
| 字寬、描邊或排版改變 | 換字型及材質可能影響呈現，請自行確認效果。 |

## 更新、停用與還原

- 更新：先關閉遊戲，替換原 DLL；保留既有 `.cfg`，並確認 `FontFile` 仍正確。
- 停用：設定 `Enabled = false`，再重開遊戲。
- 還原：關閉遊戲，移除本插件或換回先前備份的版本；不需要刪除整個 BepInEx。

## 開發與授權

需要修改或自行編譯時，請參閱 [BUILDING.md](BUILDING.md)。本 Fork 使用動態字形圖、多張字形圖與定期掃描，保留原插件 GUID `com.github.you9you.ttfloader`。

原作者：游游（you9you）。程式碼沿用原作者 [MIT License](LICENSE)。字型、Unity、BepInEx 等依賴適用各自授權；本插件不授予其再散布權。
