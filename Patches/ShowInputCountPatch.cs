using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Base_Mod;
using HarmonyLib;
using JetBrains.Annotations;

namespace Recipe_Available_Input_Count.Patches {
    [HarmonyPatch]
    [UsedImplicitly]
    public static class ShowInputCountPatch1 {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(FactoryStation).GetMethod("WriteInfoInputs", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [UsedImplicitly]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var il = instructions.ToList();

            // Remove the `base.WriteInfoInputs(recipe, result);` as we'll call it in our Postfix if needed.
            var lastPop = il.LastIndexOf(new CodeInstruction(OpCodes.Pop));
            il.Nop(lastPop + 1, il.Count - 2); // -1 to skip the `ret`. (-2 because 1 based 'count'.)

            return il;
        }
    }

    [HarmonyPatch]
    [UsedImplicitly]
    public static class ShowInputCountPatch2 {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(FactoryStation).GetMethod("WriteInfoInputs", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static readonly MethodInfo GET_INPUT_AMOUNT_METHOD_INFO = typeof(FactoryStation).GetMethod("GetInputAmount", BindingFlags.NonPublic | BindingFlags.Instance);

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void Postfix(ref FactoryStation __instance, ref Recipe recipe, ref RichTextWriter result, ref FactoryTexts ___m_texts, ref OnlineCargo ___m_cargo) {
            foreach (var inventoryItemData in recipe.Inputs) {
                var inputAmount     = (int) GET_INPUT_AMOUNT_METHOD_INFO.Invoke(__instance, new object[] {inventoryItemData});
                var availableAmount = ___m_cargo.GetAmount(inventoryItemData.Item, inventoryItemData.Stats);

                // Show input amount needed.
                result.CurrentStyle = inputAmount >= inventoryItemData.Amount ? "Text" : "TextError";
                result.Text.ConcatFormat(___m_texts.InputFormat.Text, inventoryItemData.Amount, inventoryItemData.Item.Name);
                // Show available amount.
                result.CurrentStyle = "Text";
                result.Text.ConcatFormat(___m_texts.InputAvailableFormat.Text, availableAmount);

                result.Text.AppendLine();
            }
        }
    }
}