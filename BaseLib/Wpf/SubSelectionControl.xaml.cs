﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BaseLibS.Num;
using BaseLibS.Param;

namespace BaseLib.Wpf{
	/// <summary>
	/// Interaction logic for SubSelectionControl.xaml
	/// </summary>
	public partial class SubSelectionControl{
		private Thread downThread;
		private Thread upThread;
		private Func<string[], Parameters> parameterFuncs;
		internal readonly List<Parameters> parameters = new List<Parameters>();
		internal MultiListSelectorControl MultiListSelectorControl { get; set; }

		internal Func<string[], Parameters> ParameterFuncs{
			get { return parameterFuncs; }
			set{
				ParameterButton.Visibility = Visibility.Visible;
				parameterFuncs = value;
			}
		}

		public SubSelectionControl(){
			InitializeComponent();
			H1.Source = WpfUtils.LoadBitmap(Properties.Resources.top);
			H2.Source = WpfUtils.LoadBitmap(Properties.Resources.up);
			H3.Source = WpfUtils.LoadBitmap(Properties.Resources.down);
			H4.Source = WpfUtils.LoadBitmap(Properties.Resources.bottom);
			H5.Source = WpfUtils.LoadBitmap(Properties.Resources.filter);
			H6.Source = WpfUtils.LoadBitmap(Properties.Resources.right);
			H7.Source = WpfUtils.LoadBitmap(Properties.Resources.left);
			DownButton.MouseDown += DownButtonMouseDown;
			DownButton.MouseUp += DownButtonMouseUp;
			UpButton.MouseDown += UpButtonMouseDown;
			UpButton.MouseUp += UpButtonMouseUp;
		}

		public string Text{
			set { TitleBlock.Text = value; }
			get { return TitleBlock.Text; }
		}

		internal ItemCollection SelectedItems => SelectedListBox.Items;

		internal string[] SelectedStrings{
			get{
				ItemCollection sel = SelectedItems;
				string[] result = new string[sel.Count];
				for (int i = 0; i < result.Length; i++){
					result[i] = sel[i].ToString();
				}
				return result;
			}
		}

		private static int[] GetSelectedIndices(ListBox box){
			int[] result = new int[box.SelectedItems.Count];
			for (int i = 0; i < result.Length; i++){
				result[i] = box.Items.IndexOf(box.SelectedItems[i]);
			}
			return result;
		}

		private void SetOrder(IList<int> order, IEnumerable<int> selection){
			object[] items = new object[SelectedListBox.Items.Count];
			for (int i = 0; i < items.Length; i++){
				items[i] = SelectedListBox.Items[i];
			}
			SelectedListBox.Items.Clear();
			SelectedListBox.UnselectAll();
			items = ArrayUtils.SubArray(items, order);
			foreach (object item in items){
				SelectedListBox.Items.Add(item);
			}
			foreach (int i in selection){
				SelectedListBox.SelectedItems.Add(items[i]);
			}
		}

		private void DownButtonMouseUp(object sender, MouseButtonEventArgs e){
			downThread.Abort();
			downThread = null;
		}

		private void UpButtonMouseUp(object sender, MouseButtonEventArgs e){
			upThread.Abort();
			upThread = null;
		}

		private void DownButtonMouseDown(object sender, MouseButtonEventArgs e){
			downThread = new Thread(WalkDown);
			downThread.Start();
		}

		private void UpButtonMouseDown(object sender, MouseButtonEventArgs e){
			upThread = new Thread(WalkUp);
			upThread.Start();
		}

		private void WalkDown(){
			Thread.Sleep(400);
			while (true){
				Dispatcher.Invoke(new Action(() => DownButtonClick(null, null)));
				Thread.Sleep(150);
			}
// ReSharper disable FunctionNeverReturns
		}

// ReSharper restore FunctionNeverReturns
		private void WalkUp(){
			Thread.Sleep(400);
			while (true){
				Dispatcher.Invoke(new Action(() => UpButtonClick(null, null)));
				Thread.Sleep(150);
			}
// ReSharper disable FunctionNeverReturns
		}

// ReSharper restore FunctionNeverReturns
		private void TopButtonClick(object sender, EventArgs e){
			int[] selectedIndices = GetSelectedIndices(SelectedListBox);
			if (selectedIndices.Length == 0){
				return;
			}
			int n = SelectedListBox.Items.Count;
			int[] unselectedIndices = ArrayUtils.Complement(selectedIndices, n);
			int[] order = ArrayUtils.Concat(selectedIndices, unselectedIndices);
			int[] selection = ArrayUtils.ConsecutiveInts(selectedIndices.Length);
			SetOrder(order, selection);
			parameters.Clear();
		}

		private void UpButtonClick(object sender, EventArgs e){
			int[] selectedIndices = GetSelectedIndices(SelectedListBox);
			if (selectedIndices.Length == 0){
				return;
			}
			Array.Sort(selectedIndices);
			int index = -1;
			for (int i = selectedIndices.Length - 1; i >= 0; i--){
				if (i == 0 || selectedIndices[i - 1] < selectedIndices[i] - 1){
					index = i;
					break;
				}
			}
			int q = selectedIndices[index];
			if (q == 0){
				return;
			}
			int m = selectedIndices.Length - index;
			int n = SelectedListBox.Items.Count;
			int[] order = new int[n];
			for (int i = 0; i < q - 1; i++){
				order[i] = i;
			}
			for (int i = 0; i < m; i++){
				order[q - 1 + i] = q + i;
			}
			order[q + m - 1] = q - 1;
			for (int i = q + m; i < n; i++){
				order[i] = i;
			}
			int[] selection = selectedIndices;
			for (int i = index; i < selection.Length; i++){
				selection[i]--;
			}
			SetOrder(order, selection);
			parameters.Clear();
		}

		private void DownButtonClick(object sender, EventArgs e){
			int[] selectedIndices = GetSelectedIndices(SelectedListBox);
			if (selectedIndices.Length == 0){
				return;
			}
			Array.Sort(selectedIndices);
			int index = -1;
			for (int i = 0; i < selectedIndices.Length; i++){
				if (i == selectedIndices.Length - 1 || selectedIndices[i + 1] > selectedIndices[i] + 1){
					index = i;
					break;
				}
			}
			int q = selectedIndices[0];
			int n = SelectedListBox.Items.Count;
			if (selectedIndices[index] == n - 1){
				return;
			}
			int m = index + 1;
			int[] order = new int[n];
			for (int i = 0; i < q; i++){
				order[i] = i;
			}
			order[q] = q + m;
			for (int i = 0; i < m; i++){
				order[q + 1 + i] = q + i;
			}
			for (int i = q + m + 1; i < n; i++){
				order[i] = i;
			}
			int[] selection = selectedIndices;
			for (int i = 0; i <= index; i++){
				selection[i]++;
			}
			SetOrder(order, selection);
			parameters.Clear();
		}

		private void BottomButtonClick(object sender, EventArgs e){
			int[] selectedIndices = GetSelectedIndices(SelectedListBox);
			if (selectedIndices.Length == 0){
				return;
			}
			int n = SelectedListBox.Items.Count;
			int[] unselectedIndices = ArrayUtils.Complement(selectedIndices, n);
			int[] order = ArrayUtils.Concat(unselectedIndices, selectedIndices);
			int[] selection = ArrayUtils.ConsecutiveInts(n - selectedIndices.Length, n);
			SetOrder(order, selection);
			parameters.Clear();
		}

		private void ParameterButtonClick(object sender, EventArgs e){
			if (SelectedStrings.Length == 0){
				return;
			}
			Point p = ParameterButton.PointToScreen(new Point(0, 0));
			FilterWindow fw = new FilterWindow(this){
				Top = p.Y,
				Left = p.X - 895,
				Title = "Filter " + Text.ToLower() + " columns"
			};
			fw.ShowDialog();
		}

		//TODO
		//protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
		//	switch (keyData) {
		//		case Keys.Control | Keys.A:
		//			System.Windows.Controls.Control c = GetChildAtPoint(System.Windows.Forms.Cursor.Position);
		//			if (c != null) {
		//				if (c.Equals(selectedListBox)) {
		//					MultiListSelector.SelectAll(selectedListBox);
		//				}
		//			}
		//			Invalidate(true);
		//			break;
		//	}
		//	return base.ProcessCmdKey(ref msg, keyData);
		//}
		private void Select_OnClick(object sender, RoutedEventArgs e){
			parameters.Clear();
			object[] os = new object[MultiListSelectorControl.AllListBox.SelectedItems.Count];
			MultiListSelectorControl.AllListBox.SelectedItems.CopyTo(os, 0);
			foreach (object o in os){
				if (!SelectedListBox.Items.Contains(o)){
					SelectedListBox.Items.Add(o);
				}
			}
			foreach (object o in os){
				MultiListSelectorControl.AllListBox.Items.Remove(o);
			}
			MultiListSelectorControl.SelectionHasChanged(this, e);
		}

		private void Deselect_OnClick(object sender, RoutedEventArgs e){
			parameters.Clear();
			object[] os = new object[SelectedListBox.SelectedItems.Count];
			SelectedListBox.SelectedItems.CopyTo(os, 0);
			foreach (object o in os){
				SelectedListBox.Items.Remove(o);
			}
			foreach (object o in os){
				MultiListSelectorControl.AllListBox.Items.Add(o);
			}
			MultiListSelectorControl.SelectionHasChanged(this, e);
		}

		public Parameters GetParameters(){
			return parameterFuncs(SelectedStrings);
		}
	}
}