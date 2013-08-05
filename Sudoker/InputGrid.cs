using System;

namespace Sudoker
{
	class InputCell : ViewModel
	{
		public string Id { get; set; }

		private char _value;
		public char Value
		{
			get { return _value; }
			set
			{
				_value = value;
				NotifyPropertyChanged("Value");
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
				NotifyPropertyChanged("State");
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
		public void Clear()
		{
			Value = ' ';
			IsInvalid = false;
			IsUserEntered = false;
			IsImmutable = false;
		}
	}
	class SudokerGrid
	{
		public InputCell[][] Items;
		public SolutionList SolutionList;
		private Explorer explorer;
		private Solver solver;

		public SudokerGrid()
		{
			Items = new InputCell[9][];
			for (int row = 0; row < 9; row++)
			{
				Items[row] = new InputCell[9];
				for (int col = 0; col < 9; col++)
				{
					Set(row, col, 0);
				}
			}
			SolutionList = new SolutionList();
			explorer = new Explorer(this);
			solver = new Solver(this, SolutionList);
		}

		public void Set(int row, int col, int value)
		{
			Items[row][col] = new InputCell(row, col, value);
		}
		public void Set(int row, int col, int value, bool isImmutable)
		{
			Items[row][col] = new InputCell(row, col, value);
			Items[row][col].IsImmutable = isImmutable;
		}

		public void ClearAll()
		{
			SolutionList.Clear();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					Items[i][j].Clear();
				}
			}
		}

		public void ClearNonInput()
		{
			SolutionList.Clear();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					if (!Items[i][j].IsUserEntered && !Items[i][j].IsImmutable)
					{
						Items[i][j].Clear();
					}
				}
			}
		}

		public void Explore()
		{
			explorer.Explore();
		}
		public void Explore(int row, int col, char value)
		{
			explorer.Explore(row, col, value);
		}

		public void Solve()
		{
			solver.Solve(-1);
		}

		public void ChooseSolution(int id)
		{
			solver.ChooseSolution(id);
		}

		public void GenerateRandom()
		{
			solver.GenerateRandom();
		}
	}
}
