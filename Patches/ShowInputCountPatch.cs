using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
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

            // base.WriteInfoInputs(recipe, result);
            for (var i = 34; i <= 37; i++) {
                il[i].opcode  = OpCodes.Nop;
                il[i].operand = null;
            }

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

        private static readonly MethodInfo GetInputAmount_MethodInfo = typeof(FactoryStation).GetMethod("GetInputAmount", BindingFlags.NonPublic | BindingFlags.Instance);

        [UsedImplicitly]
        [HarmonyPostfix]
        public static void Postfix(ref FactoryStation __instance, ref Recipe recipe, ref RichTextWriter result, ref FactoryTexts ___m_texts, ref OnlineCargo ___m_cargo) {
            foreach (var inventoryItemData in recipe.Inputs) {
                var inputAmount     = (int) GetInputAmount_MethodInfo.Invoke(__instance, new object[] {(InventoryItem) inventoryItemData});
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