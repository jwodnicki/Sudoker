using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Sudoker
{
	class Solution
	{
		public int ID { get; set; }
		public char[] Grid { get; set; }
	}
	class SolutionList : ViewModel
	{
		public ObservableCollection<Solution> Solutions { get; set; }

		private int idSeq;
		public SolutionList()
		{
			Solutions = new ObservableCollection<Solution>();
			idSeq = 0;
		}
		public void Add(char[] grid)
		{
			Solutions.Add(new Solution() { ID = ++idSeq, Grid = grid });
		}
		public void Clear()
		{
			Solutions.Clear();
			idSeq = 0;
		}
	}

	class Solver
	{
		private SolutionList solutionList;
		private SudokerGrid sGrid;
		private InputCell[][] iGrid;
		private char[] cGrid;
		private BitVector32[] bRow;
		private BitVector32[] bCol;
		private BitVector32[] bBox;

		public Solver(SudokerGrid grid, SolutionList solutions)
		{
			sGrid = grid;
			iGrid = grid.Grid;
			solutionList = solutions;
			bRow = new BitVector32[9];
			bCol = new BitVector32[9];
			bBox = new BitVector32[9];
		}

		public void Solve()
		{
			initialize();

			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					char cVal = iGrid[row][col].Value;
					if (cVal.Equals(' '))
					{
						continue;
					}
					int val = 1 << cVal - '1';
					cGrid[row * 9 + col] = cVal;
					bRow[row][val] = true;
					bCol[col][val] = true;
					bBox[((row / 3) * 9 + col) / 3][val] = true;
				}
			}

			solve(0);
		}
		private bool solve(int pos)
		{
			if (pos == 81)
			{
				solutionList.Add((char[])cGrid.Clone());
				return true;
			}
			if (cGrid[pos] != 0)
			{
				return solve(pos + 1);
			}

			int row = pos / 9;
			int col = pos % 9;
			int box = ((row / 3) * 9 + col) / 3;

			for (int i = 0; i < 9; i++)
			{
				int val = 1 << i;
				if (
					bRow[row][val] ||
					bCol[col][val] ||
					bBox[box][val]
					)
				{
					continue;
				}
				bRow[row][val] = true;
				bCol[col][val] = true;
				bBox[box][val] = true;
				cGrid[pos] = (char)(i + '1');
				if (solve(pos + 1))
				{
					continue;
				}
				else
				{
					bRow[row][val] = false;
					bCol[col][val] = false;
					bBox[box][val] = false;
					cGrid[pos] = (char)0;
				}
			}

			return false;
		}

		public void ChooseSolution(int id)
		{
			var cGrid = solutionList.Solutions[id - 1].Grid;
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					iGrid[row][col].Value = cGrid[row * 9 + col];
					iGrid[row][col].IsInvalid = false;
				}
			}
		}

		private void initialize()
		{
			solutionList.Clear();
			cGrid = new char[81];
			for (int i = 0; i < 9; i++)
			{
				bRow[i] = new BitVector32(0);
				bCol[i] = new BitVector32(0);
				bBox[i] = new BitVector32(0);
			}
			sGrid.ClearNonUserInput();
		}
	}
}
