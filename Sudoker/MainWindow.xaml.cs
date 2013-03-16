using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Sudoker
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SudokerGrid SudokerGrid;
		public MainWindow()
		{
			InitializeComponent();
			SudokerGrid = new SudokerGrid();
			grid.ItemsSource = SudokerGrid.Grid;
			solutionChooser.ItemsSource = SudokerGrid.SolutionList.Solutions;
			SudokerGrid.Explore();

			//for (int i = 1; i < 7; i++)
			//{
			//	for (int j = 1; j < 8; j++)
			//	{
			//		SudokerGrid.Set(i, j, (i * 3 + i / 3 + j) % 9 + 1, true);
			//	}
			//}
		}

		private void eventAllowOnlyInt(object sender, TextCompositionEventArgs e)
		{
			if (!Char.IsDigit(e.Text[0]) || e.Text[0].Equals('0'))
			{
				window.Left -= 8;
				System.Threading.Thread.Sleep(50);
				window.Left += 16;
				System.Threading.Thread.Sleep(50);
				window.Left -= 8;
				e.Handled = true;
			}
		}

		private void eventExplore(object sender, EventArgs e)
		{
			var textbox = (TextBox)sender;

			if (textbox.IsReadOnly)
			{
				return;
			}

			string[] rc = textbox.Tag.ToString().Split('.');
			int r = Convert.ToInt32(rc[0]);
			int c = Convert.ToInt32(rc[1]);

			SudokerGrid.Grid[r][c].IsUserEntered = true;
			bool wasDeleted = textbox.Text.Length == 0;

			if (explorerCb.IsChecked == true)
			{
				SudokerGrid.Explore(r, c, wasDeleted ? ' ' : textbox.Text[0]);
			}

			if (wasDeleted)
			{
				textbox.SelectAll();
			}
		}

		private void eventExploreSelect(object sender, EventArgs e)
		{
			SudokerGrid.Explore();
		}

		private void eventSolve(object sender, EventArgs e)
		{
			SudokerGrid.Solve();
			solutionChooser.SelectedIndex = 0;
		}

		private void eventGenerateRandom(object sender, EventArgs e)
		{
			SudokerGrid.GenerateRandom();
		}

		private void eventClearAll(object sender, EventArgs e)
		{
			SudokerGrid.ClearAll();
		}

		private void eventChooseSolution(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				SudokerGrid.ChooseSolution(((Solution)e.AddedItems[0]).ID);
			}
			catch { }
		}

		private void eventSetBorder(object sender, EventArgs e)
		{
			var textbox = (TextBox)sender;
			string[] rc = textbox.Tag.ToString().Split('.');
			int r = Convert.ToInt32(rc[0]);
			int c = Convert.ToInt32(rc[1]);
			textbox.BorderThickness = new Thickness(1, 1, c == 2 || c == 5 ? 5 : 1, r == 2 || r == 5 ? 5 : 1);
		}
	}

	public class ColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = (byte)value;
			return
				(val & (1 << 0)) == 1 << 0 ? Brushes.Red :
				(val & (1 << 1)) == 1 << 1 ? Brushes.CornflowerBlue :
				Brushes.Black;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class WeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = (byte)value;
			return
				(val & (1 << 1)) == 1 << 1 ||
				(val & (1 << 2)) == 1 << 2 ? FontWeights.Bold :
				FontWeights.Normal;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class MutableConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((byte)value & (1 << 2)) == 1 << 2;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	public class ComboVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Hidden;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
