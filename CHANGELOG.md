# Fork 改動紀錄

## 1.2.1 — 2026-10-26 整理上傳

- 整合本機維護的 Mono／IL2CPP 1.2.1 字型替換實作。
- 加入 Enabled、FontFile、ScanIntervalSeconds 設定。
- 替換啟用中的 TMP／Unity UI 元件字型與 TMP 共用材質。
- 建立 2048×2048 動態字形圖，支援多張字形圖及定期掃描。
- IL2CPP 在 Unity 主執行緒初始化並套用字型。
- 移除固定文字的啟動字形測試與相關日誌。
- 更新專案版本、IL2CPP 目標框架與 Unity/UI 引用，加入建置腳本。
- 保留原插件 GUID、原作者授權與上游歷史。

本次沒有修改上游儲存庫，也沒有建立 Release。