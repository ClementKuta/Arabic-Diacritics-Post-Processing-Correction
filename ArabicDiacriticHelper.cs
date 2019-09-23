using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Arabic Diacritic Helper
/// </summary>
static class ArabicDiacriticHelper {

    /// <summary>
    /// Get unicodes from string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string GetStringUnicodes(string str) {
        if (String.IsNullOrEmpty(str)) {
            return "";
        }
        char[] chars = str.ToCharArray();
        string output = "";
        for (int i = 0; i < chars.Length; i++) {
            char character = chars[i];
            output += GetHexUnicodeFromChar(character) + " ";
        }
        if (output.Length >= 2) {
            output = output.Remove(output.Length - 1);
        }
        return output;
    }

    /// <summary>
    /// Return letter from unicode
    /// </summary>
    /// <param name="hexCode">string Hexadecimal number</param>
    /// <returns>string char</returns>
    public static string GetLetterFromUnicode(string hexCode) {
        if (String.IsNullOrEmpty(hexCode)) {
            return "";
        }
        // Not Hex code
        if (!System.Text.RegularExpressions.Regex.IsMatch(hexCode, @"\A\b[0-9a-fA-F]+\b\Z")) {
            return "";
        }

        int unicode = int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);
        var character = (char)unicode;
        return character.ToString();
    }

    /// <summary>
    /// Return Text from Unicodes
    /// The default space separator can be overwritten in the parameters
    /// </summary>
    /// <param name="hexCodes">string Hexadecimals with separator</param>
    /// <param name="separator">Default separator : Space. Can be overwritten</param>
    /// <returns>string text</returns>
    public static string GetTextFromUnicode(string hexCodes, char separator = ' ') {
        var characters = hexCodes.Split(separator);
        var sentence = "";
        foreach (var character in characters) {
            sentence += GetLetterFromUnicode(character);
        }
        return sentence;
    }

    /// <summary>
    /// Return Char from Unicode
    /// </summary>
    /// <param name="_char"></param>
    /// <param name="unicodePrefix"></param>
    /// <returns></returns>
    public static string GetHexUnicodeFromChar(char character, bool unicodePrefix = false) {
        return string.Format("{1}{0:X4}", Convert.ToUInt16(character), unicodePrefix ? "/U" : string.Empty);
    }
}