using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ArabicAlphabetHelper : Used to manually fix arabic diacritic font issues created by unity
/// </summary>
/// 
public class ArabicDiacriticSupport {

    protected static readonly string PathResources = "Assets/Resources/";
    protected static readonly string PathArabicDiacriticDictionarySO = "ArabicDiacritic/";
    protected static readonly string NameArabicDiacriticDictionarySO = "ArabicDiacriticDictionary";
    protected static readonly string FormatArabicDiacriticDictionarySO = ".asset";

    public ArabicDiacriticDictionarySO fontsDictionaries;

    private List<DiacriticConfig> diacriticConfigs = null;

    public bool debug = false;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="debug"></param>
    public ArabicDiacriticSupport(bool debug = false) {
        this.debug = debug;
        LoadAsset();
    }

    /// <summary>
    /// Loading Asset
    /// </summary>
    protected void LoadAsset() {
        if (fontsDictionaries == null) {
            fontsDictionaries = Resources.Load<ArabicDiacriticDictionarySO>(PathArabicDiacriticDictionarySO + NameArabicDiacriticDictionarySO);
        }
    }



    /// <summary>
    /// Build the distionary of corrections
    /// </summary>
    private void BuildDiacriticConfigs() {
        if (fontsDictionaries == null || fontsDictionaries.diacriticConfigs == null) {
            // Go back to default
            diacriticConfigs = ArabicDiacriticDictionaryInit.DiacriticConfigs;
        } else {
            // Use SO Dynamic Data
            diacriticConfigs = fontsDictionaries.diacriticConfigs;
        }
    }

    /// <summary>
    /// FindDiacriticConfig
    /// Find diacritic configuration when looking for a pair of context character and character to update
    /// Return a vector2 containing the configuration found
    /// </summary>
    /// <param name="hexaCharContext"></param>
    /// <param name="hexaCharUpdate"></param>
    /// <returns></returns>
    private Vector2 FindDiacriticConfig(string hexaCharContext, string hexaCharUpdate) {
        if (diacriticConfigs == null) { BuildDiacriticConfigs(); }

        // Search for those unicodes in diacriticConfigs
        var diacriticCombos =
            from combo
            in diacriticConfigs
            where combo.hexaCharContext == hexaCharContext
            && combo.hexaCharUpdate == hexaCharUpdate
            select combo;
        var newDelta = new Vector2(0, 0);
        if (diacriticCombos.Count() > 0) {
            var positionX = diacriticCombos.First().positionX;
            var positionY = diacriticCombos.First().positionY;

            newDelta = new Vector2(positionX, positionY);
        }
        // Return the Vector2 of corrections if the config is found.
        return newDelta;
    }



    /// <summary>
    /// Fix the diacritics positions inside a TMP component
    /// </summary>
    /// <param name="obj"></param>
    public void FixDiacriticsPositionsInTMP(UnityEngine.Object obj) {


        var tmp = obj as TMPro.TMP_Text;
        if (tmp == null) {
            return;
        }
        var textInfo = tmp.textInfo;
        if (textInfo == null) {
            return;
        }


        var characterInfo = textInfo.characterInfo;
        var characterCount = textInfo.characterCount;
        var changed = false;

        // Only apply changes if the count of characters is superior to 1
        if (characterCount > 1) {
            Vector2 modificationDelta = new Vector2(0, 0);
            // From right to left. Looking for pairs
            for (var charPosition = characterCount - 1; charPosition > 0; charPosition--) {

                if (debug) {
                    Debug.Log("Combo Results : " + characterInfo[charPosition].character + " " +
                    ArabicDiacriticHelper.GetHexUnicodeFromChar(characterInfo[charPosition].character) + " " +
                    characterInfo[charPosition - 1].character + " " +
                    ArabicDiacriticHelper.GetHexUnicodeFromChar(characterInfo[charPosition - 1].character) + " "
                    + FindDiacriticConfig(
                        ArabicDiacriticHelper.GetHexUnicodeFromChar(characterInfo[charPosition].character),
                        ArabicDiacriticHelper.GetHexUnicodeFromChar(characterInfo[charPosition - 1].character)
                    )
                );
                }
                // Look for the configuration to apply for the current pair of characters
                modificationDelta = FindDiacriticConfig(
                    ArabicDiacriticHelper.GetHexUnicodeFromChar(characterInfo[charPosition].character),
                    ArabicDiacriticHelper.GetHexUnicodeFromChar(characterInfo[charPosition - 1].character)
                );

                // If a modification has to be applied
                if (modificationDelta.sqrMagnitude > 0f) {
                    changed = true;

                    // Retrieve information from the character
                    var materialReferenceIndex = characterInfo[charPosition - 1].materialReferenceIndex;
                    var vertexIndex = characterInfo[charPosition - 1].vertexIndex;
                    var originalVertices = textInfo.meshInfo[materialReferenceIndex].vertices;

                    // Calculate the required changes to apply
                    var charsize = (originalVertices[vertexIndex + 2].y - originalVertices[vertexIndex + 0].y);
                    var offsetX = charsize * modificationDelta.x / 100f;
                    var offsetY = charsize * modificationDelta.y / 100f;
                    var offset = new Vector3(offsetX, offsetY, 0f);

                    // Apply new vertices
                    var finalVertices = textInfo.meshInfo[materialReferenceIndex].vertices;
                    finalVertices[vertexIndex + 0] = originalVertices[vertexIndex + 0] + offset;
                    finalVertices[vertexIndex + 1] = originalVertices[vertexIndex + 1] + offset;
                    finalVertices[vertexIndex + 2] = originalVertices[vertexIndex + 2] + offset;
                    finalVertices[vertexIndex + 3] = originalVertices[vertexIndex + 3] + offset;
                }
            }
        }
        if (changed) {
            // Force the element to update the vertex data
            tmp.UpdateVertexData();
        }
    }

    /// <summary>
    /// Enable Post Processing for all TMP elements
    /// </summary>
    public void EnablePostProcessingTMP() {
        TMPro.TMPro_EventManager.TEXT_CHANGED_EVENT.Add(FixDiacriticsPositionsInTMP);
    }
    
}