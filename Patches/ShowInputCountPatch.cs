using System.Reflection;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;

namespace Recipe_Available_Input_Count.Patches {
    [HarmonyPatch]
    [UsedImplicitly]
    public static class ShowInputCountPatch {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(FactoryUi).GetMethod(nameof(FactoryUi.WriteRecipeInputs), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [UsedImplicitly]
        [HarmonyPrefix]
        public static bool Prefix(ref FactoryUi __instance, ref RichTextWriter result) {
            if (FactoryUi.m_availabilityInfo.Inputs.Count > 0) result.Text.AppendLine();

            foreach (var inputInfo in FactoryUi.m_availabilityInfo.Inputs) {
                var inputAmount = inputInfo.AmountRequired;
                var haveAmount  = __instance.m_producer.GetPullInventory().GetAmount(inputInfo.Item, int.MaxValue);

                // Show input amount needed.
                result.CurrentStyle = inputAmount > haveAmount ? "TextError" : "Text";
                result.Text.ConcatFormat(__instance.Texts.InputFormat.Text, inputInfo.AmountRequired, inputInfo.Item.Name);
                // Show available amount.
                result.CurrentStyle = "Text";
                result.Text.ConcatFormat(__instance.Texts.InputAvailableFormat.Text, haveAmount);

                result.Text.AppendLine();
            }
            return false;
        }
    }
}