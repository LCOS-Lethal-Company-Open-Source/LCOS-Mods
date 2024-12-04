using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomCompany.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {

        private static System.Random random;

        private static float movementSpeed;
        private static float movementSpeedMultiplierUpper;
        private static float movementSpeedMultiplierLower;
        private static bool movementSpeedMultiplierEnabled;

        private static float climbSpeed;
        private static float climbSpeedMultiplierUpper;
        private static float climbSpeedMultiplierLower;
        private static bool climbSpeedMultiplierEnabled;

        public PlayerControllerBPatch()
        {
            random = new Random();

            // Load movement speed multiplier values
            movementSpeed = 0;
            movementSpeedMultiplierUpper = Config.Instance.moveSpeedUpper.Value;
            movementSpeedMultiplierLower = Config.Instance.moveSpeedLower.Value;
            movementSpeedMultiplierEnabled = Config.Instance.moveSpeedEnabled.Value;
            if(movementSpeedMultiplierLower > movementSpeedMultiplierUpper || !movementSpeedMultiplierEnabled)
            {
                movementSpeedMultiplierLower = 1;
                movementSpeedMultiplierUpper = 1;
            }

            // Load climb speed multiplier values
            climbSpeed = 0;
            climbSpeedMultiplierUpper = Config.Instance.climbSpeedUpper.Value;
            climbSpeedMultiplierLower = Config.Instance.climbSpeedLower.Value;
            climbSpeedMultiplierEnabled = Config.Instance.climbSpeedEnabled.Value;
            if(climbSpeedMultiplierLower > climbSpeedMultiplierUpper || !climbSpeedMultiplierEnabled)
            {
                climbSpeedMultiplierLower = 1;
                climbSpeedMultiplierUpper = 1;
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void RandomizePlayerControllerPatch(ref float ___movementSpeed, ref float ___climbSpeed)
        {
            // Set Player movement speed
            if(movementSpeed == 0)
            {
                movementSpeed = ___movementSpeed * movementSpeedMultiplierLower + (float)(random.NextDouble() * (movementSpeedMultiplierUpper - movementSpeedMultiplierLower));
                RandomCompany.logMessage(movementSpeed.ToString());
            }
            ___movementSpeed = movementSpeed;
            
            // Set Player climb speed
            if(climbSpeed == 0)
            {
                climbSpeed = ___climbSpeed * movementSpeedMultiplierLower + (float)(random.NextDouble() * (climbSpeedMultiplierUpper - climbSpeedMultiplierLower));
            }
            ___climbSpeed = climbSpeed;
        }

        [HarmonyPostfix]
        [HarmonyPatch("ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer()
        {
            if (Config.IsHost)
            {
                try
                {
                    Config.MessageManager.RegisterNamedMessageHandler("ModName_OnRequestConfigSync", Config.OnRequestSync);
                    Config.Synced = true;
                }
                catch (Exception e)
                {
                    RandomCompany.logMessage(e.ToString());
                }

                return;
            }

            Config.Synced = false;
            Config.MessageManager.RegisterNamedMessageHandler("ModName_OnReceiveConfigSync", Config.OnReceiveSync);
            Config.RequestSync();
        }

    }
}
