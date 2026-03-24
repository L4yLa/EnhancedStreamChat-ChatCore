using BeatSaberMarkupLanguage;
using System.Linq;
using TMPro;
using UnityEngine;

namespace EnhancedStreamChat.Utilities
{
    public static class BeatSaberUtils
    {
        private static Material s_noGlow;
        public static Material UINoGlowMaterial => s_noGlow ??= Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").FirstOrDefault();

        private static Shader s_tmpNoGlowFontShader;
        public static Shader TMPNoGlowFontShader
        {
            get
            {
                if (s_tmpNoGlowFontShader == null && BeatSaberUI.MainTextFont) {
                    s_tmpNoGlowFontShader = BeatSaberUI.MainTextFont.material.shader;
                }
                return s_tmpNoGlowFontShader;
            }
        }

        // DaNike to the rescue 
        public static bool TryGetTMPFontByFamily(string family, out TMP_FontAsset font)
        {
            if (FontManager.TryGetTMPFontByFamily(family, out font)) {
                font.material.shader = TMPNoGlowFontShader;
                return true;
            }

            return false;
        }
    }
}