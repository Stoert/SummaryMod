using DistantWorlds2;
using HarmonyLib;
using JetBrains.Annotations;

namespace SummaryMod;

[PublicAPI]
public class Mod
{

    public Mod(DWGame game)
    {
        //Harmony.DEBUG = true;
        new Harmony(nameof(SummaryMod)).PatchAll();
    } 
}
