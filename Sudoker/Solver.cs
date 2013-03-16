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
			sGrid.ClearNonInput();
			solve();
		}
		public void solve()
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					if (iGrid[row][col].IsInvalid)
					{
						return;
					}
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

		public void GenerateRandom()
		{
			initialize();
			sGrid.ClearAll();

			int[] code = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			Util.Shuffle(code);
			for (int row = 0; row < 9; row++)
			{
				iGrid[row][0].Value = (char)('0' + code[row]);
			}
			Util.Shuffle(code, 3);
			for (int col = 1; col < 7; col++)
			{
				iGrid[0][col].Value = (char)('0' + code[col + 2]);
			}
			iGrid[0][7].Value = (char)('0' + code[1]);
			iGrid[0][8].Value = (char)('0' + code[2]);

			solve();
			ChooseSolution(Util.Random.Next(1, 1 + solutionList.Solutions.Count));

			var iGridOrig = new char[9][];
			for (int row = 0; row < 9; row++)
			{
				iGridOrig[row] = new char[9];
				for (int col = 0; col < 9; col++)
				{
					iGridOrig[row][col] = iGrid[row][col].Value;
				}
			}

			int numberOfCellsToFill = Util.Random.Next(15, 30);
			for (int i = 0; i < numberOfCellsToFill; i++)
			{
				iGrid[Util.Random.Next(9)][Util.Random.Next(9)].IsImmutable = true;
			}
			for(;;)
			{
				Solve();
				if (solutionList.Solutions.Count <= 1 || numberOfCellsToFill > 50)
				{
					break;
				}
				int row = Util.Random.Next(9);
				int col = Util.Random.Next(9);
				iGrid[row][col].Value = iGridOrig[row][col];
				iGrid[row][col].IsImmutable = true;
				numberOfCellsToFill++;
			}
			if (numberOfCellsToFill > 50)
			{
				GenerateRandom();
				return;
			}
			solutionList.Clear();
		}

		private void initialize()
		{
			cGrid = new char[81];
			for (int i = 0; i < 9; i++)
			{
				bRow[i] = new BitVector32(0);
				bCol[i] = new BitVector32(0);
				bBox[i] = new BitVector32(0);
			}
		}
	}
}
