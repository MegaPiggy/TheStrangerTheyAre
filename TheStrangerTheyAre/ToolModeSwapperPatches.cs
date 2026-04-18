using HarmonyLib;

namespace TheStrangerTheyAre
{
    [HarmonyPatch(typeof(ToolModeSwapper))]
    public static class ToolModeSwapperPatches
    {
        // Prevent translator from being equipped in the dream world.
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ToolModeSwapper.IsNomaiTextInFocus))]
        public static void ToolModeSwapper_IsNomaiTextInFocus_Postfix(ref bool __result)
        {
            __result = __result && !PlayerState.InDreamWorld();
        }
    }
}