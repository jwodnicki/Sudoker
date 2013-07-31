using System;
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
		private SudokerGrid sudokerGrid;
		public MainWindow()
		{
			InitializeComponent();
			sudokerGrid = new SudokerGrid();
			uiGrid.ItemsSource = sudokerGrid.Items;
			solutionChooser.ItemsSource = sudokerGrid.SolutionList.Solutions;
			sudokerGrid.Explore();

			//for (int i = 1; i < 7; i++)
			//{
			//	for (int j = 1; j < 8; j++)
			//	{
			//		sudokerGrid.Set(i, j, (i * 3 + i / 3 + j) % 9 + 1, true);
			//	}
			//}
		}

		private void AllowOnlyInt(object sender, TextCompositionEventArgs e)
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

		private void OnExplore(object sender, EventArgs e)
		{
			var textbox = (TextBox)sender;

			if (textbox.IsReadOnly)
			{
				return;
			}

			string[] rc = textbox.Tag.ToString().Split('.');
			int r = Convert.ToInt32(rc[0]);
			int c = Convert.ToInt32(rc[1]);

			sudokerGrid.Items[r][c].IsUserEntered = true;
			bool wasDeleted = textbox.Text.Length == 0;

			if (explorerCb.IsChecked == true)
			{
				sudokerGrid.Explore(r, c, wasDeleted ? ' ' : textbox.Text[0]);
			}

			if (wasDeleted)
			{
				textbox.SelectAll();
			}
		}

		private void OnExploreSelect(object sender, EventArgs e)
		{
			sudokerGrid.Explore();
		}

		private void OnSolve(object sender, EventArgs e)
		{
			sudokerGrid.Solve();
			solutionChooser.SelectedIndex = 0;
		}

		private void OnGenerateRandom(object sender, EventArgs e)
		{
			sudokerGrid.GenerateRandom();
		}

		private void OnClearAll(object sender, EventArgs e)
		{
			sudokerGrid.ClearAll();
		}

		private void OnChooseSolution(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				sudokerGrid.ChooseSolution(((Solution)e.AddedItems[0]).ID);
			}
			catch { }
		}

		private void SetBorder(object sender, EventArgs e)
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
