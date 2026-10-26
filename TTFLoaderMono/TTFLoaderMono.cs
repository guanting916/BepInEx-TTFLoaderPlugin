using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace TTFLoaderMono
{
    // Replacement for the original plugin. Keep its identity to avoid two loaders.
    [BepInPlugin("com.github.you9you.ttfloader", "TTF Font Loader (Mono)", "1.2.1")]
    public sealed class TTFLoaderPlugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> loaderEnabled;
        private ConfigEntry<string> fontFile;
        private ConfigEntry<float> scanInterval;
        private Font sourceFont;
        private TMP_FontAsset tmpFont;
        private float nextScan;
        private bool ready;
        private bool stopped;

        private void Awake()
        {
            loaderEnabled = Config.Bind<bool>("General", "Enabled", true, "Apply the selected font to active TMP and Unity UI labels.");
            fontFile = Config.Bind<string>("General", "FontFile", "微軟正黑體.ttf", "Font file relative to the game directory.");
            scanInterval = Config.Bind<float>("General", "ScanIntervalSeconds", 2f, "Scan for newly opened UI; minimum 1 second.");
        }

        private void Start()
        {
            if (!loaderEnabled.Value) return;
            try
            {
                string path = Path.GetFullPath(Path.Combine(Paths.GameRootPath, fontFile.Value));
                if (!File.Exists(path)) throw new FileNotFoundException("Font file not found", path);
                sourceFont = new Font(path);
                if (sourceFont == null) throw new InvalidOperationException("Unity could not load the font file.");
                UnityEngine.Object.DontDestroyOnLoad(sourceFont);
                tmpFont = TMP_FontAsset.CreateFontAsset(sourceFont, 48, 6, GlyphRenderMode.SDFAA,
                    2048, 2048, AtlasPopulationMode.Dynamic, true);
                if (tmpFont == null) throw new InvalidOperationException("TMP font creation failed.");
                tmpFont.name = "TTFLoader_TraditionalChinese_Dynamic";
                tmpFont.isMultiAtlasTexturesEnabled = true;
                UnityEngine.Object.DontDestroyOnLoad(tmpFont);

                TMP_Settings.defaultFontAsset = tmpFont;
                List<TMP_FontAsset> fallbacks = TMP_Settings.fallbackFontAssets;
                if (fallbacks == null)
                {
                    fallbacks = new List<TMP_FontAsset>();
                    TMP_Settings.fallbackFontAssets = fallbacks;
                }
                if (!fallbacks.Contains(tmpFont)) fallbacks.Add(tmpFont);
                ready = true;
                nextScan = Time.realtimeSinceStartup + 1f;
                Logger.LogInfo("Font loader 1.2.1 ready: replaces actual component fonts; font=" + path);
            }
            catch (Exception ex)
            {
                stopped = true;
                Logger.LogError("Font initialization failed; existing UI fonts left unchanged: " + ex);
            }
        }

        private void Update()
        {
            if (!ready || stopped || !loaderEnabled.Value || Time.realtimeSinceStartup < nextScan) return;
            float seconds = scanInterval.Value;
            if (float.IsNaN(seconds) || float.IsInfinity(seconds)) seconds = 2f;
            nextScan = Time.realtimeSinceStartup + Mathf.Max(1f, seconds);
            try
            {
                int tmpCount = 0;
                int uiCount = 0;
                foreach (TMP_Text label in UnityEngine.Object.FindObjectsOfType(typeof(TMP_Text)))
                {
                    if (label == null || !label.isActiveAndEnabled || label.font == tmpFont) continue;
                    label.font = tmpFont;
                    // An existing material preset can still point at the old atlas.
                    label.fontSharedMaterial = tmpFont.material;
                    label.SetAllDirty();
                    tmpCount++;
                }
                foreach (Text label in UnityEngine.Object.FindObjectsOfType(typeof(Text)))
                {
                    if (label == null || !label.isActiveAndEnabled || label.font == sourceFont) continue;
                    label.font = sourceFont;
                    label.SetAllDirty();
                    uiCount++;
                }
                if (tmpCount + uiCount > 0)
                    Logger.LogInfo("Font applied: TMP=" + tmpCount + ", UGUI=" + uiCount + ".");
            }
            catch (Exception ex)
            {
                stopped = true;
                Logger.LogError("Font application stopped after error: " + ex);
            }
        }
    }
}
