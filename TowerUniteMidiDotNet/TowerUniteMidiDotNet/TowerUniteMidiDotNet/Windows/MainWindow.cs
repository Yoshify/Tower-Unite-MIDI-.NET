using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Devices;
using NAudio.Midi;
using TowerUniteMidiDotNet.Core;
using TowerUniteMidiDotNet.Util;
using NHotkey;
using NHotkey.WindowsForms;

namespace TowerUniteMidiDotNet
{
	public partial class MainWindow : Form
	{
		public const float VersionNumber = 1.0f;
		public static int KeyDelay = 15;

		private MidiIn midiIn;
		private int midiDeviceIndex = -1;
		private int noteLookupOctaveTransposition = 3;
		private MidiContainer currentMidi;
		private OpenFileDialog openFileDialog;
		private bool detailedLogging = false;
		private int midiTransposition = 0;
		private double midiPlaybackSpeed = 1.0;
		private Dictionary<int, Note> noteLookup;
		private Keys startKey = Keys.F1;
		private Keys stopKey = Keys.F2;

		//It's called this because I plan on adding AZERTY support. Eventually...
		private List<char> qwertyLookup = new List<char>()
		{
			'1','!','2','@','3','4','$','5','%','6','^','7',
			'8','*','9','(','0','q','Q','w','W','e','E','r',
			't','T','y','Y','u','i','I','o','O','p','P','a',
			's','S','d','D','f','g','G','h','H','j','J','k',
			'l','L','z','Z','x','c','C','v','V','b','B','n',
			'm'
		};

		/// <summary>
		/// A container class for holding a MIDI file, its Playback object and its filename.
		/// </summary>
		private class MidiContainer
		{
			public Melanchall.DryWetMidi.Smf.MidiFile MidiFile;
			public Playback MidiPlayback;
			public string MidiName;

			public MidiContainer(string name, Melanchall.DryWetMidi.Smf.MidiFile file)
			{
				MidiFile = file;
				MidiName = name;
				MidiPlayback = file.GetPlayback();
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			ScanDevices();
			BuildNoteDictionary();

			MIDIPlaybackSpeedSlider.Value = 10;
			MIDIPlaybackTransposeSlider.Value = 0;
			OctaveTranspositionSlider.Value = 0;
			StartListeningButton.Enabled = false;
			StopListeningButton.Enabled = false;
			MIDIPlayButton.Enabled = false;
			MIDIStopButton.Enabled = false;

			HotkeyManager.Current.AddOrReplace("Start", startKey, OnHotkeyPress);
			HotkeyManager.Current.AddOrReplace("Stop", stopKey, OnHotkeyPress);
			Text += " " + VersionNumber.ToString("0.0");
		}

		private void OnHotkeyPress(object sender, HotkeyEventArgs e)
		{
			switch(e.Name)
			{
				case "Start":

					if(TabControl.SelectedIndex == 0)
					{
						if(StartListeningButton.Enabled)
						{
							StartListening();
						}
					}
					else
					{
						if(MIDIPlayButton.Enabled)
						{
							PlayMidi();
						}
					}

					break;
				case "Stop":

					if (TabControl.SelectedIndex == 0)
					{
						if(StopListeningButton.Enabled)
						{
							StopListening();
						}
					}
					else
					{
						if(MIDIStopButton.Enabled)
						{
							StopMidi();
						}
					}

					break;
			}
		}

		/// <summary>
		/// Will look for any MIDI input devices connected to your computer.
		/// </summary>
		private void ScanDevices()
		{
			DeviceComboBox.Items.Clear();

			for (int device = 0; device < MidiIn.NumberOfDevices; device++)
			{
				DeviceComboBox.Items.Add(MidiIn.DeviceInfo(device).ProductName);
			}
		}

		/// <summary>
		/// Builds the note lookup dictionary for the program to reference. The key in the Dictionary is the notes MIDI number, whereas
		/// the value is the corresponding Note object itself.
		/// </summary>
		private void BuildNoteDictionary()
		{
			noteLookup = new Dictionary<int, Note>();

			for (int row = 0; row < 6; row++)
			{
				for (int column = 0; column < 12; column++)
				{
					if (row == 5 && column == 1)
					{
						break;
					}

					if (!char.IsLetterOrDigit(qwertyLookup[row * 12 + column]) || char.IsUpper(qwertyLookup[row * 12 + column]))
					{
						noteLookup.Add((row + noteLookupOctaveTransposition) * 12 + column, new Note(qwertyLookup[(row * 12 + column) - 1], true));
					}
					else
					{
						noteLookup.Add((row + noteLookupOctaveTransposition) * 12 + column, new Note(qwertyLookup[row * 12 + column]));
					}
				}
			}

			Log($"Note dictionary built. Middle C is C{noteLookupOctaveTransposition + 1}.");
		}

		/// <summary>
		/// Pushes <paramref name="logText"/> to the EventListView log. If there are more than 100 items in the log, the log will start being culled.
		/// </summary>
		/// <param name="logText">The text to push to the log.</param>
		private void Log(string logText)
		{
			if (EventListView.Items.Count > 99)
			{
				EventListView.Items.RemoveAt(0);
			}

			EventListView.Items.Add(logText);
			EventListView.Items[EventListView.Items.Count - 1].EnsureVisible();
		}

		#region MIDI Playback

		private void PlayMidi()
		{
			if (currentMidi == null || currentMidi.MidiPlayback.IsRunning)
			{
				return;
			}
			else if (currentMidi != null)
			{
				currentMidi.MidiPlayback.NotesPlaybackStarted -= OnMidiPlaybackNoteEventRecieved;
			}

			MIDIPlaybackTransposeSlider.Enabled = false;
			MIDIPlaybackSpeedSlider.Enabled = false;

			currentMidi.MidiPlayback.NotesPlaybackStarted += OnMidiPlaybackNoteEventRecieved;
			currentMidi.MidiPlayback.Finished += OnMidiPlaybackComplete;
			currentMidi.MidiPlayback.Speed = midiPlaybackSpeed;
			currentMidi.MidiPlayback.Start();
			Log($"Started playing {currentMidi.MidiName}.");
		}

		private void OnMidiPlaybackNoteEventRecieved(object sender, NotesEventArgs e)
		{
			foreach (Melanchall.DryWetMidi.Smf.Interaction.Note midiNote in e.Notes)
			{
				if (noteLookup.TryGetValue(midiNote.NoteNumber + midiTransposition, out Note note))
				{
					note.Play();
					if (detailedLogging)
					{
						Invoke((MethodInvoker)(() =>
						{
							Log($"Recieved MIDI number {midiNote.NoteNumber}, the note is {note.NoteCharacter}.");
						}));
					}
				}
			}
		}

		private void StopMidi()
		{
			if (currentMidi?.MidiPlayback.IsRunning == true)
			{
				MIDIPlaybackTransposeSlider.Enabled = true;
				MIDIPlaybackSpeedSlider.Enabled = true;
				currentMidi.MidiPlayback.Stop();
				currentMidi.MidiPlayback.MoveToStart();
				Log($"Stopped playing {currentMidi.MidiName}.");
			}
		}

		private void OnMidiPlaybackComplete(object sender, EventArgs e)
		{
			currentMidi.MidiPlayback.OutputDevice.Dispose();
		}

		#endregion

		#region MIDI In

		private void SelectDevice(int deviceIndex)
		{
			if (midiDeviceIndex != deviceIndex && midiIn != null)
			{
				midiIn.MessageReceived -= OnMidiEventRecieved;
				midiIn.Dispose();
			}

			midiDeviceIndex = deviceIndex;
			midiIn = new MidiIn(deviceIndex);
			midiIn.MessageReceived += OnMidiEventRecieved;
			Log($"Selected {MidiIn.DeviceInfo(deviceIndex).ProductName}.");
		}

		private void StartListening()
		{
			midiIn?.Start();
			Log($"Started listening to '{MidiIn.DeviceInfo(midiDeviceIndex).ProductName}'.");
		}

		private void StopListening()
		{
			midiIn?.Stop();
			Log($"Stopped listening to '{MidiIn.DeviceInfo(midiDeviceIndex).ProductName}'.");
		}

		private void OnMidiEventRecieved(object sender, MidiInMessageEventArgs e)
		{
			if (e.MidiEvent is NoteOnEvent)
			{
				var noteOnEvent = e.MidiEvent as NoteOnEvent;
				var noteEvent = e.MidiEvent as NoteOnEvent;
				noteLookup.TryGetValue(noteEvent.NoteNumber, out Note note);

				if (note != null)
				{
					note.Play();
					if (detailedLogging)
					{
						Invoke((MethodInvoker)(() =>
						{
							Log($"Recieved MIDI number {noteEvent.NoteNumber}, the note is {note.NoteCharacter}.");
						}));
					}
				}
			}
		}

		#endregion

		#region UI Event Handlers

		private void DeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(DeviceComboBox.SelectedIndex == midiDeviceIndex)
			{
				return;
			}

			SelectDevice(DeviceComboBox.SelectedIndex);
			StartListeningButton.Enabled = true;
			StopListeningButton.Enabled = true;
		}

		private void StartListeningButton_Click(object sender, EventArgs e)
		{
			StartListening();
		}

		private void StopListeningButton_Click(object sender, EventArgs e)
		{
			StopListening();
		}

		private void MIDIBrowseButton_Click(object sender, EventArgs e)
		{
			openFileDialog = new OpenFileDialog()
			{
				FileName = "Select your MIDI file.",
				Filter = "MIDI Files (*.mid;*.midi)|*.mid;*.midi",
				Title = "Open MIDI File",
				InitialDirectory = @"C:\"
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				currentMidi?.MidiPlayback.OutputDevice.Dispose();
				currentMidi = new MidiContainer(openFileDialog.SafeFileName, Melanchall.DryWetMidi.Smf.MidiFile.Read(openFileDialog.FileName));
				MIDIPlayButton.Enabled = true;
				MIDIStopButton.Enabled = true;
				Log($"Loaded {openFileDialog.SafeFileName}.");
			}
		}

		private void MIDIPlayButton_Click(object sender, EventArgs e)
		{
			PlayMidi();
		}

		private void MIDIStopButton_Click(object sender, EventArgs e)
		{
			StopMidi();
		}

		private void InputDeviceScanButton_Click(object sender, EventArgs e)
		{
			ScanDevices();
		}

		private void InputPingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Prompt.ShowIntDialog("By inputting your ping, the program can work out an appropriate amount of delay to reduce the chance of a missed black key. Note that this will add latency to your keys.", "Input your ping", out int result))
			{
				if(result == 0)
				{
					return;
				}

				KeyDelay = result / 6;
				KeyDelay = (KeyDelay < 8) ? 8 : KeyDelay;
				Log($"Your ping is {result}, your calculated delay is {KeyDelay}ms.");
			}
		}

		private void DetailedLoggingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
			detailedLogging = menuItem.Checked = !menuItem.Checked;
		}

		private void MIDIPlaybackSpeedSlider_ValueChanged(object sender, EventArgs e)
		{
			midiPlaybackSpeed = MIDIPlaybackSpeedSlider.Value / 10.0;
			ToolTipController.SetToolTip((TrackBar)sender, midiPlaybackSpeed.ToString());
			
			if(currentMidi != null)
			{
				currentMidi.MidiPlayback.Speed = midiPlaybackSpeed;
			}
		}

		private void MIDIPlaybackTransposeSlider_ValueChanged(object sender, EventArgs e)
		{
			if (MIDIPlaybackTransposeSlider.Value > 0)
			{
				ToolTipController.SetToolTip((TrackBar)sender, $"+{MIDIPlaybackTransposeSlider.Value.ToString()} semitones");
			}
			else
			{
				ToolTipController.SetToolTip((TrackBar)sender, $"{MIDIPlaybackTransposeSlider.Value.ToString()} semitones");
			}

			midiTransposition = MIDIPlaybackTransposeSlider.Value;
		}

		private void OctaveTranspositionSlider_ValueChanged(object sender, EventArgs e)
		{
			if (OctaveTranspositionSlider.Value > 0)
			{
				ToolTipController.SetToolTip((TrackBar)sender, $"+{OctaveTranspositionSlider.Value.ToString()} octaves");
			}
			else
			{
				ToolTipController.SetToolTip((TrackBar)sender, $"{OctaveTranspositionSlider.Value.ToString()} octaves");
			}

			noteLookupOctaveTransposition = 3 + OctaveTranspositionSlider.Value;
			BuildNoteDictionary();
		}

		#endregion
	}
}
