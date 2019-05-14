using WindowsInput;
using WindowsInput.Native;
using System.Windows.Input;
using TowerUniteMidiDotNet.Windows;

namespace TowerUniteMidiDotNet.Core
{
	/// <summary>
	/// The class that contains a TowerUnite note's information and playback logic.
	/// </summary>
	public class Note
	{
		public readonly char NoteCharacter;
		public readonly VirtualKeyCode KeyCode;
		public readonly bool IsShiftedKey;
		private InputSimulator inputSim = new InputSimulator();

		/// <summary>
		/// Creates a new Note object.
		/// </summary>
		/// <param name="noteCharacter">The corresponding Tower Unite note character.</param>
		/// <param name="isShifted">Whether the shift key is required for this note or not.</param>
		public Note(char noteCharacter, bool isShifted = false)
		{
			NoteCharacter = noteCharacter;
			IsShiftedKey = isShifted;

			//Converting the character into a VirtualKeyCode, something the InputSimulator can read.
			KeyConverter converter = new KeyConverter();
			Key key = (Key)converter.ConvertFromString(noteCharacter.ToString());
			KeyCode = (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(key);
		}

		public void Play()
		{
			if (!IsShiftedKey)
			{
				inputSim.Keyboard.KeyDown(KeyCode);
				inputSim.Keyboard.Sleep(MainWindow.KeyDelay);
				inputSim.Keyboard.KeyUp(KeyCode);
			}
			else
			{
				inputSim.Keyboard.KeyDown(VirtualKeyCode.LSHIFT);
				inputSim.Keyboard.Sleep(MainWindow.KeyDelay);
				inputSim.Keyboard.KeyDown(KeyCode);
				inputSim.Keyboard.Sleep(MainWindow.KeyDelay);
				inputSim.Keyboard.KeyUp(KeyCode);
				inputSim.Keyboard.KeyUp(VirtualKeyCode.LSHIFT);
			}
		}
	}
}
