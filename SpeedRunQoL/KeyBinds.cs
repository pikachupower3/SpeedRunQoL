using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using DebugMod;
using DebugMod.Hitbox;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding;
using SpeedRunQoL.Functionality;
using UnityEngine;
using UnityEngine.SceneManagement;
using Console = DebugMod.Console;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace SpeedRunQoL
{
    //needs to be a public static class
    public static class KeyBinds
    {
        //each keybind that has to be loaded needs to be annotated with BindableMethod and be public static void
        [BindableMethod(name = "Reload Radiance Fight", category = "Speedrun Extentions")]
        public static void LoadRadiance()
        {
            GameManager.instance.StartCoroutine(LoadRadianceRoom());
            
            //making sure debugmod console is logged when something happens is good
            DebugMod.Console.AddLine("Radiance Loaded");
        }
        [BindableMethod(name = "Press Accept Challenge", category = "Speedrun Extentions")]
        public static void ChallengeRadiance()
        {
            GameObject.Find("Challenge Prompt Radiant").LocateMyFSM("Challenge Start").SetState("Take Control");
            DebugMod.Console.AddLine("Radiance Challenged (asuming it exists))");
        }
        
        //since this function is private static and isnt annotated with BindableMethod, it wont show up as keybind
        private static IEnumerator LoadRadianceRoom()
        {
            //makes sure the initial platform and challage prompt appears
            HeroController.instance.gameObject.LocateMyFSM("ProxyFSM").FsmVariables.FindFsmBool("Faced Radiance").Value = false;
            
            HeroController.instance.RelinquishControl();
            HeroController.instance.StopAnimationControl();
            EventRegister.SendEvent("START DREAM ENTRY");
            EventRegister.SendEvent("DREAM ENTER");
            HeroController.instance.enterWithoutInput = true; // stop early control on scene load
            
            GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
            {
                SceneName = "Dream_Final_Boss",
                EntryGateName = "door1",
                EntryDelay = 0f,
                PreventCameraFadeOut = true,
            });

            yield return new WaitUntil(() => HeroController.instance.acceptingInput);
            yield return null;
            //cuz people mostly have full soul from thk fight
            HeroController.instance.AddMPCharge(198);  
        }
        
        [BindableMethod(name = "Force Uumuu extra attack", category = "Speedrun Extentions")]
        public static void ForceUumuuExtra() 
        {
            ForceUumuuExtraAttack.ToggleExtraAttack();
        }

        [BindableMethod(name = "Position Save", category = "Speedrun Extentions")]
        public static void SavePosition()
        {
            PositionSaveState.SavePosition();
        }
        [BindableMethod(name = "Position Load", category = "Speedrun Extentions")]
        public static void LoadPosition()
        {
            PositionSaveState.LoadPosition();
        }
        
        [BindableMethod(name = "Quickslot (load duped)", category = "Savestates")]
        public static void LoadStateDuped()
        {
            LoadState(new object[] { 0, true }); //SaveStateType.MEMORY
        }

        [BindableMethod(name = "Load duped state from file", category = "Savestates")]
        public static void LoadFromFileDuped()
        {
            LoadState(new object[] { 2, true }); //SaveStateType.SKIPONE
        }

        private static void LoadState(object[] args)
        {
            var debugModType = typeof(DebugMod.DebugMod);
            var instance = debugModType.GetField("instance", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null);
            if (instance == null)
            {
                Console.AddLine("Error while loading savestate: no DebugMod instance");
                return;
            }
            
            var saveStateManager =
                debugModType.GetField("saveStateManager", BindingFlags.Static | BindingFlags.NonPublic)
                    ?.GetValue(instance);
            if (saveStateManager == null)
            {
                Console.AddLine("Error while loading savestate: no SaveStateManager");
                return;
            }
            
            saveStateManager.GetType().GetMethod("LoadState", BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(saveStateManager, args);
        }

        [BindableMethod(name = "Toggle Bench Storage", category = "Glitches")]
        public static void ToggleBenchStorage()
        {
            PlayerData.instance.atBench = !PlayerData.instance.atBench;
            Console.AddLine($"{(PlayerData.instance.atBench ? "Given" : "Taken away")} bench storage");
        }

        [BindableMethod(name = "Toggle Collision", category = "Glitches")]
        public static void ToggleCollision()
        {
            var rb2d = HeroController.instance.GetComponent<Rigidbody2D>();
            rb2d.isKinematic = !rb2d.isKinematic;
            Console.AddLine($"{(rb2d.isKinematic ? "Disabled" : "Enabled")} collision");
        }

        [BindableMethod(name = "Dreamgate Invulnerability", category = "Glitches")]
        public static void GiveDgateInvuln()
        {
            PlayerData.instance.isInvincible = true;
            UnityEngine.Object.FindObjectOfType<HeroBox>().gameObject.SetActive(false);
            HeroController.instance.gameObject.LocateMyFSM("Roar Lock").FsmVariables.FindFsmBool("No Roar").Value =
                true;
            Console.AddLine("Given dreamgate invulnerability");
        }
        
        [BindableMethod(name = "Reset Quick Map Storage", category = "Glitches")]
        public static void ResetQMStorage()
        {
            var mapFSM = HeroController.instance.gameObject.LocateMyFSM("Map Control");
            mapFSM.FsmVariables.FindFsmGameObject("Inventory").Value = null;
            Console.AddLine("Reset quick map storage");
        }
        
        [BindableMethod(name = "Dupe Active Room", category = "Glitches")]
        public static void DupeActiveRoom()
        {
            GameManager.instance.StartCoroutine(LoadRoom(USceneManager.GetActiveScene().name));
        }

        private static IEnumerator LoadRoom(string sceneName)
        {
            var loadop = USceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            loadop.allowSceneActivation = true;
            yield return loadop;
            GameManager.instance.RefreshTilemapInfo(sceneName);

            var settings = DebugMod.DebugMod.settings;
            if (settings.ShowHitBoxes > 0)
            {
                int cs = settings.ShowHitBoxes;
                settings.ShowHitBoxes = 0;
                yield return new WaitUntil(() => HitboxViewer.State == 0);
                settings.ShowHitBoxes = cs;
            }
        }
    }
}