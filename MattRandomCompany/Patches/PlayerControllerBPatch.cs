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
        private static float movementSpeedMulUpper;
        private static float movementSpeedMulLower;
        private static bool movementSpeedMulEnabled;

        private static float sprintTime;
        private static float sprintTimeMulUpper;
        private static float sprintTimeMulLower;
        private static bool sprintTimeMulEnabled;

        private static float jumpForce;
        private static float jumpForceMulUpper;
        private static float jumpForceMulLower;
        private static bool jumpForceMulEnabled;

        private static float climbSpeed;
        private static float climbSpeedMultiplierUpper;
        private static float climbSpeedMultiplierLower;
        private static bool climbSpeedMultiplierEnabled;

        public PlayerControllerBPatch()
        {
            random = new Random();

            try
            {
                // Load movement speed multiplier values
                movementSpeed = 0;
                movementSpeedMulUpper = Config.Instance.moveSpeedMulUpper.Value;
                movementSpeedMulLower = Config.Instance.moveSpeedMulLower.Value;
                movementSpeedMulEnabled = Config.Instance.moveSpeedMulEnabled.Value;
                if (movementSpeedMulLower > movementSpeedMulUpper || !movementSpeedMulEnabled)
                {
                    movementSpeedMulLower = 1;
                    movementSpeedMulUpper = 1;
                }

                // Load sprint time multiplier values
                sprintTime = 0;
                sprintTimeMulLower = Config.Instance.sprintTimeMulLower.Value;
                sprintTimeMulUpper = Config.Instance.sprintTimeMulUpper.Value;
                sprintTimeMulEnabled = Config.Instance.sprintTimeMulEnabled.Value;
                if (sprintTimeMulLower > sprintTimeMulUpper || !sprintTimeMulEnabled)
                {
                    sprintTimeMulLower = 1;
                    sprintTimeMulUpper = 1;
                }

                // Load jump force values
                jumpForce = 0;
                jumpForceMulUpper = Config.Instance.jumpForceMulLower.Value;
                jumpForceMulUpper = Config.Instance.jumpForceMulUpper.Value;
                jumpForceMulEnabled = Config.Instance.jumpForceMulEnabled.Value;
                if(jumpForceMulLower > jumpForceMulUpper || !jumpForceMulEnabled)
                {
                    jumpForceMulLower = 1;
                    jumpForceMulUpper = 1;
                }

                // Load climb speed multiplier values
                climbSpeed = 0;
                climbSpeedMultiplierUpper = Config.Instance.climbSpeedMulUpper.Value;
                climbSpeedMultiplierLower = Config.Instance.climbSpeedMulLower.Value;
                climbSpeedMultiplierEnabled = Config.Instance.climbSpeedMulEnabled.Value;
                if (climbSpeedMultiplierLower > climbSpeedMultiplierUpper || !climbSpeedMultiplierEnabled)
                {
                    climbSpeedMultiplierLower = 1;
                    climbSpeedMultiplierUpper = 1;
                }
            }
            catch(Exception ex)
            {
                RandomCompany.logMessage("Error " + ex);
            }
        }
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void RandomizePlayerControllerPatch(PlayerControllerB __instance)
        {
            // error handling
            if(__instance == null)
            {
                RandomCompany.logMessage("failed to retrieve PlayerControllerB reference");
                return;
            }

            // Set Player movement speed
            if (movementSpeed == 0)
            {
                movementSpeed = __instance.movementSpeed * (movementSpeedMulLower + (float)(random.NextDouble() * (movementSpeedMulUpper - movementSpeedMulLower)));
            }
            __instance.movementSpeed = movementSpeed;

            // Set Player sprint time
            if(sprintTime == 0)
            {
                sprintTime = __instance.sprintTime * (sprintTimeMulLower + (float)(random.NextDouble() * (sprintTimeMulUpper - sprintTimeMulLower)));
            }
            __instance.sprintTime = sprintTime;

            // Set Player jump force
            if(jumpForce == 0)
            {
                
                jumpForce = __instance.jumpForce * (jumpForceMulLower + (float)(random.NextDouble() * (jumpForceMulUpper - jumpForceMulLower)));
            }
            __instance.jumpForce = jumpForce;

            // Set Player climb speed
            if (climbSpeed == 0)
            {
                climbSpeed = __instance.climbSpeed * (movementSpeedMulLower + (float)(random.NextDouble() * (climbSpeedMultiplierUpper - climbSpeedMultiplierLower)));
            }
            __instance.climbSpeed = climbSpeed;

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
