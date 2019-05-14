using System.Windows.Forms;

namespace TowerUniteMidiDotNet.Util
{
	public static class Prompt
	{
		public static bool ShowIntDialog(string labelText, string titleText, out int result)
		{
			result = 0;

			Form prompt = new Form()
			{
				Width = 320,
				Height = 150,
				Text = titleText,
				FormBorderStyle = FormBorderStyle.FixedDialog,
				MaximizeBox = false,
				MinimizeBox = false,
				AutoSize = true,
				StartPosition = FormStartPosition.CenterParent
			};

			Label textLabel = new Label()
			{
				Left = 10,
				Top = 5,
				Text = labelText,
				Width = 300,
				Height = 60,
				AutoSize = false
			};

			NumericUpDown inputBox = new NumericUpDown()
			{
				Left = 10,
				Top = 55,
				Width = 300
			};

			Button confirmationButton = new Button()
			{
				Text = "Confirm",
				Left = 80,
				Top = 80,
				Width = 160,
				Height = 25
			};

			confirmationButton.Click += (sender, e) =>
			{
				prompt.Close();
			};

			prompt.Controls.Add(confirmationButton);
			prompt.Controls.Add(inputBox);
			prompt.Controls.Add(textLabel);
			prompt.ShowDialog();

			if(int.TryParse(inputBox.Text, out int parsed))
			{
				result = parsed;
				return true;
			}

			return false;
		}
	}
}
