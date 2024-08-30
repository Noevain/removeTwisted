using BepInEx;
using HarmonyLib;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.SceneManagement;

namespace ExamplePlugin
{
    // This is an example plugin that can be put in
    // BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    // It's a small plugin that adds a relatively simple item to the game,
    // and gives you that item whenever you press F2.

    // This attribute specifies that we have a dependency on a given BepInEx Plugin,
    // We need the R2API ItemAPI dependency because we are using for adding our item to the game.
    // You don't need this if you're not using R2API in your plugin,
    // it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(EliteAPI.PluginGUID)]

    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // This is the main declaration of our plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    // BaseUnityPlugin itself inherits from MonoBehaviour,
    // so you can use this as a reference for what you can declare and use in your plugin class
    // More information in the Unity Docs: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class RemoveTwisted : BaseUnityPlugin
    {
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Noevain";
        public const string PluginName = "RemoveTwisted";
        public const string PluginVersion = "1.0.0";
		private Timer timer;
        // We need our item definition to persist through our functions, and therefore make it a class field.

        // The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
			// Init our logging class so that we can properly log for debugging
			Log.Init(Logger);
            // First let's define our item
            Logger.LogInfo("Hooking into onServerStageBegin");
            //DirectorAPI.MonsterActions += DirectorAPI_MonsterActions;
            Stage.onServerStageBegin += removedTwisted;
            // But now we have defined an item, but it doesn't do anything yet. So we'll need to define that ourselves.
            //GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            

        }

		private void removedTwisted(Stage s)
        {
			Logger.LogDebug("Get Current Elite tiers");
			CombatDirector.EliteTierDef[] currElites = EliteAPI.GetCombatDirectorEliteTiers();
			EliteDef twisted = null;
			foreach (var eliteR in currElites)
			{
				foreach (var eliteDef in eliteR.availableDefs)
				{
					Logger.LogDebug(eliteDef.name);
					if (eliteDef.name == "edBead")
					{
						twisted = eliteDef;
						Logger.LogDebug("found the fuckers");
					}
				}
				if (twisted != null)
				{
					eliteR.availableDefs.Remove(twisted);
					Logger.LogDebug("Removed from availableDefs");
				}
			}
            if (twisted != null)
            {
                Logger.LogDebug("Attempting CombatDirector EliteTiers Override");
                EliteAPI.OverrideCombatDirectorEliteTiers(currElites);
                Logger.LogDebug("Removed the worst elite type in the game");
            }

            this.timer = new Timer();
            this.timer.Interval = 30000;//30sec
            this.timer.Elapsed += removeTwistedTimer;
            this.timer.Enabled = true;


		}

        private void removeTwistedTimer(object sender, ElapsedEventArgs e)
        {
			Log.Init(Logger);
			Logger.LogDebug("30 sec elapsded,checking eliteDefs...");
			CombatDirector.EliteTierDef[] currElites = EliteAPI.GetCombatDirectorEliteTiers();
			EliteDef twisted = null;
			foreach (var eliteR in currElites)
			{
				foreach (var eliteDef in eliteR.availableDefs)
				{
					Logger.LogDebug(eliteDef.name);
					if (eliteDef.name == "edBead")
					{
						twisted = eliteDef;
						Logger.LogDebug("found the fuckers");
					}
				}
				if (twisted != null)
				{
					eliteR.availableDefs.Remove(twisted);
					Logger.LogDebug("Removed from availableDefs");
				}
			}
			if (twisted != null)
			{
				Logger.LogDebug("Attempting CombatDirector EliteTiers Override");
				EliteAPI.OverrideCombatDirectorEliteTiers(currElites);
				Logger.LogDebug("Removed the worst elite type in the game");
			}
		}

		// The Update() method is run on every frame of the game.
		private void Update()
        {
            // This if statement checks if the player has currently pressed F2.
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Logger.LogDebug("Keybind removing twisted");
                Logger.LogDebug("Get Current Elite tiers");
              CombatDirector.EliteTierDef[] currElites = EliteAPI.GetCombatDirectorEliteTiers();
                EliteDef twisted = null;
                foreach(var eliteR in currElites)
                {
                    foreach(var eliteDef in eliteR.availableDefs)
                    {
                        Logger.LogDebug(eliteDef.name);
                        if(eliteDef.name == "edBead")
                        {
                            twisted = eliteDef;
                            Logger.LogDebug("found the fuckers");
                        }
                    }
                    if(twisted != null)
                    {
                        eliteR.availableDefs.Remove(twisted);
						Logger.LogDebug("Removed from availableDefs");
					}
                }
                if (twisted != null)
                {
                    Logger.LogDebug("Attempting CombatDirector EliteTiers Override");
                    EliteAPI.OverrideCombatDirectorEliteTiers(currElites);
                    Logger.LogDebug("Removed the worst elite type in the game");
                }

			}
        }
    }
}
