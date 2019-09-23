#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ArabicDiacriticConfigWindows class used to configure ArabicDiacritic support
/// </summary>
class ArabicDiacriticConfigWindows : EditorWindow {

    // Configuration of the paths
    protected static readonly string PathResources = "Assets/Resources/";
    protected static readonly string PathArabicDiacriticDictionarySO = "ArabicDiacritic/";
    protected static readonly string NameArabicDiacriticDictionarySO = "ArabicDiacriticDictionary";
    protected static readonly string FormatArabicDiacriticDictionarySO = ".asset";


    protected ArabicDiacriticDictionarySO arabicDiacriticDictionary;
    protected ArabicDiacriticSupport arabicDiacriticSupport;

    protected int indexDict = 0;
    private int viewIndex = 0;

    // Variables Window
    protected string searchCharContexteHexa = "";
    protected string searchCharContexteChar = "";

    protected string searchCharUpdateHexa = "";
    protected string searchCharUpdateChar = "";

    protected string textSearchChars = "";
    protected string textSearchHexa = "";

    // Sections toggles
    protected bool showIntro = true;
    protected bool showInstructions = false;
    protected bool showConfig = false;
    protected bool showDebug = false;

    /// <summary>
    /// Create a new asset
    /// </summary>
    /// <returns></returns>
    protected ArabicDiacriticDictionarySO CreateNewAsset() {
        if (!Directory.Exists(PathArabicDiacriticDictionarySO)) {
            Directory.CreateDirectory(PathArabicDiacriticDictionarySO);
        }
        ArabicDiacriticDictionarySO asset = ScriptableObject.CreateInstance<ArabicDiacriticDictionarySO>();
        UnityEditor.AssetDatabase.CreateAsset(asset, PathResources + PathArabicDiacriticDictionarySO + NameArabicDiacriticDictionarySO + FormatArabicDiacriticDictionarySO);
        UnityEditor.AssetDatabase.SaveAssets();
        return asset;
    }

    /// <summary>
    /// On enable of object - open asset and instanciate arabicDiacriticSupport
    /// </summary>
    void OnEnable() {
        LoadAssetEditor();
        arabicDiacriticSupport = new ArabicDiacriticSupport(true);
    }

    /// <summary>
    /// Try to load the asset if it exists otherwise creates it
    /// </summary>
    protected void LoadAssetEditor() {

        Debug.Log("Attempt to load asset : " + PathResources + PathArabicDiacriticDictionarySO + NameArabicDiacriticDictionarySO + FormatArabicDiacriticDictionarySO);

        arabicDiacriticDictionary = AssetDatabase.LoadAssetAtPath<ArabicDiacriticDictionarySO>(PathResources + PathArabicDiacriticDictionarySO + NameArabicDiacriticDictionarySO + FormatArabicDiacriticDictionarySO);
        if (arabicDiacriticDictionary == null) {
            Debug.Log("Asset cannot be found or opened. Create a new one instead.");
            arabicDiacriticDictionary = CreateNewAsset();
        } else {
            Debug.Log("Asset successfully loaded.");
        }



        if (arabicDiacriticDictionary.diacriticConfigs == null || arabicDiacriticDictionary.diacriticConfigs.Count == 0) {
            Debug.Log("Asset is currently empty. It will be filled with default data. (ArabicDiacriticDictionaryInit)");
            arabicDiacriticDictionary.diacriticConfigs = new List<DiacriticConfig>(ArabicDiacriticDictionaryInit.DiacriticConfigs);

            EditorUtility.SetDirty(arabicDiacriticDictionary);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    /// <summary>
    /// Menu item used to access configuration
    /// </summary>
    [MenuItem("ArabicDiacriticSupport/Configuration", false, 300)]
    static void ShowWindow() {
        EditorWindow.GetWindow(typeof(ArabicDiacriticConfigWindows));
    }

    /// <summary>
    /// Display of the window
    /// </summary>
    void OnGUI() {
        this.titleContent.text = "Arabic Diacritic - Configuration";

        DrawUI(Screen.width);
    }

    /// <summary>
    /// Draw default line
    /// </summary>
    protected void DrawLine() {
        GUILayout.Space(5);
        DrawLine(Color.grey);
        GUILayout.Space(10);
    }

    /// <summary>
    /// Draw line
    /// </summary>
    protected void DrawLine(Color color, int thickness = 2, int padding = 10) {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    /// <param name="width"></param>
    /// 
    protected void DrawUI(float width) {
        EditorGUILayout.BeginVertical();

        var margin = (EditorStyles.miniButton.padding.left) / 2f;
        width = width - margin * 2;

        showIntro = EditorGUILayout.Foldout(showIntro, "Introduction");
        if (showIntro) {
            DrawIntro(Screen.width);
            DrawLine();
        }

        showInstructions = EditorGUILayout.Foldout(showInstructions, "Instructions");
        if (showInstructions) {
            DrawInstructions(Screen.width);
            DrawLine();
        }

        showConfig = EditorGUILayout.Foldout(showConfig, "Configuration");
        if (showConfig) {
            DrawConfig(Screen.width);
            DrawLine();
        }

        showDebug = EditorGUILayout.Foldout(showDebug, "Debug");
        if (showDebug) {
            DrawReloadAsset();
            DrawLine();
        }
    }

    /// <summary>
    /// Draw the intro
    /// </summary>
    /// <param name="width"></param>
    protected void DrawIntro(float width) {
        GUILayout.Space(10);
        GUILayout.TextArea("This windows is used to configure the ArabicDiacritic plugin. This plugin uses this configuration in order to correct the positions (with values in x and y) of Arabic diacritic depending of an Arabic character.");
        GUILayout.Space(10);
        GUILayout.TextArea("The sentence is always analyzed from right to left. And each character is definied using its Hexa Char code.");
        GUILayout.Space(10);
        GUILayout.TextArea("Each configuration is definied as follow : \n" +
            " - Char Context (Hexa Char Unicode) : The chacter giving context to the Diacritic that needs to be corrected.\n" +
            " - Char to update (Hexa Char Unicode) : Diacritic that will be corrected in the context or the previous character.\n" +
            " - Position X to correct (Float) : The correction that will be applied on X axis on the Diacritic character.\n" +
            " - Position Y to correct (Float) : The correction that will be applied on Y axis on the Diacritic character.\n");



    }

    /// <summary>
    /// Draw reloead asset
    /// </summary>
    protected void DrawReloadAsset() {
        GUILayout.Label("In case of issues related to the asset file");
        if (GUILayout.Button("Reload asset", GUILayout.ExpandWidth(false))) {
            LoadAssetEditor();
        }
    }

    protected void DrawInstructions(float width) {
        GUILayout.TextArea("Here is the main content of the Arabic Diacritic Dictionary. You can visualyze a specifc configuration using its index.");
        GUILayout.Space(10);
        GUILayout.TextArea("You can first perform a search using the Hexa Char Unicode of both the character used as a context and the diacritic that will be upadted. If this duo cannot be found, a button to add a new configuration will appear.");
        GUILayout.Space(10);
        GUILayout.TextArea("Be mindful, configuration is done in realtime. Real time push of changes in the current Scene is only done when pushing the `Apply on Current Scene` button");
    }

    /// <summary>
    /// Draw dictionary
    /// </summary>
    /// <param name="width"></param>
    protected void DrawConfig(float width) {
        if (arabicDiacriticDictionary != null && arabicDiacriticDictionary.diacriticConfigs.Count > 0) {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search for Hexa");


            searchCharContexteHexa = EditorGUILayout.TextField(searchCharContexteHexa, GUILayout.ExpandWidth(false));
            searchCharUpdateHexa = EditorGUILayout.TextField(searchCharUpdateHexa, GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Search", GUILayout.ExpandWidth(false))) {
                var index = arabicDiacriticDictionary.diacriticConfigs.FindIndex(a => a.hexaCharContext == searchCharContexteHexa && a.hexaCharUpdate == searchCharUpdateHexa);
                if (index >= 0) {
                    viewIndex = index;
                    textSearchHexa = "";
                } else {
                    textSearchHexa = "Not found";
                }
            }
            GUILayout.EndHorizontal();
            if (textSearchHexa != "") {
                GUILayout.Label(textSearchHexa);
                if (GUILayout.Button("Add new one", GUILayout.ExpandWidth(false))) {
                    var index = arabicDiacriticDictionary.diacriticConfigs.FindIndex(a => a.hexaCharContext == searchCharContexteHexa && a.hexaCharUpdate == searchCharUpdateHexa);
                    if (index == -1) {
                        arabicDiacriticDictionary.diacriticConfigs.Add(new DiacriticConfig("", searchCharContexteHexa, searchCharUpdateHexa, 0, 0));
                        EditorUtility.SetDirty(arabicDiacriticDictionary);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        var New_index = arabicDiacriticDictionary.diacriticConfigs.FindIndex(a => a.hexaCharContext == searchCharContexteHexa && a.hexaCharUpdate == searchCharUpdateHexa);
                        if (New_index >= 0) {
                            viewIndex = New_index;
                        } else {
                            Debug.Log("Error : Creation of entry did not work for : " + searchCharContexteHexa + " - " + searchCharUpdateHexa);
                        }
                    } else {
                        Debug.Log("Error : Trying to add entries that are already found at " + index + " :" + searchCharContexteHexa + " - " + searchCharUpdateHexa);
                    }
                    textSearchHexa = "";
                }
            }

            GUILayout.Space(20);


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prev", GUILayout.ExpandWidth(false))) {
                if (viewIndex > 0)
                    viewIndex--;
            }
            viewIndex = EditorGUILayout.IntSlider(viewIndex, 0, arabicDiacriticDictionary.diacriticConfigs.Count - 1);
            if (GUILayout.Button("Next", GUILayout.ExpandWidth(false))) {
                if (viewIndex < arabicDiacriticDictionary.diacriticConfigs.Count) {
                    viewIndex++;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Char Contexte " + ArabicDiacriticHelper.GetLetterFromUnicode(arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).hexaCharContext), arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).hexaCharContext);
            EditorGUILayout.TextField(ArabicDiacriticHelper.GetLetterFromUnicode(arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).hexaCharContext));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Char to Update", arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).hexaCharUpdate);
            EditorGUILayout.TextField(ArabicDiacriticHelper.GetLetterFromUnicode(arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).hexaCharUpdate));
            GUILayout.EndHorizontal();
            arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).positionX = EditorGUILayout.FloatField("Position X to Correct", arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).positionX);
            arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).positionY = EditorGUILayout.FloatField("Position Y to Correct", arabicDiacriticDictionary.diacriticConfigs.ElementAt(viewIndex).positionY);


            if (GUILayout.Button("Apply on Current Scene", GUILayout.ExpandWidth(false))) {
                // save the asset
                EditorUtility.SetDirty(arabicDiacriticDictionary);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                var objects = GameObject.FindObjectsOfType<TextMeshProUGUI>();
                foreach (var obj in objects) {
                    // force the correction for this specific element
                    arabicDiacriticSupport.FixDiacriticsPositionsInTMP(obj);
                    // dont judge me please... it is only to force TMP to reload during runtime for viewing purposes
                    obj.isRightToLeftText = !obj.isRightToLeftText;
                    obj.isRightToLeftText = !obj.isRightToLeftText;
                }
            }

        } else {
            GUILayout.Label("Error : Count not find or generate a dictionary.");
        }

    }
}
#endif