#define Apollo_Adapter
using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;

using System.IO;

public static class XCodePostProcess
{
	private static string ENABLE_BITCODE = "NO"; // YES or NO

	[PostProcessBuild(999)]
	public static void OnPostProcessBuild( BuildTarget target, string pathToBuiltProject )
	{

#if UNITY_4 || UNITY_4_7 || UNITY_4_6 || UNITY_4_5
		if (target != BuildTarget.iPhone)
#else
		if (target != BuildTarget.iOS)
#endif
		{
			// Target is not iPhone/iOS. XCodePostProcess will not run
			return;
		}

		Debug.Log("======================= Editor Xcode Project Begin =======================");
		UnityVersion uVerseion = GetUnityVersion();
		string path = Path.GetFullPath(pathToBuiltProject);
		
		Debug.Log("[XUPorter] unityVersion:" + uVerseion + ", pathToBuiltProject:" + pathToBuiltProject + ", full path:" + path);
		// Create a new project object from build target
		XCProject project = new XCProject(path);
		
		if (uVerseion > UnityVersion.Unity_4)
		{
			project.AddFrameworkSearchPaths("$(SRCROOT)/Frameworks/Plugins/iOS");
		}
		
		// Find and run through all projmods files to patch the project.
		// Please pay attention that ALL projmods files in your project folder will be excuted!
		// Application.dataPath: <project folder> / Assets
		
		string[] files = Directory.GetFiles(Application.dataPath, "*.projmods", SearchOption.AllDirectories); 
		
		foreach (string file in files)
		{
			Debug.Log("[XUPorter] Add projmods File: " + file);
			project.ApplyMod(file);
		}

		project.BitCodeEnableExceptions(ENABLE_BITCODE);

		EditorPlist(path);

		EditorCode(uVerseion, path);
		#if Apollo_Adapter
        AdapterXCodePostProcess.EditorCodeAdapter(uVerseion, path);
		AdapterXCodePostProcess.EditorPlist(path);
		#endif
		// Finally save the xcode project
		project.Save();

		Debug.Log("======================= Editor Xcode Project End =======================");
	}

    public enum UnityVersion
	{
		Unknow,
		Unity_4,
		Unity_5,
		Unity_2017,
		Unity_2018
	}
	
	static UnityVersion GetUnityVersion()
	{
		if (Application.unityVersion.StartsWith("4"))
		{
			return UnityVersion.Unity_4;
		}
		else if (Application.unityVersion.StartsWith("5"))
		{
			return UnityVersion.Unity_5;
		}
		else if (Application.unityVersion.StartsWith("2017"))
		{
			return UnityVersion.Unity_2017;
		}
        else if (Application.unityVersion.StartsWith("2018"))
        {
            return UnityVersion.Unity_2018;
        }
		else
		{
			return UnityVersion.Unknow;
		}
	}
	
	private static void AddPush(XCProject project, string projectPath)
	{
		//Enable Push Notification
		project.EnablePushNotification ("7ARWKX4JSR");
		XCPlist	entitilements = new XCPlist();

		entitilements.AddLine ("<plist version=\"1.0\">");
		entitilements.AddLine ("<dict>");
		entitilements.AddLine ("<key>aps-environment</key>");
		entitilements.AddLine ("<string>development</string>");
		entitilements.AddLine ("</dict>");
		entitilements.AddLine ("</plist>");

		string entitleFilepath = projectPath + "/production.entitlements";
		entitilements.Save (entitleFilepath);
		project.overwriteBuildSetting ("CODE_SIGN_ENTITLEMENTS", "./production.entitlements", "Release");
		project.overwriteBuildSetting ("CODE_SIGN_ENTITLEMENTS", "./production.entitlements", "Debug");
		PBXSortedDictionary<PBXGroup> groups_ = project.groups;

		PBXSortedDictionary<PBXGroup>.ValueCollection array_ = groups_.Values;

		foreach (PBXGroup group_ in array_) 
		{
			if(group_.name == "CustomTemplate")
			{
				Debug.Log("[XUPorter]find group CustomTemplate" );
				project.AddFile(entitleFilepath, group_);
			}
		}
		Debug.Log ("cant`t find CustomTemplate");
	}
	
	private static void EditorPlist(string filePath)
	{
		
		XCPlist list =new XCPlist(filePath);
		
		string PlistAdd = @"
			<key>GCloud</key>
		    <dict>
		      <key>GameKey</key>
		      <string>1234567890</string>
		      <key>GameId</key>
		      <string>123456789</string>
		    </dict>
		    <key>GCloudCore</key>
		    <dict>
		      <key>RemoteConfigUrl</key>
		      <string>https://cloudctrl.gcloud.qq.com</string>
		      <key>GTraceUrl</key>
		      <string>https://cloudctrl.gcloud.qq.com/gtrace/get_traceid_list</string>
		    </dict>
			<key>CHANNEL_DENGTA</key>
			<string>1001</string>
			<key>WX_UNIVERSAL_LINK</key>
            <string>https://wiki.ssl.msdk.qq.com/app/</string>
			<key>MSDK_OfferId</key>
			<string>1450000528</string>
			<key>MSDK_ENV</key>
			<string>test</string>
			<key>EnableMSDKLog</key>
			<true/>
			<key>QQAppID</key>
			<string>1000001061</string>
			<key>QQAppKey</key>
			<string>GEkBatmPLXYY56oJ</string>
			<key>WXAppID</key>
			<string>wxf58707b1adb71279</string>
			<key>MSDKKey</key>
			<string>d4755682e2315e0fcea418d679223fcd</string>
            <key>MSDK_Webview_Force_Adapt_Bang_Screen</key>
            <false/>
			<key>NeedNotice</key>
			<true/>
			<key>NoticeTime</key>
			<integer>600</integer>
			<key>MSDK_PUSH_SWITCH</key>
			<true/>
			<key>MSDK_REAL_NAME_AUTH_SWITCH</key>
	        <integer>0</integer>
            <key>XG_HOST</key>
            <string></string>
            <key>GCloud</key>
            <dict>
            <key>GameId</key>
            <string>123456789</string>
            </dict>
            <key>TDM</key>
            <dict>
            <key>AppId</key>
            <string>123456789</string>
            <key>TGEMIT_ROUTER_ADDRESS_FORMAL</key>
            <string>https://hc.tdm.qq.com:8013/tdm/v1/route</string>
            <key>TGEMIT_ROUTER_ADDRESS_TEST</key>
            <string>https://htest.tdm.qq.com:8013/tdm/v1/route</string>
            </dict>
			<key>ApolloURLScheme</key>
			<string>apollourl</string>";
		list.AddKey(PlistAdd);

		list.Save();
	}
	
	private static void EditorCode (UnityVersion uVersion, string filePath)
	{
		XClass UnityAppController = new XClass (filePath + "/Classes/UnityAppController.mm");

		if (uVersion == UnityVersion.Unity_4) 
		{
			UnityAppController.WriteBelow ("#include \"PluginBase/AppDelegateListener.h\"", "#include <apollo/ApolloApplication.h>");
			UnityAppController.WriteBelow ("- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation\n{", "    [[ApolloApplication sharedInstance] handleOpenURL:url];");	
			UnityAppController.WriteBelow ("printf_console(\"-> applicationDidEnterBackground()\\n\");", "    [[ApolloApplication sharedInstance] applicationDidEnterBackground:application];");
			UnityAppController.WriteBelow ("printf_console(\"-> applicationWillEnterForeground()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillEnterForeground:application];");
			UnityAppController.WriteBelow ("printf_console(\"-> applicationWillResignActive()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillResignActive:application];");		
			UnityAppController.WriteBelow ("UnityCleanup();\n", "    [[ApolloApplication sharedInstance] applicationWillTerminate:application];");
			UnityAppController.WriteBelow ("[_rootView layoutSubviews];\n", "    [[ApolloApplication sharedInstance] setViewController:_window.rootViewController];");
			UnityAppController.WriteBelow ("[[ApolloApplication sharedInstance] applicationWillTerminate:application];\n\n}", "- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\r{\r    return [[ApolloApplication sharedInstance] handleOpenURL:url];\r}");
			UnityAppController.WriteBelow ("SensorsCleanup();\n}", "- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler\r{\r    [[ApolloApplication sharedInstance] applicationContinueUserActivity:application Activity:userActivity];\r    return YES;\r}");
					

		}
		else 
		{
			UnityAppController.WriteBelow ("#include \"PluginBase/AppDelegateListener.h\"", "#include <apollo/ApolloApplication.h>");
            UnityAppController.WriteBelow ("- (BOOL)application:(UIApplication*)app openURL:(NSURL*)url options:(NSDictionary<NSString*, id>*)options\n{", "    [[ApolloApplication sharedInstance] handleOpenURL:url];");
            UnityAppController.WriteBelow ("- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation\n{", "    [[ApolloApplication sharedInstance] handleOpenURL:url];");
			UnityAppController.WriteBelow ("::printf(\"-> applicationDidEnterBackground()\\n\");", "     [[ApolloApplication sharedInstance] applicationDidEnterBackground:application];");			
			UnityAppController.WriteBelow ("::printf(\"-> applicationWillEnterForeground()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillEnterForeground:application];");
			UnityAppController.WriteBelow ("::printf(\"-> applicationWillResignActive()\\n\");", "    [[ApolloApplication sharedInstance] applicationWillResignActive:application];");		
			UnityAppController.WriteBelow ("UnityCleanup();\n", "    [[ApolloApplication sharedInstance] applicationWillTerminate:application];");
			UnityAppController.WriteBelow ("return YES;\n}", "- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url\r{\r    return [[ApolloApplication sharedInstance] handleOpenURL:url];\r} \n\n");
			UnityAppController.WriteBelow ("[KeyboardDelegate Initialize];\n", "    [[ApolloApplication sharedInstance] setViewController:UnityGetGLViewController()];");
			UnityAppController.WriteBelow ("SensorsCleanup();\n}", "- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray * _Nullable))restorationHandler\r{\r    [[ApolloApplication sharedInstance] applicationContinueUserActivity:application Activity:userActivity];\r    return YES;\r}");
		}
	}
}
