# BepInEx TTF Font Loader

本專案的目標是讓 Unity 遊戲的文字元件使用**指定的 TTF 字型**，改善原字型缺少中文字形時的文字顯示。

專案 Fork 自 [you9you/BepInEx-TTFLoaderPlugin](https://github.com/you9you/BepInEx-TTFLoaderPlugin)，保留原作者授權與上游歷史，並擴充 Mono 與 IL2CPP 環境的字型替換支援。

## 主要功能

- **替換文字元件字型**：支援 TextMeshPro（TMP）與 Unity UI 文字元件，使用自行準備的 TTF 字型。
- **動態建立字形**：使用動態字形圖與多張字形圖，載入文字顯示所需的字形。
- **處理新出現的文字元件**：定期掃描目前啟用的文字元件，套用指定字型。
- **支援 Mono 與 IL2CPP**：提供對應 BepInEx 環境的插件版本。
- **執行時替換**：在遊戲執行期間套用字型，保留遊戲原始資源檔。

## 目前限制

- 字型檔需自行準備，且必須包含所需字形；本專案不附字型。
- 功能限於支援的文字元件，不處理圖片中的文字，也不提供翻譯功能。
- 字型替換可能影響字寬、描邊與排版，無法解決所有裁切或逐字顯示問題。
- 遊戲使用的 Unity、TMP 或文字框架不同時，可能需要額外調整。

## 開發與授權

本專案透過 **GPT6.1 SOL** 協助建置與編譯。建置方式請參閱 [BUILDING.md](BUILDING.md)。

原作者：游游（you9you）。程式碼沿用原作者 [MIT License](LICENSE)。字型、Unity、BepInEx 等依賴適用各自授權；本插件不授予其再散布權。
