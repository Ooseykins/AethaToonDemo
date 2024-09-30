using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AethaToonMaterialEditor : ShaderGUI
{
    private const string BoxStyle = "box";

    private static bool _foldoutEditorSettings = true;
    private static bool _foldoutMainTexture = true;
    private static bool _foldoutLightAndShadow = true;
    private static bool _foldoutPbr = true;
    private static bool _foldout2DRimLight = true;
    private static bool _foldoutRimLight = true;
    private static bool _foldoutMatCap = true;
    private static bool _foldoutStylize = true;
    private static bool _foldoutStencil = true;
    private static bool _foldoutOutline = true;

    private const string KeywordFresnelTint = "_AETHA_FRESNEL";
    private const string KeywordPbr = "_AETHA_PBR";
    private const string KeywordGlitter = "_AETHA_GLITTER";
    private const string KeywordRim = "_AETHA_RIM";

    private static bool _showCopyPropertyName = false;
    private static bool ShowResetPropertyButton
    {
        get => EditorPrefs.GetBool("AethaToonResetPropertyButton");
        set => EditorPrefs.SetBool("AethaToonResetPropertyButton", value);
    }

    private const string EditorPrefLanguageName = "AethaToonLanguage";
    private const string DefaultLanguage = "en-us";
    
    private static string CurrentLanguage
    {
        get
        {
            string lang = EditorPrefs.GetString(EditorPrefLanguageName);
            return string.IsNullOrEmpty(lang) ? DefaultLanguage : lang;
        }
        set => EditorPrefs.SetString(EditorPrefLanguageName, value);
    }
    private static readonly Dictionary<string, Dictionary<string, string>> Localizations = new();
    
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material[] targetMats = materialEditor.targets.Select(x => (Material)x).ToArray();
        if (targetMats.Length == 0)
        {
            EditorGUILayout.LabelField("Selected material is null");
            return;
        }
        
        MaterialProperty p; // Used for the HasProperty(...) and Property(...) functions

        // EDITOR SETTINGS
        _foldoutEditorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutEditorSettings, Loc("HeaderEditorSettings"));
        if (_foldoutEditorSettings)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;

            if (Localizations.Count == 0)
            {
                LoadLanguages();
            }

            // LANGUAGE
            if (EditorGUILayout.DropdownButton(new GUIContent(Loc("SelectLanguage")), FocusType.Keyboard))
            {
                LoadLanguages();
                GenericMenu menu = new GenericMenu();
                foreach (var kv in Localizations)
                {
                    string endonym = kv.Key;
                    if (kv.Value.ContainsKey("Endonym"))
                    {
                        endonym = kv.Value["Endonym"];
                    }

                    menu.AddItem(new GUIContent(endonym), false, () => CurrentLanguage = kv.Key);
                }

                menu.ShowAsContext();
            }

            var note = Loc("TranslationNote");
            if (note != "TranslationNote" && !string.IsNullOrEmpty(note))
            {
                EditorGUILayout.LabelField(note);
            }

            // OPTIONS
            _showCopyPropertyName = EditorGUILayout.ToggleLeft(Loc("OptionShowCopyButton"), _showCopyPropertyName);
            //_showExperimentalFeatures = EditorGUILayout.ToggleLeft(Loc("OptionShowExperimental"), _showExperimentalFeatures);
            ShowResetPropertyButton = EditorGUILayout.ToggleLeft(Loc("OptionShowResetButton"), ShowResetPropertyButton);

            // Demo settings
            Shader fullVersionShader = Shader.Find("Aetha/AethaToon");
            Shader demoVersionShader = Shader.Find("Aetha/AethaToonDemo");
            Shader fullVersionTransparentShader = Shader.Find("Aetha/AethaToonTransparent");
            Shader demoVersionTransparentShader = Shader.Find("Aetha/AethaToonTransparentDemo");
            
            GUILayout.Space(10);
            EditorGUILayout.LabelField(Loc("VERSION"));
            if (!fullVersionShader || !fullVersionTransparentShader)
            {
                EditorGUILayout.LabelField(Loc("DemoVersionText"));
            }
            else
            {
                EditorGUILayout.LabelField(Loc("FullVersionText"));
            }
            if (EditorGUILayout.LinkButton("https://oose.itch.io/aethatoon"))
            {
                Application.OpenURL("https://oose.itch.io/aethatoon");
            }
            if (EditorGUILayout.LinkButton("https://github.com/Ooseykins/AethaToonDemo"))
            {
                Application.OpenURL("https://github.com/Ooseykins/AethaToonDemo");
            }
            
            FromToButton(fullVersionShader, demoVersionShader, Loc("OptionSetToDemo"));
            FromToButton(demoVersionShader, fullVersionShader, Loc("OptionSetToRetail"));
            FromToButton(fullVersionTransparentShader, demoVersionTransparentShader, Loc("OptionSetToDemo"));
            FromToButton(demoVersionTransparentShader, fullVersionTransparentShader, Loc("OptionSetToRetail"));
            
            void FromToButton(Shader from, Shader to, string label)
            {
                if (from != null && to != null && targetMats.All(x => x.shader == from))
                {
                    if (GUILayout.Button(label))
                    {
                        Undo.RecordObjects(targetMats, Loc("UndoConvertEntry"));
                        foreach (var m in targetMats)
                        {
                            m.shader = to;
                        }
                    }
                }
            }
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        

        // MAIN TEXTURE
        _foldoutMainTexture = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMainTexture, Loc("HeaderMainTexture"));
        if (_foldoutMainTexture)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            Property("_MainTex");
            Property("_Opacity");
            Property("_Cutoff");
            Property("_Tint");
            var useFresnelTint = Property(KeywordFresnelTint);
            if (useFresnelTint)
            {
                EditorGUI.indentLevel++;
                if (Property("_FresnelTintBlendMode"))
                {
                    Property("_FresnelTint");
                    Property("_FresnelTintBias");
                    Property("_FresnelTintPower");
                }
                EditorGUI.indentLevel--;
            }
            Property("_Normals");
            Property("_NormalScale");
            Property("_Emissive");
            Property("_EmissiveTint");
            Property("_ReducePerspective");
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // LIGHT AND SHADOW
        _foldoutLightAndShadow = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutLightAndShadow, Loc("HeaderLightAndShadow"));
        if (_foldoutLightAndShadow)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUILayout.LabelField(Loc("HeaderLightColors"));
            EditorGUI.indentLevel++;
            Property("_IndirectDiffuseTint"); 
            Property("_ShadowTint");
            Property("_DirectionalLightTint");
            Property("_LightTint");
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUILayout.LabelField(Loc("HeaderShadowBlending"));
            EditorGUI.indentLevel++;
            Property("_ShadowStart");
            EditorGUI.indentLevel++;
            Property("_FixDeepShadows");
            EditorGUI.indentLevel--;
            Property("_DirectionalShadowSoftness");
            Property("_ShadowSoftness");
            Property("_ShadowmapSoftness");
            Property("_SoftenIndirectLight");
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUILayout.LabelField(Loc("HeaderFadeShadow"));
            EditorGUI.indentLevel++;
            Property("_MinimumShadowValue");
            Property("_MinimumCastShadowValue");
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // PBR
        _foldoutPbr = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutPbr, Loc("HeaderPBR"));
        if (_foldoutPbr)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            var usePbr = KeywordProperty(KeywordPbr);
            if (usePbr)
            {
                EditorGUI.indentLevel++;
                Property("_PBRMult");
                Property("_Metallic");
                Property("_Smoothness");
                Property("_PBRCatchReflections");
                Property("_PBRCubemapIntensity");
                var useGlitter = KeywordProperty(KeywordGlitter);
                if (useGlitter)
                {
                    EditorGUI.indentLevel++;
                    Property("_PBRGlitterIntensity");
                    Property("_PBRGlitterNoiseScale");
                    EditorGUI.indentLevel--;
                }
                Property("_PBRMask");
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        
        
        if (HasProperty("_OutlineWidth"))
        {
            // Outline
            _foldoutOutline = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutOutline, Loc("HeaderOutline"));
            if (_foldoutOutline)
            {
                EditorGUILayout.BeginVertical(BoxStyle);
                EditorGUI.indentLevel++;
                Property("_OutlineWidth");
                Property("_OutlineColor");
                Property("_OutlineMask");
                Property("_OutlineUseLighting");
                Property("_BackFaceFlipSelfShadow");
                Property("_OutlineDetailAdjust");
                Property("_OutlineFadeStart");
                Property("_OutlineFadeEnd");
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        // Matcap
        _foldoutMatCap = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutMatCap, Loc("HeaderMatcap"));
        if (_foldoutMatCap)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            Property("_Matcap");
            Property("_MatcapStrength");
            Property("_MatcapEmissive");
            Property("_MatcapEmissiveStrength");
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // 2D RimLights
        _foldout2DRimLight = EditorGUILayout.BeginFoldoutHeaderGroup(_foldout2DRimLight, Loc("Header2DRimLights"));
        if (_foldout2DRimLight)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            if (Property("_2DRimLightBlendMode"))
            {
                Property("_2DRimLightColor");
                Property("_2DRimLightMainTextureMix");
                Property("_2DRimLightDepth");
                Property("_2DRimLightSoftness");
                Property("_2DRimLightWorldOffset");
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            if (Property("_2DRimLightBlendModeAuto"))
            {
                Property("_2DRimLightAutoColorDirectional");
                Property("_2DRimLightAutoColor");
                Property("_2DRimLightAutoStrengthDirectional");
                Property("_2DRimLightAutoStrength");
                Property("_2DRimLightAutoMainTextureMix");
                Property("_2DRimLightAutoDepth");
                Property("_2DRimLightAutoSoftness");
                Property("_2DRimLightAutoOffset");
                Property("_2DRimLightAutoTaper");
            }
            EditorGUI.indentLevel--; 
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // RimLights
        _foldoutRimLight = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutRimLight, Loc("HeaderRimLights"));
        if (_foldoutRimLight)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            var useRim = KeywordProperty(KeywordRim);
            if (useRim)
            {
                EditorGUILayout.BeginVertical(BoxStyle);
                EditorGUILayout.LabelField("1");
                EditorGUI.indentLevel++;
                Property("_Rim1Color");
                Property("_Rim1Power");
                Property("_Rim1Bias");
                Property("_Rim1Start");
                Property("_Rim1Softness");
                
                EditorGUILayout.BeginVertical(BoxStyle);
                EditorGUILayout.LabelField(Loc("HeaderRimLightVectors"));
                EditorGUI.indentLevel++;
                Property("_Rim1WorldDirection");
                Property("_Rim1CameraDirection");
                Property("_Rim1ViewWeight");
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.LabelField("2");
                EditorGUI.indentLevel++;
                Property("_Rim2Color");
                Property("_Rim2Power");
                Property("_Rim2Bias");
                Property("_Rim2Start");
                Property("_Rim2Softness");
                EditorGUILayout.BeginVertical(BoxStyle);
                EditorGUILayout.LabelField(Loc("HeaderRimLightVectors"));
                EditorGUI.indentLevel++;
                Property("_Rim2WorldDirection");
                Property("_Rim2CameraDirection");
                Property("_Rim2ViewWeight");
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
                Property("_RimMask");
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        Shader.SetGlobalFloat("TIME_TRIAL", Time.timeSinceLevelLoad);
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Stylize
        _foldoutStylize = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutStylize, Loc("HeaderStylized"));
        if (_foldoutStylize)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            var useStylized = ToggleProperty("_Stylized");
            if (useStylized)
            {
                EditorGUI.indentLevel++;
                Property("_StylizedTexture");
                Property("_StylizedTextureScale");
                Property("_FresnelTintUseStylized");
                Property("_UseStylizedShadow");
                Property("_Rim1Stylized");
                Property("_Rim2Stylized");
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Stencil
        _foldoutStencil = EditorGUILayout.BeginFoldoutHeaderGroup(_foldoutStencil, Loc("HeaderStencil"));
        if (_foldoutStencil)
        {
            EditorGUILayout.BeginVertical(BoxStyle);
            EditorGUI.indentLevel++;
            materialEditor.RenderQueueField();
            Property("_CullMode");
            EditorGUILayout.LabelField(Loc("HeaderStencilSettings"));
            EditorGUI.indentLevel++;
            Property("_StencilReference");
            Property("_StencilWriteMask");
            Property("_StencilReadMask");
            Property("_StencilComparison");
            Property("_StencilPass");
            Property("_StencilFail");
            Property("_StencilZFail");
            EditorGUI.indentLevel--;
            
            // Presets
            GUILayout.Space(5);
            if (EditorGUILayout.DropdownButton(new GUIContent(Loc("StencilPresetDropdownLabel")), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent(Loc("StencilPresetDefault")), false , () => PresetDefault(targetMats));
                menu.AddItem(new GUIContent(Loc("StencilPresetEye")), false , () => PresetEyes(targetMats));
                menu.AddItem(new GUIContent(Loc("StencilPresetHair")), false , () => PresetHairEyes(targetMats));
                menu.AddItem(new GUIContent(Loc("StencilPresetHairTransparent")), false , () => PresetHairEyesTransparent(targetMats));
                menu.AddItem(new GUIContent(Loc("StencilPresetFakeHairShadow")), false , () => PresetFakeHairShadow(targetMats));
                menu.ShowAsContext();
            }
            
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        bool Property(string name)
        {
            if (HasProperty(name))
            {
                string label = Loc(name);
                string tooltip = Loc(name + "Tooltip");
                GUILayout.BeginHorizontal();
                GUIContent c = new GUIContent(label);
                if (!string.IsNullOrEmpty(tooltip) && tooltip != name+"Tooltip")
                {
                    c = new GUIContent(label, tooltip);
                }
                materialEditor.ShaderProperty(p, c);
                
                if (ShowResetPropertyButton &&
                    p.type is 
                        MaterialProperty.PropType.Float or 
                        MaterialProperty.PropType.Int or 
                        MaterialProperty.PropType.Color or 
                        MaterialProperty.PropType.Vector or 
                        MaterialProperty.PropType.Range)
                {
                    if (GUILayout.Button(Loc("↺"), GUILayout.Width(32)))
                    {
                        foreach (Material m in targetMats)
                        {
                            ResetMaterialProperties(m, name);
                        }
                    }
                }
                if (_showCopyPropertyName)
                {
                    if (GUILayout.Button(name, GUILayout.Width(200)))
                    {
                        GUIUtility.systemCopyBuffer = name;
                    }
                }

                GUILayout.EndHorizontal();
            }

            if (p is { type: MaterialProperty.PropType.Float or MaterialProperty.PropType.Int })
            {
                return p.floatValue > 0.5f || p.hasMixedValue;
            }
            return true;
        }
        
        bool ToggleProperty(string name)
        {
            if (HasProperty(name))
            {
                string label = Loc(name);
                string tooltip = Loc(name + "Tooltip");
                GUIContent c = new GUIContent(label);
                if (!string.IsNullOrEmpty(tooltip) && tooltip != name+"Tooltip")
                {
                    c = new GUIContent(label, tooltip);
                }
                materialEditor.ShaderProperty(p, c);
                return p.floatValue > 0.5f || p.hasMixedValue;
            }
            return false;
        }
        
        bool KeywordProperty(string name)
        {
            if (HasProperty(name))
            {
                string label = Loc(name);
                string tooltip = Loc(name + "Tooltip");
                GUIContent c = new GUIContent(label);
                if (!string.IsNullOrEmpty(tooltip) && tooltip != name+"Tooltip")
                {
                    c = new GUIContent(label, tooltip);
                }
                materialEditor.ShaderProperty(p, c);
                return p.floatValue > 0.5f || p.hasMixedValue;
            }
            return false;
        }

        bool HasProperty(string name)
        {
            p = properties.FirstOrDefault(x => x.name == name);
            return p != null;
        }
        
        
    }

    static void ResetMaterialProperties(Material material, params string[] properties)
    {
        Undo.RecordObject(material,Loc("UndoResetEntry")+material.name);
        foreach (var p in properties)
        {
            int i = material.shader.FindPropertyIndex(p);
            if (i >= 0)
            {
                if (material.HasVector(p))
                {
                    material.SetVector(p,material.shader.GetPropertyDefaultVectorValue(i));
                }
                if (material.HasColor(p))
                {
                    material.SetColor(p,material.shader.GetPropertyDefaultVectorValue(i));
                }
                if (material.HasFloat(p))
                {
                    material.SetFloat(p,material.shader.GetPropertyDefaultFloatValue(i));
                }
                if (material.HasInt(p))
                {
                    material.SetInt(p,(int)material.shader.GetPropertyDefaultFloatValue(i));
                }
                if (material.HasInteger(p))
                {
                    material.SetInt(p,(int)material.shader.GetPropertyDefaultFloatValue(i));
                }
            }
        }
    }
    
    static void PresetDefault(params Material[] materials)
    {
        Undo.RecordObjects(materials, "AethaToon stencil defaults");
        foreach (var m in materials)
        {
            PresetStencil(m, 0, 0, 0, 0, 0, 0, 0);
            m.renderQueue = m.shader.renderQueue;
        }
    }
    
    static void PresetEyes(params Material[] materials)
    {
        Undo.RecordObjects(materials, "AethaToon stencil eye preset");
        foreach (var m in materials)
        {
            PresetStencil(m, 255, 128, 128, 8, 2, 0, 0);
            m.renderQueue = m.shader.renderQueue + 10;
        }
    }
    
    static void PresetHairEyes(params Material[] materials)
    {
        Undo.RecordObjects(materials, "AethaToon stencil hair preset");
        foreach (var m in materials)
        {
            PresetStencil(m, 255, 128, 128, 6, 0, 0, 0);
            m.renderQueue = m.shader.renderQueue + 15;
        }
    }
    
    static void PresetHairEyesTransparent(params Material[] materials)
    {
        Undo.RecordObjects(materials, "AethaToon stencil hair preset");
        foreach (var m in materials)
        {
            PresetStencil(m, 255, 128, 128, 3, 0, 0, 0);
            m.renderQueue = m.shader.renderQueue;
        }
    }
    
    static void PresetFakeHairShadow(params Material[] materials)
    {
        Undo.RecordObjects(materials, "AethaToon fake hair shadow, lit preset");
        foreach (var m in materials)
        {
            PresetStencil(m, 1, 64, 64, 3, 0, 0, 0);
            m.renderQueue = m.shader.renderQueue + 10;
        }
    }
    
    static void PresetStencil(Material m, int stencilRef, int stencilWrite, int stencilRead, int stencilComparison, int stencilPass, int stencilFail, int stencilZFail)
    {
        m.SetInt("_StencilReference", stencilRef);
        m.SetInt("_StencilWriteMask", stencilWrite);
        m.SetInt("_StencilReadMask", stencilRead);
        m.SetFloat("_StencilComparison", stencilComparison);
        m.SetFloat("_StencilPass", stencilPass);
        m.SetFloat("_StencilFail", stencilFail);
        m.SetFloat("_StencilZFail", stencilZFail);
    }

    static string Loc(string key)
    {
        if (Localizations.ContainsKey(CurrentLanguage) && Localizations[CurrentLanguage].ContainsKey(key))
        {
            return Localizations[CurrentLanguage][key];
        }
        return key;
    }

    static void LoadLanguages()
    {
        foreach (var path in AssetDatabase.FindAssets("t:TextAsset").Select(AssetDatabase.GUIDToAssetPath).Where(x => x.Contains("Localization") && x.Contains("AethaToon")))
        {
            LoadLanguage(AssetDatabase.LoadAssetAtPath<TextAsset>(path));
        }
    }

    static void LoadLanguage(TextAsset textAsset)
    {
        string languageName = textAsset.name.Split('.').First();
        Localizations[languageName] = new Dictionary<string, string>();
        string[] lines = textAsset.text.Split(
            new [] { "\r\n", "\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries
        );
        foreach (var line in lines)
        {
            int delimiterIndex = line.IndexOf('=');
            if (delimiterIndex < 1)
            {
                continue;
            }
            string key = line.Substring(0, delimiterIndex);
            string value = line.Substring(delimiterIndex+1);
            Localizations[languageName][key] = value;
        }
    }
}
