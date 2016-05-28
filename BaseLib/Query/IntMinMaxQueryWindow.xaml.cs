﻿using System.Windows.Input;

namespace BaseLib.Query{
	/// <summary>
	/// Interaction logic for IntQueryWindow.xaml
	/// </summary>
	public partial class IntMinMaxQueryWindow{
		public IntMinMaxQueryWindow(int value1, int value2, int min, int max){
			InitializeComponent();
			NumUpDown1.Minimum = min;
			NumUpDown1.Maximum = max;
			NumUpDown2.Minimum = min;
			NumUpDown2.Maximum = max;
			NumUpDown1.Value = value1;
			NumUpDown2.Value = value2;
			NumUpDown1.Focus();
		}

		public int Value1 => (int) NumUpDown1.Value;
		public int Value2 => (int) NumUpDown2.Value;

		private void CancelButtonClick(object sender, System.EventArgs e){
			DialogResult = false;
			Close();
		}

		private void OkButtonClick(object sender, System.EventArgs e){
			DialogResult = true;
			Close();
		}

		private void OnKeyDownHandler(object sender, KeyEventArgs e){
			if (e.Key == Key.Return){
				DialogResult = true;
				Close();
			}
		}

		//TODO
		private void NumericUpDown1ValueChanged(object sender, System.EventArgs e){
			if (NumUpDown2.Value < NumUpDown1.Value){
				NumUpDown2.Value = NumUpDown1.Value;
			}
		}

		//TODO
		private void NumericUpDown2ValueChanged(object sender, System.EventArgs e){
			if (NumUpDown2.Value < NumUpDown1.Value){
				NumUpDown1.Value = NumUpDown2.Value;
			}
		}
	}
}