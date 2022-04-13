using System.Diagnostics.CodeAnalysis;
using DistantWorlds2;
using DistantWorlds.Types;
using DistantWorlds.UI;

using Xenko.Games;
using Xenko.Input;

using HarmonyLib;
using JetBrains.Annotations;

namespace SummaryMod;

[PublicAPI]
[HarmonyPatch(typeof(InputSystem))]
[SuppressMessage("ReSharper", "InconsistentNaming")]

public class PatchInputSystem
{
    public static bool bShowGalaxySummary = false;
    public static ScaledRenderer? scaledRenderer = null;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(InputSystem.Update))]
    public static void Update(InputSystem __instance, InputManager inputManager, GameSettings gameSettings, GameTime gameTime, float timePassed, float timeInSeconds)
    {
        var _Renderer = Traverse.Create(__instance).Field("_Renderer").GetValue() as ScaledRenderer;
        var _Game = Traverse.Create(__instance).Field("_Game").GetValue() as DWGame;

        if (_Renderer != null && _Game != null)
        {
            scaledRenderer = _Renderer;

            if (inputManager != null && inputManager.HasKeyboard)
            {
                bool isShiftPressed = false;
                bool isCtrlPressed = false;
                isShiftPressed = (inputManager.DownKeys.Contains(Keys.LeftShift) | inputManager.DownKeys.Contains(Keys.RightShift));
                isCtrlPressed = (inputManager.DownKeys.Contains(Keys.LeftCtrl) | inputManager.DownKeys.Contains(Keys.RightCtrl));

                Keys[] array = inputManager.DownKeys.ToArray<Keys>();
                if (!UserInterfaceController.AcceptKeyboardInput(gameSettings, DateTime.Now, array, isShiftPressed, false, isCtrlPressed))
                {
                    if (_Renderer.SceneRenderType != SceneRenderType.Menu)
                    {
                        if (inputManager.IsKeyPressed(Keys.H) && isShiftPressed)
                            bShowGalaxySummary = !bShowGalaxySummary;
                    }
                }
            }
        }
    }
}
