using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

[assembly: System.Reflection.AssemblyVersion("1.2.2.0")]

namespace TTFLoaderIL2CPP
{
    [BepInPlugin("com.github.you9you.ttfloader", "TTF Font Loader (IL2CPP)", "1.2.2")]
    public sealed class TTFLoaderPlugin : BasePlugin
    {
        internal static FontRuntime Runtime;

        public override void Load()
        {
            ConfigEntry<bool> enabled = Config.Bind<bool>("General", "Enabled", true,
                "Apply the selected font to active TMP and Unity UI labels.");
            ConfigEntry<string> fontFile = Config.Bind<string>("General", "FontFile", "",
                "Font file relative to the game directory. Leave blank to skip font replacement; set a file name and restart.");
            ConfigEntry<float> interval = Config.Bind<float>("General", "ScanIntervalSeconds", 2f,
                "Scan for newly opened UI; minimum 1 second.");
            Runtime = new FontRuntime(Log, enabled, fontFile, interval);
            if (!enabled.Value) { Log.LogInfo("Font loader disabled in configuration."); return; }
            if (string.IsNullOrWhiteSpace(fontFile.Value))
            {
                Log.LogInfo("FontFile 尚未設定，已跳過字型替換。請將 TTF 放在遊戲執行檔旁，設定 FontFile 後重新啟動遊戲。");
                return;
            }
            try
            {
                // BepInEx registers this type and attaches it to its persistent manager.
                AddComponent<FontUpdater>();
                Log.LogInfo("IL2CPP font updater registered; initialization will run on the Unity thread.");
            }
            catch (Exception ex) { Log.LogError("Cannot register IL2CPP font updater: " + ex); }
        }
    }

    // Keep managed configuration/state outside the injected class.
    public sealed class FontUpdater : MonoBehaviour
    {
        public FontUpdater(IntPtr pointer) : base(pointer) { }
        public void Update()
        {
            if (TTFLoaderPlugin.Runtime != null) TTFLoaderPlugin.Runtime.Tick();
        }
    }

    internal sealed class FontRuntime
    {
        private readonly ManualLogSource log;
        private readonly ConfigEntry<bool> enabled;
        private readonly ConfigEntry<string> fontFile;
        private readonly ConfigEntry<float> interval;
        private Font sourceFont;
        private TMP_FontAsset tmpFont;
        private bool initialized;
        private bool stopped;
        private float nextScan = 1f;

        internal FontRuntime(ManualLogSource logger, ConfigEntry<bool> enabledSetting,
            ConfigEntry<string> fileSetting, ConfigEntry<float> intervalSetting)
        {
            log = logger; enabled = enabledSetting; fontFile = fileSetting; interval = intervalSetting;
        }

        internal void Tick()
        {
            if (stopped || !enabled.Value || Time.realtimeSinceStartup < nextScan) return;
            float seconds = interval.Value;
            if (float.IsNaN(seconds) || float.IsInfinity(seconds)) seconds = 2f;
            nextScan = Time.realtimeSinceStartup + Mathf.Max(1f, seconds);
            try
            {
                if (!initialized && !Initialize()) { stopped = true; return; }
                ApplyFonts();
            }
            catch (Exception ex)
            {
                stopped = true;
                log.LogError("IL2CPP font updater stopped after error: " + ex);
            }
        }

        private bool Initialize()
        {
            string path = Path.GetFullPath(Path.Combine(Paths.GameRootPath, fontFile.Value));
            if (!File.Exists(path)) throw new FileNotFoundException("Font file not found", path);
            sourceFont = new Font(path);
            if (sourceFont == null) throw new InvalidOperationException("Unity could not load the font file.");
            UnityEngine.Object.DontDestroyOnLoad(sourceFont);
            tmpFont = TMP_FontAsset.CreateFontAsset(sourceFont, 48, 6, GlyphRenderMode.SDFAA,
                2048, 2048, AtlasPopulationMode.Dynamic, true);
            if (tmpFont == null) throw new InvalidOperationException("TMP font creation failed.");
            tmpFont.name = "TTFLoader_TraditionalChinese_Dynamic_IL2CPP";
            tmpFont.isMultiAtlasTexturesEnabled = true;
            UnityEngine.Object.DontDestroyOnLoad(tmpFont);
            TMP_Settings.defaultFontAsset = tmpFont;
            var fallbacks = TMP_Settings.fallbackFontAssets;
            if (fallbacks == null)
            {
                fallbacks = new Il2CppSystem.Collections.Generic.List<TMP_FontAsset>();
                TMP_Settings.fallbackFontAssets = fallbacks;
            }
            if (!fallbacks.Contains(tmpFont)) fallbacks.Add(tmpFont);
            initialized = true;
            log.LogInfo("IL2CPP font loader 1.2.2 ready: replaces actual component fonts; font=" + path);
            return true;
        }

        private void ApplyFonts()
        {
            int tmpCount = 0;
            int uiCount = 0;
            foreach (TMP_Text label in UnityEngine.Object.FindObjectsOfType<TMP_Text>())
            {
                if (label == null || !label.isActiveAndEnabled || label.font == tmpFont) continue;
                label.font = tmpFont;
                label.fontSharedMaterial = tmpFont.material;
                label.SetAllDirty();
                tmpCount++;
            }
            foreach (Text label in UnityEngine.Object.FindObjectsOfType<Text>())
            {
                if (label == null || !label.isActiveAndEnabled || label.font == sourceFont) continue;
                label.font = sourceFont;
                label.SetAllDirty();
                uiCount++;
            }
            if (tmpCount + uiCount > 0) log.LogInfo("Font applied: TMP=" + tmpCount + ", UGUI=" + uiCount + ".");
        }
    }
}
