using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;

using System.IO;

public static class AdapterXCodePostProcess
{
    private static void EditorPlist(string filePath)
	{
		
		XCPlist list =new XCPlist(filePath);
		
		string PlistAdd = @"
			<key>LSApplicationQueriesSchemes</key>
			<array>
				<string>mqq</string>
				<string>mqqapi</string>
				<string>wtloginmqq2</string>
				<string>mqqopensdkapiV4</string>
				<string>mqqopensdkapiV3</string>
				<string>mqqopensdkapiV2</string>
				<string>mqqwpa</string>
				<string>mqqOpensdkSSoLogin</string>
				<string>mqqgamebindinggroup</string>
				<string>mqqopensdkfriend</string>
				<string>mqzone</string>
				<string>weixin</string>
				<string>wechat</string>
				<string>weixinULAPI</string>
                <string>mqqopensdkminiapp</string>
                <string>mqqopensdklaunchminiapp</string>
			</array>
            <key>XG_FREE_ACCESS_ID</key>
            <integer>2291101061</integer>
            <key>XG_V2_ACCESS_ID</key>
            <integer>1600008740</integer>
            <key>XG_V2_ACCESS_KEY</key>
            <string>I4FS2NPJ7G6I</string>
			<key>APPKEY_DENGTA</key>
			<string>0I200A6H7A04ZKTA</string>
			<key>CFBundleURLTypes</key>
			<array>
				<dict>
					<key>CFBundleTypeRole</key>
					<string>Editor</string>
					<key>CFBundleURLName</key>
					<string>weixin</string>
					<key>CFBundleURLSchemes</key>
					<array>
						<string>wxf58707b1adb71279</string>
					</array>
				</dict>
				<dict>
					<key>CFBundleTypeRole</key>
					<string>Editor</string>
					<key>CFBundleURLName</key>
					<string>tencentopenapi</string>
					<key>CFBundleURLSchemes</key>
					<array>
						<string>tencent1000001061</string>
					</array>
				</dict>
				<dict>
					<key>CFBundleTypeRole</key>
					<string>Editor</string>
					<key>CFBundleURLName</key>
					<string>QQLaunch</string>
					<key>CFBundleURLSchemes</key>
					<array>
						<string>tencentlaunch1000001061</string>
					</array>
				</dict>
				<dict>
					<key>CFBundleTypeRole</key>
					<string>Editor</string>
					<key>CFBundleURLName</key>
					<string>QQ</string>
					<key>CFBundleURLSchemes</key>
					<array>
						<string>QQ3B9ACE25</string>
					</array>
				</dict>
				<dict>
					<key>CFBundleTypeRole</key>
					<string>Editor</string>
					<key>CFBundleURLName</key>
					<string>apollourl</string>
					<key>CFBundleURLSchemes</key>
					<array>
						<string>apollourl</string>
					</array>
				</dict>
			</array>
			<key>UISupportedInterfaceOrientations</key>
			<array>
				<string>UIInterfaceOrientationPortrait</string>
				<string>UIInterfaceOrientationLandscapeLeft</string>
				<string>UIInterfaceOrientationLandscapeRight</string>
				<string>UIInterfaceOrientationPortraitUpsideDown</string>
			</array>";
        list.AddKey(PlistAdd);

		list.Save();           
    }

    public static void EditorCodeAdapter (XCodePostProcess.UnityVersion uVersion, string filePath)
    {
        XClass UnityAppController = new XClass (filePath + "/Classes/UnityAppController.mm");

        if (uVersion == XCodePostProcess.UnityVersion.Unity_4) 
        {                 
            //For Push
            UnityAppController.WriteBelow ("#import <OpenGLES/ES2/glext.h>", "#import <MsdkAdapter/MsdkFactory.h>");
            UnityAppController.WriteBelow ("#import <MsdkAdapter/MsdkFactory.h>", "#import <MSDK/MSDK.h>");
            UnityAppController.WriteBelow ("[self preStartUnity];\n", "    NMsdkAdapter::Install();\n    [[ApolloApplication sharedInstance] application:application didFinishLaunchingWithOptions:launchOptions]; \n    [MSDKXG WGRegisterAPNSPushNotification:launchOptions];");
            UnityAppController.WriteBelow ("UnitySendDeviceToken(deviceToken);\n", "    [[ApolloApplication sharedInstance] application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken]; \n    [MSDKXG WGSuccessedRegisterdAPNSWithToken:deviceToken];");
            UnityAppController.WriteBelow ("UnitySendRemoteNotificationError(error);\n", "    [[ApolloApplication sharedInstance] application:application didFailToRegisterForRemoteNotificationsWithError:error]; \n    [MSDKXG WGFailedRegisteredAPNS];");
            UnityAppController.WriteBelow ("UnitySendRemoteNotification(userInfo);\n", "    [[ApolloApplication sharedInstance] application:application didReceiveRemoteNotification:userInfo]; \n    [MSDKXG WGReceivedMSGFromAPNSWithDict:userInfo];");
            UnityAppController.WriteBelow ("_didResignActive = false;\n", "    [[ApolloApplication sharedInstance] applicationDidBecomeActive:application]; \n    [MSDKXG WGCleanBadgeNumber];");

        }
        else 
        {
            //For Push
            UnityAppController.WriteBelow ("#import <OpenGLES/ES2/glext.h>", "#import <MsdkAdapter/MsdkFactory.h>");
            UnityAppController.WriteBelow ("#import <MsdkAdapter/MsdkFactory.h>", "#import <MSDK/MSDK.h>");
            UnityAppController.WriteBelow ("[self preStartUnity];\n", "    NMsdkAdapter::Install();\n    [[ApolloApplication sharedInstance] application:application didFinishLaunchingWithOptions:launchOptions]; \n    [MSDKXG WGRegisterAPNSPushNotification:launchOptions];");
            UnityAppController.WriteBelow ("UnitySendDeviceToken(deviceToken);\n", "    [[ApolloApplication sharedInstance] application:application didRegisterForRemoteNotificationsWithDeviceToken:deviceToken]; \n    [MSDKXG WGSuccessedRegisterdAPNSWithToken:deviceToken];");
            UnityAppController.WriteBelow ("UnitySendRemoteNotificationError(error);\n", "    [[ApolloApplication sharedInstance] application:application didFailToRegisterForRemoteNotificationsWithError:error]; \n    [MSDKXG WGFailedRegisteredAPNS];");
            UnityAppController.WriteBelow ("UnitySendRemoteNotification(userInfo);\n", "    [[ApolloApplication sharedInstance] application:application didReceiveRemoteNotification:userInfo]; \n    [MSDKXG WGReceivedMSGFromAPNSWithDict:userInfo];");
            UnityAppController.WriteBelow ("_didResignActive = false;\n", "    [[ApolloApplication sharedInstance] applicationDidBecomeActive:application]; \n    [MSDKXG WGCleanBadgeNumber];");
        }
    }
}