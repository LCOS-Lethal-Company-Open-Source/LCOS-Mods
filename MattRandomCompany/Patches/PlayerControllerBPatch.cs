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
        /*
         * 
         *  THIS IS A PROOF OF CONCEPT
         *  IT WORKED, MEANING I CAN RANDOMIZE PLAYER CHARACTERISTICS
         *  INDIVIDUALLY. EVEYR PLAYER WILL HAVE DIFFERENT VALUES
         * 
         */

        private static System.Random random;
        private static int multiplier;
        private static float initialms = 0;

        public PlayerControllerBPatch()
        {
            random = new Random();
            multiplier = random.Next(2, 10);
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void RandomizePlayerControllerPatch(ref float ___movementSpeed)
        {
            RandomCompany.logMessage("testing123");
            if(initialms == 0)
            {
                initialms = ___movementSpeed * multiplier;
            }
            ___movementSpeed = initialms;
        }
    }
}
