
# Arabic-Diacritics-Runtime-Correction
[![License](https://img.shields.io/badge/License-BSD%202--Clause-blue)](https://opensource.org/licenses/BSD-2-Clause)
[![Unity Engine](https://img.shields.io/badge/Unity%20Engine-2018.4.0f1%20%F0%9F%97%B8-green)](https://unity3d.com/get-unity/download/archive)
[![Unity Engine](https://img.shields.io/badge/Unity%20Engine-2019.1.1f1%20%F0%9F%97%B8-green)](https://unity3d.com/get-unity/download/archive)


Runtime correction of Arabic Diacritics in Unity Game Development.

The aim of this project is to overcome the incorrect rendering of Arabic Diacritics in Unity Game Engine.

An article has been created to describe our approach of a solution: {URL}

## Installation

This code has been tested for the following versions of Unity Engine:

- 2018.4.0f1
- 2019.1.1f1


### Setup

You can clone the repo or download the code from the latest release (available in unitypackage, zip or tar.gz).

### Configuration

The configuration [ArabicDiacriticDictionary_GeezaProRegular](Configuration/ArabicDiacriticDictionary_GeezaProRegular.asset) provided in the `Configuration` folder is a preset of configuration based on the `Geeza Pro Regular` font.

Because of licensing rights, we could not share this font in this repo.

If you wish to use this file, you can place at the following path: `Assets\Resources\ArabicDiacritic`. And rename it into `ArabicDiacriticDictionary.asset`

If you prefer creating your own configuration file for your own font, you have to generate the following path `Assets\Resources\ArabicDiacritic`.
And the script will generate (at runtime after code initialization or when opening the Configuration Editor window) a new empty configuration file `ArabicDiacriticDictionary.asset` in there.

### Initializing

To initialize and enable the runtime correction, you can integrate the following code when first starting your scene.

```
var arabicDiacriticSupport = new ArabicDiacriticSupport();
arabicDiacriticSupport.EnablePostProcessingTMP();
```

### Overview

- [ArabicDiacriticDictionary_GeezaProRegular](Configuration/ArabicDiacriticDictionary_GeezaProRegular.asset) contains a preset of configuration for the `Geeza Pro Regular` font (not included in this repo).
- [ArabicDiacriticSupport](ArabicDiacriticSupport.cs) is the main entry point of the system.
- [ArabicDiacriticConfigWindows](ArabicDiacriticConfigWindows.cs) allows to update the configuration via a Unity Editor Window.
- [ArabicDiacriticDictionarySO](ArabicDiacriticDictionarySO.cs) is in charge of handling and generating the Configuration contained in a Scriptable Object.
- [ArabicDiacriticHelper](ArabicDiacriticHelper.cs) contains few helpers methods

## License

To be consistent with third party licenses, this project is licensed under the BSD-2-Clause License - See the [LICENSE.txt](LICENSE.txt) file for more details.

For the third party licenses, see the [THIRD-PARTY.txt](Licenses/THIRD-PARTY.txt) file.


## Credits
- Antura

	> Great inspiration for the approach of post-processing characters modifications in Unity Game Engine.
	>
	> Copyright (c) 2017, TH Köln / Cologne Game Lab, Video Games Without Borders & Wixel Studios
	>
	> https://github.com/vgwb/Antura
	>
	> BSD 2-Clause License