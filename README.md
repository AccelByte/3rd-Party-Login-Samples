Prerequisites
- PS4
1. AccelByteUnitySDK ver 1.12.0 or newer
2. Sony NPToolkit2 and Unity with PS4 SDK support. You can find it [here](https://ps4.siedev.net/forums/thread/227324/)

- Xbox One
1. AccelByteUnitySDK ver 1.12.1 or newer
2. Unity with Xbox One Editor extension and Visual Studio Tools for Unity. You can follow [this documentation](https://docs.microsoft.com/en-us/gaming/xbox-live/get-started/setup-ide/managed-partners/unity-xbox/live-partner-unity-xdk-il2cpp)  

Settings
- PS4
1. Change your build platform to PS4
2. Set your App Content ID on Player Settings>Publishing Settings>Content ID. [Here](https://ps4.siedev.net/resources/documents/Misc/current/Publishing_Tools-Overview/0003.html) is some explanation about Content ID and Packages
3. Set your NP Title ID on Player Settings>Publishing Settings>Playstation(R)Network>NP Title ID
4. Fill your PS4 Client ID credential on file thirdPartyLogin.cs. You can find your Client ID on your PS4 DevNet Dashboard.
5. Don't forget to fill your AB Credentials too on AccelByteSDKConfig.json

- Xbox One
1. Change your build platform to Xbox One
2. Set your Title ID and Service Config ID(SCID) on Player Settings>Publishing Settings. You can find your Title ID and SCID on your Microsoft Partner Center Dashboard.
3. Fill your Xbox Live request url on file thirdPartyLogin.cs
4. Don't forget to fill your AB Credentials too on AccelByteSDKConfig.json