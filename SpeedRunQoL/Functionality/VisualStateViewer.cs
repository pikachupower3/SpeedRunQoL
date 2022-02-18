using System;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Utils;
using UnityEngine;

namespace SpeedRunQoL.Functionality
{
    public enum CurrentViewingState
    {
        None,
        FacingDirection,
        CanFireball,
        CanDash,
        CanWallJump,
        CanJump,
        CanAttack,
        CanOpenInventory
    }
    public static class VisualStateViewer
    {

        public static CurrentViewingState CurrentViewingState = CurrentViewingState.None;

        public static void CheckForFlashConditions()
        {
            switch (CurrentViewingState)
            {
                case CurrentViewingState.FacingDirection:
                    
                    Flash(HeroController.instance.cState.facingRight);
                    break;
                
                case CurrentViewingState.CanFireball:

                    Flash(HeroController.instance.CanCast());
                    break;
                
                case CurrentViewingState.CanDash:
                    
                    Flash((bool) privateFuncs["CanDash"](HeroController.instance));
                        break;
                
                case CurrentViewingState.CanWallJump:
                    
                    Flash((bool) privateFuncs["CanWallJump"](HeroController.instance));
                    break;
                
                case CurrentViewingState.CanJump:
                    
                    Flash((bool) privateFuncs["CanJump"](HeroController.instance));
                    break;
                
                case CurrentViewingState.CanAttack:
                    
                    Flash((bool) privateFuncs["CanAttack"](HeroController.instance));
                    break;
                
                case CurrentViewingState.CanOpenInventory:
                    
                    Flash(HeroController.instance.CanOpenInventory());
                    break;
            }
        }
        
        public static void IncrementCurrentViewingState()
        {
            int newvalue = (int) CurrentViewingState + 1;
            if (newvalue > Enum.GetNames(typeof(CurrentViewingState)).Length - 1)
            {
                newvalue = 0;
            }
            CurrentViewingState = (CurrentViewingState)newvalue;
        }

        private static BindingFlags priv = BindingFlags.NonPublic | BindingFlags.Instance;

        private static readonly Dictionary<string, FastReflectionDelegate> privateFuncs =
            new Dictionary<string, FastReflectionDelegate>()
            {
                {"CanDash", typeof(HeroController).GetMethod("CanDash",priv).GetFastDelegate()},
                {"CanJump", typeof(HeroController).GetMethod("CanJump",priv).GetFastDelegate()},
                {"CanAttack", typeof(HeroController).GetMethod("CanAttack",priv).GetFastDelegate()},
                {"CanWallJump", typeof(HeroController).GetMethod("CanWallJump",priv).GetFastDelegate()},
            };

        //flash values
        private static float amount = 1f;
        private static float timeUp = 0f;
        private static float stayTime = 0.1f;
        private static float timeDown = 0f;
        private static void Flash(bool on)
        {
            var spriteFlash = HeroController.instance.gameObject.GetComponent<SpriteFlash>();
            switch (on)
            {
                case true:
                    spriteFlash.flash(Color.white, amount, timeUp, stayTime, timeDown);
                    break;
                case false:
                    spriteFlash.flash(Color.black, amount, timeUp, stayTime, timeDown);
                    break;
            }
        }
    }
}