using System;
using System.Collections;
using System.Linq;
using DebugMod;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SpeedRunQoL.Functionality;
using UnityEngine;
using UnityEngine.SceneManagement;
using Console = DebugMod.Console;

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
    }
}