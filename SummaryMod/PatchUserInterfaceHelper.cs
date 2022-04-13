using DistantWorlds2;
using DistantWorlds.Types;

using HarmonyLib;
using JetBrains.Annotations;

using Xenko.Graphics;
using Xenko.Core.Mathematics;

namespace SummaryMod;

[PublicAPI]
[HarmonyPatch(typeof(UserInterfaceHelper))]

public class PatchUserInterfaceHelper
{
	[HarmonyPostfix]
	[HarmonyPatch(nameof(UserInterfaceHelper), "DrawMainViewHeader")]

	public static void DrawMainViewHeader(SpriteBatch spriteBatch, Galaxy galaxy, Empire playerEmpire, Point mousePoint, DrawDetailType hoveredDrawType, RectangleF region, float minimumHoverTipWidth, SpriteFont normalFont, SpriteFont boldFont, SpriteFont smallFont, Color textColor, Color shadowColor, float borderSize, float tinyGap, float smallGap, float separator, float starDateRegionWidth, ref HoverTipList hovertips)
	{
		if (PatchInputSystem.bShowGalaxySummary && playerEmpire != null)
		{
			playerEmpire.ExplorationMap.CountExploredSystems(galaxy, playerEmpire, out int fullyExplored, out int partiallyExplored, out int unexplored);

			var text1 = "Explored Systems: " + fullyExplored.ToString() + "/" + unexplored.ToString();
			var text2 = "Partially explored: " + partiallyExplored.ToString();
			// ...
			var text4 = "Pirate Empires:     " + galaxy.Empires.GetPirateEmpires().Count.ToString();
			var text5 = "Mouse x,y: " + mousePoint.X.ToString() + " , " + mousePoint.Y.ToString();

			Vector2 vector1 = UserInterfaceHelper.FontSmall.MeasureString(text1);
			Color foreColor = Color.Gray;

			var x = region.Left - vector1.X - 40.0f;
			var y = 0.0f;

			int yOffset = 0;

			DrawingHelper.DrawStringDropShadow(spriteBatch, text1, FontSize.Small, true, playerEmpire.PrimaryColor, new Vector2(x, y + yOffset));
			yOffset += (int)vector1.Y;
			DrawingHelper.DrawStringDropShadow(spriteBatch, text2, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
			yOffset += (int)vector1.Y;
			//DrawingHelper.DrawStringDropShadow(spriteBatch, text3, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
			//yOffset += (int)vector1.Y;
			DrawingHelper.DrawStringDropShadow(spriteBatch, text4, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
			yOffset += (int)vector1.Y;
			DrawingHelper.DrawStringDropShadow(spriteBatch, text5, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));

			if (PatchInputSystem.scaledRenderer != null)
			{
				var currentGraphicsNodes = PatchInputSystem.scaledRenderer.CurrentGraphicsNodes;

				PatchInputSystem.scaledRenderer.DetermineViewGalaxyCoordinates(out int galaxyX, out int galaxyY);
				string text6 = "GalaxyView x,y: ";
				Vector2 vector1a = UserInterfaceHelper.FontSmall.MeasureString(text6);

				yOffset += (int)vector1.Y;
				DrawingHelper.DrawStringDropShadow(spriteBatch, text6, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
				DrawingHelper.DrawStringDropShadow(spriteBatch, galaxyX.ToString() + " , " + galaxyY.ToString() + "  (CoordinateDivisor: " + currentGraphicsNodes.CoordinateDivisor.ToString() + ")", FontSize.Small, true, Color.SteelBlue, new Vector2(x + vector1a.X + 10, y + yOffset));

				var isLockView = PatchInputSystem.scaledRenderer.GetLockViewOnSelected();

				object selectedObject = PatchInputSystem.scaledRenderer.SelectedObject;
				if (selectedObject != null && selectedObject is StellarObject)
				{
					string text7 = "Selected Object: " + ((StellarObject)selectedObject).GetName() + " (LockView: " + isLockView.ToString() + ")";
					yOffset += (int)vector1.Y;
					DrawingHelper.DrawStringDropShadow(spriteBatch, text7, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));

					string text7a = " -> Current Target x,y: ";
					Vector2 vector2 = UserInterfaceHelper.FontSmall.MeasureString(text7a);

					if (selectedObject is Ship && ((Ship)selectedObject).Mission != null)
					{
						Ship ship = (Ship)selectedObject;
						ShipCommand shipCommand = ship.Mission.ResolveCurrentCommand();
						if (!shipCommand.IsEmpty)
						{
							var currentTarget = ship.Mission.ResolveMissionTarget(galaxy, shipCommand.TargetType);
							if (currentTarget != null)
							{
								yOffset += (int)vector1.Y;

								DrawingHelper.DrawStringDropShadow(spriteBatch, text7a, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
								DrawingHelper.DrawStringDropShadow(spriteBatch, currentTarget.GalaxyX.ToString() + " , " + currentTarget.GalaxyY.ToString(), FontSize.Small, true, Color.Gold, new Vector2(x + vector2.X + 10, y + yOffset));
							}
						}
					}
					else if (selectedObject is Fleet && ((Fleet)selectedObject).Mission != null)
					{
						Fleet fleet = (Fleet)selectedObject;
						ShipCommand shipCommand = fleet.Mission.ResolveCurrentCommand();
						if (!shipCommand.IsEmpty)
						{
							var currentTarget = fleet.Mission.ResolveMissionTarget(galaxy, shipCommand.TargetType);
							if (currentTarget != null)
							{
								yOffset += (int)vector1.Y;

								DrawingHelper.DrawStringDropShadow(spriteBatch, text7a, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
								DrawingHelper.DrawStringDropShadow(spriteBatch, currentTarget.GalaxyX.ToString() + " , " + currentTarget.GalaxyY.ToString(), FontSize.Small, true, Color.Gold, new Vector2(x + vector2.X + 10, y + yOffset));
							}
						}
					}
					var modelPath = EffectHelper.ResolveModelPath((StellarObject)selectedObject, galaxy);
					var materialPath = EffectHelper.ResolveMaterialPath((StellarObject)selectedObject, galaxy);
					if (!string.IsNullOrEmpty(modelPath))
					{
						yOffset += (int)vector1.Y;
						DrawingHelper.DrawStringDropShadow(spriteBatch, " -> ModelPath: " + modelPath, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
					}
					if (!string.IsNullOrEmpty(materialPath))
					{
						yOffset += (int)vector1.Y;
						DrawingHelper.DrawStringDropShadow(spriteBatch, " -> MaterialPath: " + materialPath, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
					}

					yOffset += (int)vector1.Y;
					string text7c = " Galaxy x,y: ";
					Vector2 vector3 = UserInterfaceHelper.FontSmall.MeasureString(text7c);
					DrawingHelper.DrawStringDropShadow(spriteBatch, text7c, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
					DrawingHelper.DrawStringDropShadow(spriteBatch, ((StellarObject)selectedObject).GalaxyX.ToString() + " , " + ((StellarObject)selectedObject).GalaxyY.ToString(), FontSize.Small, true, Color.SteelBlue, new Vector2(x + vector3.X + 10, y + yOffset));
					yOffset += (int)vector1.Y;

					string text7d = " Pos: x,y,z: ";
					Vector2 vector4 = UserInterfaceHelper.FontSmall.MeasureString(text7d);

					DrawingHelper.DrawStringDropShadow(spriteBatch, text7d, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
					DrawingHelper.DrawStringDropShadow(spriteBatch, ((StellarObject)selectedObject).GetPositionRaw().X.ToString("0") + " , " + ((StellarObject)selectedObject).GetPositionRaw().Y.ToString("0") + " , " + ((StellarObject)selectedObject).GetPositionRaw().Z.ToString("0"), FontSize.Small, true, Color.SteelBlue, new Vector2(x + vector4.X + 10, y + yOffset));
				}

				var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
				var physicalMemInMB = currentProcess.WorkingSet64 / 1024 / 1024;
				string text8 = "Physical Mem usage: " + physicalMemInMB.ToString("#0") + " MB";
				yOffset += (int)vector1.Y;
				DrawingHelper.DrawStringDropShadow(spriteBatch, text8, FontSize.Small, true, foreColor, new Vector2(x, y + yOffset));
			}

			//var numFighters = galaxy.Ships.CountFightersOnCarriers(shouldBeCompleted: true);
			var numFighters = galaxy.Ships.CountByRole(ShipRole.FighterInterceptor, mustBeCompleted: true);
			var numBombers = galaxy.Ships.CountByRole(ShipRole.FighterBomber, mustBeCompleted: true);

			var numInBattle = galaxy.Ships.CountInBattle(includeFighters: false);
			var numFleets = galaxy.Fleets.Count;
			var numShips = galaxy.Ships.Count;
			var numEscorts = galaxy.Ships.CountByRole(ShipRole.Escort, mustBeCompleted: true);
			var numEscortsUnderConstruction = galaxy.Ships.CountByRole(ShipRole.Escort, mustBeCompleted: false);
			var numFrigates = galaxy.Ships.CountByRole(ShipRole.Frigate, mustBeCompleted: true);
			var numFrigatesUnderConstruction = galaxy.Ships.CountByRole(ShipRole.Frigate, mustBeCompleted: false);
			var numDestroyers = galaxy.Ships.CountByRole(ShipRole.Destroyer, mustBeCompleted: true);
			var numDestroyersUnderConstruction = galaxy.Ships.CountByRole(ShipRole.Destroyer, mustBeCompleted: false);
			var numCruisers = galaxy.Ships.CountByRole(ShipRole.Cruiser, mustBeCompleted: true);
			var numCruisersUnderConstruction = galaxy.Ships.CountByRole(ShipRole.Cruiser, mustBeCompleted: false);
			var numCarriers = galaxy.Ships.CountByRole(ShipRole.Carrier, mustBeCompleted: true);
			var numCarriersUnderConstruction = galaxy.Ships.CountByRole(ShipRole.Carrier, mustBeCompleted: false);
			var numCapitalShips = galaxy.Ships.CountByRole(ShipRole.CapitalShip, mustBeCompleted: true);
			var numCapitalShipsUnderConstruction = galaxy.Ships.CountByRole(ShipRole.CapitalShip, mustBeCompleted: false);

			var numDefensiveBases = galaxy.Ships.CountByRole(ShipRole.DefensiveBase, mustBeCompleted: true);
			var numDefensiveBasesUnderConstruction = galaxy.Ships.CountByRole(ShipRole.DefensiveBase, mustBeCompleted: false);

			string text9 = "Galaxy Military Summary:\n";
			text9 += " Fleets: " + numFleets.ToString() + "\n";
			text9 += " Ships: " + numShips.ToString() + "\n";
			text9 += "  (inBattle: " + numInBattle.ToString() + ")\n";
			text9 += " Fighters/Bombers: " + numFighters.ToString() + "/" + numBombers.ToString() + "\n";
			text9 += " Escorts: " + numEscorts.ToString() + " (" + (numEscortsUnderConstruction - numEscorts).ToString() + ")\n";
			text9 += " Frigates: " + numFrigates.ToString() + " (" + (numFrigatesUnderConstruction - numFrigates).ToString() + ")\n";
			text9 += " Destroyers: " + numDestroyers.ToString() + " (" + (numDestroyersUnderConstruction - numDestroyers).ToString() + ")\n";
			text9 += " Cruisers: " + numCruisers.ToString() + " (" + (numCruisersUnderConstruction - numCruisers).ToString() + ")\n";
			text9 += " Capitalships: " + numCapitalShips.ToString() + " (" + (numCapitalShipsUnderConstruction - numCapitalShips).ToString() + ")\n";
			text9 += " Carriers: " + numCarriers.ToString() + " (" + (numCarriersUnderConstruction - numCarriers).ToString() + ")\n";
			text9 += " DefensiveBases: " + numDefensiveBases.ToString() + " (" + (numDefensiveBasesUnderConstruction - numDefensiveBases).ToString() + ")\n";

			y = 0;
			x = region.Left - vector1.X - 300.0f;

			DrawingHelper.DrawStringDropShadow(spriteBatch, text9, FontSize.Small, true, Color.Gray, new Vector2(x, y));
		}
	}
}

