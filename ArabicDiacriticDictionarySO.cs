using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class DiacriticConfig {
    public string hexaCharPreContext = ""; // Not used so far, could be used in case of more complex context oriented system
    public string hexaCharContext = "";
    public string hexaCharUpdate = "";
    public float positionX = 0f;
    public float positionY = 0f;

    public DiacriticConfig(string hexaCharPreContext, string hexaCharContext, string hexaCharUpdate, float positionX, float positionY) {
        this.hexaCharPreContext = hexaCharPreContext;
        this.hexaCharContext = hexaCharContext;
        this.hexaCharUpdate = hexaCharUpdate;
        this.positionX = positionX;
        this.positionY = positionY;
    }
}

/// <summary>
/// ArabicDiacriticDictionarySO
/// </summary>
public class ArabicDiacriticDictionarySO : ScriptableObject {
    public List<DiacriticConfig> diacriticConfigs;
}

/// <summary>
/// Containing the List used to fix diacritic positions
/// </summary>
public static class ArabicDiacriticDictionaryInit {

    public static List<DiacriticConfig> DiacriticConfigs = new List<DiacriticConfig> {
        // Some initial presets can be definied directly in the code as the following :
        {new DiacriticConfig("","0000", "0000", 0, 0)},
    };
}
