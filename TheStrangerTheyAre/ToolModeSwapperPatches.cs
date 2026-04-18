using HarmonyLib;

namespace TheStrangerTheyAre
{
    [HarmonyPatch(typeof(ToolModeSwapper))]
    public static class ToolModeSwapperPatches
    {
        // Prevent translator from being equipped in the following conditions
        // - When player is looking at a ghost wall text and either of the following is true:
        //   - Is in the dream world (since the translator shouldn't work there *cough* The Outsider *cough*)
        //   - Knows the language (since the translator isn't needed)
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ToolModeSwapper.IsNomaiTextInFocus))]
        public static void ToolModeSwapper_IsNomaiTextInFocus_Postfix(ToolModeSwapper __instance, ref bool __result)
        {
            var text = __instance._firstPersonManipulator.GetFocusedNomaiText();
            var blockGhostWall = PlayerState.InDreamWorld() || StrangerTextHandlerTSTA.KnowsLanguage();

            __result = __result && (!blockGhostWall || text is not GhostWallText);
        }
    }
}