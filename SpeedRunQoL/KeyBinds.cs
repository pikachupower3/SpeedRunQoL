using System;
using System.Collections;
using System.Linq;
using DebugMod;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Console = DebugMod.Console;

namespace SpeedRunQoL
{
    //needs to be a public static class
    public static class KeyBinds
    {
        
        private static bool fsmToggle = false;
        
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
            UumuuExtra();
        }
        
        public static void UumuuExtra()
        {
            if (!fsmToggle)
            {
                SetUumuuExtra(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SetUumuuExtra;
                Console.AddLine("Uumuu forced extra attack ON");
                fsmToggle = true;
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SetUumuuExtra;
                fsmToggle = false;
                Console.AddLine("Uumuu forced extra attack OFF");
            }
        }

        private static void SetUumuuExtra(Scene sceneFrom, Scene sceneTo) => SetUumuuExtra(sceneTo.name);
        private static void SetUumuuExtra(string NextSceneName)
        {
            if (NextSceneName == "Fungus3_archive_02")
            {
                GameManager.instance.StartCoroutine(UumuuExtraCoro(NextSceneName));
            }
        }

        private static IEnumerator UumuuExtraCoro(string activeScene)
        {
            Console.AddLine("Uumuu set extra attack coro launched");
            while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != activeScene)
            {
                yield return null;
            }

            // Find Uumuu, their FSM, the specific State, and the Action that dictates the possibility of extra attacks
            GameObject uumuu = GameObject.Find("Mega Jellyfish");
            PlayMakerFSM fsm = uumuu.LocateMyFSM("Mega Jellyfish");
            FsmState fsmState = fsm.FsmStates.First(t => t.Name == "Idle");
            WaitRandom waitRandom = (WaitRandom) fsmState.Actions.OfType<WaitRandom>().First();
            waitRandom.timeMax.Value = 1.6f;
            yield break;
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
    }
}