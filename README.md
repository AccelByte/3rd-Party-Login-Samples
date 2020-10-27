Prerequisites
- PS4
1. AccelByteUnitySDK ver 1.12.0 or newer.
2. Sony NPToolkit2 and Unity with PS4 SDK support. You can find it [here](https://ps4.siedev.net/forums/thread/227324/).

- Xbox One
1. AccelByteUnitySDK ver 1.12.1 or newer.
2. Unity with Xbox One Editor extension and Visual Studio Tools for Unity. You can follow [this documentation](https://docs.microsoft.com/en-us/gaming/xbox-live/get-started/setup-ide/managed-partners/unity-xbox/live-partner-unity-xdk-il2cpp).  

- Stadia
1. AccelByteUnitySDK ver 1.25.0 or newer.
2. Stadia SDK. You can get it from [here](https://developers.google.com/stadia/1.53/downloads/files).
3. Stadia plugin for Unity Editor and Stadia Platform Support package (com.unity.stadia) builds (put the Support package on Package folder on this project). You can find it [here](https://forum.unity.com/threads/unity-for-stadia-downloads.743003/).
5. Reserve an instance, you should be able to do that from the [Partner Portal](https://console.ggp.google.com/) or from your Stadia SDK console.

Settings
- PS4
1. Change your build platform to PS4.
2. Set your App Content ID on Player Settings>Publishing Settings>Content ID. [Here](https://ps4.siedev.net/resources/documents/Misc/current/Publishing_Tools-Overview/0003.html) is some explanation about Content ID and Packages.
3. Set your NP Title ID on Player Settings>Publishing Settings>Playstation(R)Network>NP Title ID.
4. Fill your PS4 Client ID credential on file thirdPartyLogin.cs. You can find your Client ID on your PS4 DevNet Dashboard.
5. Don't forget to fill your AB Credentials too on AccelByteSDKConfig.json or from the Unity Editor.

- Xbox One
1. Change your build platform to Xbox One.
2. Set your Title ID and Service Config ID(SCID) on Player Settings>Publishing Settings. You can find your Title ID and SCID on your Microsoft Partner Center Dashboard.
3. Fill your Xbox Live request url on file thirdPartyLogin.cs
4. Don't forget to fill your AB Credentials too on AccelByteSDKConfig.json or from the Unity Editor.

- Stadia
1. Change your build platform to Stadia.
2. Unity Editor will automatically choose the applications and the instances you can use. You can change it on the Build Settings.
3. If you're not reserve an instance yet, please reserve it first and click the refresh button on the Build Settings. 
4. Don't forget to fill your AB Credentials too on AccelByteSDKConfig.json or from the Unity Editor.