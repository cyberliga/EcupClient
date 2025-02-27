﻿#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Clipboard = System.Windows.Clipboard;

#endregion

namespace Hearthstone_Deck_Tracker.Utility.HotKeys
{
	public class PredefinedHotKeyActionInfo
	{
		public PredefinedHotKeyActionInfo(string title, string description, string methodName)
		{
			Title = title;
			Description = description;
			MethodName = methodName;
		}

		public string Title { get; }

		public string Description { get; }
		public string MethodName { get; }
	}

	public class PredefinedHotKeyActions
	{
		public static IEnumerable<PredefinedHotKeyActionInfo> PredefinedActionNames
		{
			get
			{
				return
					typeof(PredefinedHotKeyActions).GetMethods()
					                               .Where(x => x.GetCustomAttributes(typeof(PredefinedHotKeyActionAttribute), false).Any())
					                               .Select(x =>
					                               {
						                               var attr =
							                               ((PredefinedHotKeyActionAttribute)
							                                x.GetCustomAttributes(typeof(PredefinedHotKeyActionAttribute), false)[0]);
													   return new PredefinedHotKeyActionInfo(attr.Title, attr.Description, x.Name);
					                               });
			}
		}

		[PredefinedHotKeyAction("Toggle overlay", "Turns the overlay on or off (if the game is running).")]
		public static void ToggleOverlay()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideOverlay = !Config.Instance.HideOverlay;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle overlay: card marks",
			"Turns the card marks and age on the overlay on or off (if the game is running).")]
		public static void ToggleOverlayCardMarks()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideOpponentCardMarks = !Config.Instance.HideOpponentCardMarks;
			Config.Instance.HideOpponentCardAge = Config.Instance.HideOpponentCardMarks;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle overlay: secrets", "Turns the secrets panel on the overlay on or off (if the game is running).")]
		public static void ToggleOverlaySecrets()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideSecrets = !Config.Instance.HideSecrets;
			Core.MainWindow.Options.OptionsOverlayOpponent.CheckboxShowSecrets.IsChecked = !Config.Instance.HideSecrets;
			Config.Save();
			if(Config.Instance.HideSecrets)
				Core.Overlay.HideSecrets();
			else
				Core.Overlay.UnhideSecrects();
		}

		[PredefinedHotKeyAction("Toggle overlay: timers", "Turns the timers on the overlay on or off (if the game is running).")]
		public static void ToggleOverlayTimer()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideTimers = !Config.Instance.HideTimers;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle overlay: attack icons", "Turns both attack icons on the overlay on or off (if the game is running).")
		]
		public static void ToggleOverlayAttack()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HidePlayerAttackIcon = !Config.Instance.HidePlayerAttackIcon;
			Config.Instance.HideOpponentAttackIcon = Config.Instance.HidePlayerAttackIcon;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle overlay: active effects", "Turns both active effects on the overlay on or off (if the game is running).")
		]
		public static void ToggleOverlayActiveEffects()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HidePlayerActiveEffects = !Config.Instance.HidePlayerActiveEffects;
			Config.Instance.HidePlayerActiveEffects = Config.Instance.HidePlayerActiveEffects;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle overlay: counters", "Turns both counters on the overlay on or off (if the game is running).")
		]
		public static void ToggleOverlayCounters()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HidePlayerCounters = !Config.Instance.HidePlayerCounters;
			Config.Instance.HidePlayerCounters = Config.Instance.HidePlayerCounters;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle overlay: Related Cards", "Turns related cards tooltips on the overlay on or off (if the game is running).")
		]
		public static void ToggleOverlayRelatedCards()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HidePlayerRelatedCards = !Config.Instance.HidePlayerRelatedCards;
			Config.Instance.HideOpponentRelatedCards = Config.Instance.HidePlayerRelatedCards;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("Toggle My Games panel", "Turns on or off visibility of My Games panel.")]
		public static void ToggleMyGamesPanel()
		{
			Config.Instance.ShowMyGamesPanel = !Config.Instance.ShowMyGamesPanel;
			Core.MainWindow.Options.OptionsTrackerGeneral.CheckboxShowMyGamesPanel.IsChecked = Config.Instance.ShowMyGamesPanel;
			Core.MainWindow.UpdateMyGamesPanelVisibility();
			Config.Save();
		}

		[PredefinedHotKeyAction("Toggle no deck mode", "Activates \"no deck mode\" (use no deck) or selects the last used deck.")]
		public static void ToggleNoDeckMode()
		{
			if(DeckList.Instance.ActiveDeck == null)
				DeckList.Instance.ActiveDeck = DeckList.Instance.GetLastUsedDeck();
			else
				DeckList.Instance.ActiveDeck = null;
		}

		[PredefinedHotKeyAction("Edit active deck", "Opens the edit dialog for the active deck (if any) and brings HDT to foreground.")]
		public static void EditDeck()
		{
			if(DeckList.Instance.ActiveDeck == null)
				return;
			Core.MainWindow.ShowDeckEditorFlyout(DeckList.Instance.ActiveDeck, false);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Import from game: arena", "Starts the webimport process with all dialogs.")]
		public static void ImportFromArena()
		{
			Core.MainWindow.StartArenaImporting().Forget();
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Import from game: constructed", "Starts the webimport process with all dialogs.")]
		public static void ImportFromConstructed()
		{
			Core.MainWindow.ShowImportDialog(false);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Import from web", "Starts the webimport process with all dialogs.")]
		public static void ImportFromWeb()
		{
			Core.MainWindow.ImportDeck();
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Import from web: clipboard", "Starts the webimport process without the import dialog.")]
		public static void ImportFromWebClipboard()
		{
			var clipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "could not get text from clipboard";
			Core.MainWindow.ImportDeck(clipboard);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Import from web: highlight",
			"Starts the webimport process without any dialogs. This sends a \"ctrl-c\" command before starting the import: just highlight the url and press the hotkey."
			)]
		public static async void ImportFromWebHighlight()
		{
			SendKeys.SendWait("^c");
			await Task.Delay(200);
			var clipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "could not get text from clipboard";
			Core.MainWindow.ImportDeck(clipboard);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Screenshot",
			"Creates a screenshot of the game and overlay (and everything else in front of it). Comes with an option to automatically upload to imgur."
			)]
		public static async void Screenshot()
		{
			var handle = User32.GetHearthstoneWindow();
			if(handle == IntPtr.Zero)
				return;
			var rect = User32.GetHearthstoneRect(false);
			var bmp = await ScreenCapture.CaptureHearthstoneAsync(new Point(0, 0), rect.Width, rect.Height, handle, false, false);
			if(bmp == null)
			{
				Log.Error("There was an error capturing hearthstone.");
				return;
			}
			using(var mem = new MemoryStream())
			{
				var encoder = new PngBitmapEncoder();
				bmp.Save(mem, ImageFormat.Png);
				encoder.Frames.Add(BitmapFrame.Create(mem));
				await Core.MainWindow.SaveOrUploadScreenshot(encoder, "Hearthstone " + DateTime.Now.ToString("MM-dd-yy hh-mm-ss"));
			}
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Game Screenshot",
			"Creates a screenshot of the game only. Comes with an option to automatically upload to imgur."
			)]
		public static async void GameScreenshot()
		{
			var handle = User32.GetHearthstoneWindow();
			if(handle == IntPtr.Zero)
				return;
			var rect = User32.GetHearthstoneRect(false);
			var bmp = await ScreenCapture.CaptureHearthstoneAsync(new Point(0, 0), rect.Width, rect.Height, handle, false, true);
			if(bmp == null)
			{
				Log.Error("There was an error capturing hearthstone.");
				return;
			}
			using(var mem = new MemoryStream())
			{
				var encoder = new PngBitmapEncoder();
				bmp.Save(mem, ImageFormat.Png);
				encoder.Frames.Add(BitmapFrame.Create(mem));
				await Core.MainWindow.SaveOrUploadScreenshot(encoder, "Hearthstone " + DateTime.Now.ToString("MM-dd-yy hh-mm-ss"));
			}
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Note Dialog", "Brings up the note dialog for the current (running) game.")]
		public static void NoteDialog()
		{
			if(Core.Game.IsRunning && !Core.Game.IsInMenu && Core.Game.CurrentGameStats != null)
				new NoteDialog(Core.Game.CurrentGameStats).Show();
		}

		[PredefinedHotKeyAction("Start Hearthstone", "Starts the Battle.net launcher and/or Hearthstone.")]
		public static void StartHearthstone()
		{
			HearthstoneRunner.StartHearthstone().Forget();
		}

		[PredefinedHotKeyAction("Show main window", "Brings up the main window.")]
		public static void ShowMainWindow()
		{
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("Show stats", "Brings up the stats window or flyout.")]
		public static void ShowStats()
		{
			Core.MainWindow.ShowStats(false, false);
		}

		[PredefinedHotKeyAction("Reload deck", "Resets HDT to last game start.")]
		public static void ReloadDeck()
		{
			_ = Core.Reset();
		}

		[PredefinedHotKeyAction("Close HDT", "Closes HDT.")]
		public static void CloseHdt()
		{
			_ = Core.Shutdown();
		}
	}
}
