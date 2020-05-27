Prerequisites
1. AccelByteUnitySDK ver 1.12.0 or newer
2. Sony NPToolkit2 and Unity with PS4 SDK support. You can find it [here](https://ps4.siedev.net/forums/thread/227324/)

Settings
1. Change your build platform to PS4
2. Set your App Content ID on Player Settings>Publishing Settings>Content ID. [Here](https://ps4.siedev.net/resources/documents/Misc/current/Publishing_Tools-Overview/0003.html) is some explanation about Content ID and Packages
3. Set your NP Title ID on Player Settings>Publishing Settings>Playstation(R)Network>NP Title ID
4. Fill you PS4 Client ID credential on file thirdPartyLogin.cs line 60
5. Don't forget to fill your AB Credentials too on AccelByteSDKConfig.json