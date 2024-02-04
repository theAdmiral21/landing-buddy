using System.Reflection;
using BepInEx;
using JetBrains.Annotations;
using SpaceWarp;
using SpaceWarp.API.Assets;
using SpaceWarp.API.Mods;
using SpaceWarp.API.UI.Appbar;
using SpaceWarp.API.Game.Messages;
using LandingBuddyProj.UI;
using UitkForKsp2.API;
using UnityEngine;
using UnityEngine.UIElements;
using BepInEx.Logging;

namespace LandingBuddyProj;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
public class LandingBuddyProjPlugin : BaseSpaceWarpPlugin
{
    // Useful in case some other mod wants to use this mod a dependency
    [PublicAPI] public const string ModGuid = MyPluginInfo.PLUGIN_GUID;
    [PublicAPI] public const string ModName = MyPluginInfo.PLUGIN_NAME;
    [PublicAPI] public const string ModVer = MyPluginInfo.PLUGIN_VERSION;

    /// Singleton instance of the plugin class
    [PublicAPI] public static LandingBuddyProjPlugin Instance { get; set; }

    // AppBar button IDs
    internal const string ToolbarFlightButtonID = "BTN-LandingBuddyProjFlight";
    internal const string ToolbarOabButtonID = "BTN-LandingBuddyProjOAB";
    internal const string ToolbarKscButtonID = "BTN-LandingBuddyProjKSC";

    // Testing debug logger
    public new static ManualLogSource Logger { get; set; }

    // Game logger
    MyFirstWindowController controller;

    /// <summary>
    /// Runs when the mod is first initialized.
    /// </summary>
    public override void OnInitialized()
    {
        base.OnInitialized();

        Instance = this;

        // create the logger
        Logger = base.Logger;

        // Load all the other assemblies used by this mod
        LoadAssemblies();

        // Load the UI (umxl) from the asset bundle
        var myFirstWindowUxml = AssetManager.GetAsset<VisualTreeAsset>(
            // The case-insensitive path to the asset in the bundle is composed of:
            // - The mod GUID:
            $"{ModGuid}/" +
            // - The name of the asset bundle:
            "LandingBuddyProj_ui/" +
            // - The path to the asset in your Unity project (without the "Assets/" part)
            "ui/myfirstwindow/lb_ui.uxml"
        );

        // Create the window options object
        var windowOptions = new WindowOptions
        {
            // The ID of the window. It should be unique to your mod.
            WindowId = "LandingBuddyProj_MyFirstWindow",
            // The transform of parent game object of the window.
            // If null, it will be created under the main canvas.
            Parent = null,
            // Whether or not the window can be hidden with F2.
            IsHidingEnabled = true,
            // Whether to disable game input when typing into text fields.
            DisableGameInputForTextFields = true,
            MoveOptions = new MoveOptions
            {
                // Whether or not the window can be moved by dragging.
                IsMovingEnabled = true,
                // Whether or not the window can only be moved within the screen bounds.
                CheckScreenBounds = true
            }
        };

        // Create the window
        var myFirstWindow = Window.Create(windowOptions, myFirstWindowUxml);
        // Add a controller for the UI to the window's game object
        var myFirstWindowController = myFirstWindow.gameObject.AddComponent<MyFirstWindowController>();

        // Register Flight AppBar button
        Appbar.RegisterAppButton(
            ModName,
            ToolbarFlightButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            isOpen => myFirstWindowController.IsWindowOpen = isOpen
        );

        // Register OAB AppBar Button
        Appbar.RegisterOABAppButton(
            ModName,
            ToolbarOabButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            isOpen => myFirstWindowController.IsWindowOpen = isOpen
        );

        // Register KSC AppBar Button
        Appbar.RegisterKSCAppButton(
            ModName,
            ToolbarKscButtonID,
            AssetManager.GetAsset<Texture2D>($"{ModGuid}/images/icon.png"),
            () => myFirstWindowController.IsWindowOpen = !myFirstWindowController.IsWindowOpen
        );

        // This is where I am adding my custom functions

        StateChanges.Map3DViewEntered += message =>
        {
            Logger.LogDebug($"Map3DViewEntered message received, TESTING DEBUG YA DIG?");
        };

        StateChanges.Map3DViewLeft += message =>
        {
            Logger.LogDebug($"Map3DViewEntered message received, What have I done...");
        };
    }

    /// <summary>
    /// Loads all the assemblies for the mod.
    /// </summary>
    private static void LoadAssemblies()
    {
        // Load the Unity project assembly
        var currentFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName;
        var unityAssembly = Assembly.LoadFrom(Path.Combine(currentFolder, "LandingBuddyProj.Unity.dll"));
        // Register any custom UI controls from the loaded assembly
        CustomControls.RegisterFromAssembly(unityAssembly);
    }
}
