using Base_Mod;
using JetBrains.Annotations;

namespace Recipe_Available_Input_Count {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override string ModName    => "Recipe-Available-Input-Count";
        protected override bool   UseHarmony => true;

        public static bool showAmountOfCurrentItem = false;
    }
}