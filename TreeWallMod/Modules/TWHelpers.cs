using TownOfUs.Patches;

namespace TreeWallMod.Modules
{
    public sealed class TWHelpers
    {
        public static bool CommsActive()
        {
            var isActive = false;
            if (VanillaSystemCheckPatches.HudCommsSystem != null)
            {
                isActive = VanillaSystemCheckPatches.HudCommsSystem.IsActive;
            }
            else if (VanillaSystemCheckPatches.HqCommsSystem != null)
            {
                isActive = VanillaSystemCheckPatches.HqCommsSystem.IsActive;
            }

            return isActive;
        }
    }
}
