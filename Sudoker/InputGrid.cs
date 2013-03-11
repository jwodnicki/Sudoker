using System;
using System.ComponentModel;

namespace Sudoker
{
	class InputCell : INotifyPropertyChanged
	{
		public string Id { get; set; }

		private char _value;
		public char Value
		{
			get { return _value; }
			set
			{
				_value = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Value"));
				}
			}
		}

		private byte _state;
		public byte State
		{
			get { return _state; }
			set
			{
				_state = 0;
				if (IsInvalid)     { _state |= 1 << 0; }
				if (IsUserEntered) { _state |= 1 << 1; }
				if (IsImmutable)   { _state |= 1 << 2; }
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("State"));
				}
			}
		}
		private bool _isInvalid;
		public bool IsInvalid
		{
			get { return _isInvalid; }
			set
			{
				_isInvalid = value;
				State = State;
			}
		}
		private bool _isUserEntered;
		public bool IsUserEntered
		{
			get { return _isUserEntered; }
			set
			{
				_isUserEntered = value;
				State = State;
			}
		}
		private bool _isImmutable;
		public bool IsImmutable
		{
			get { return _isImmutable; }
			set
			{
				_isImmutable = value;
				State = State;
			}
		}

		public InputCell(int row, int col, int value)
		{
			Id = row + "." + col;
			Value = value > 0 ? (char)('0' + value) : ' ';
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
	class InputGrid
	{
		public InputCell[][] Grid;
		private Explorer Explorer;
		private Solver Solver;

		public InputGrid()
		{
			Grid = new InputCell[9][];
			for (int row = 0; row < 9; row++)
			{
				Grid[row] = new InputCell[9];
				for (int col = 0; col < 9; col++)
				{
					Set(row, col, 0);
				}
			}
			Explorer = new Explorer(Grid);
			Solver = new Solver(Grid);
		}

		public void Set(int row, int col, int value)
		{
			Grid[row][col] = new InputCell(row, col, value);
		}

		public void Set(int row, int col, int value, bool isImmutable)
		{
			Grid[row][col] = new InputCell(row, col, value);
			Grid[row][col].IsImmutable = isImmutable;
		}

		public void Explore()
		{
			Explorer.Explore();
		}
		public void Explore(int row, int col, char value)
		{
			Explorer.Explore(row, col, value);
		}

		public void Solve()
		{
			Solver.Solve();
		}
	}
}
