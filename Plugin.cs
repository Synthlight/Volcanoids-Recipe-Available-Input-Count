using Base_Mod;
using JetBrains.Annotations;

namespace Recipe_Available_Input_Count {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override string ModName    { get; } = "Recipe-Available-Input-Count";
        protected override bool   UseHarmony { get; } = true;

        protected override void Init() {
        }
    }
}