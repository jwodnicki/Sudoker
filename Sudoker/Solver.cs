using System;
using System.Collections.Specialized;

namespace Sudoker
{
	class Solver
	{
		private InputCell[][] iGrid;
		private char[] cGrid;
		private BitVector32[] bRow;
		private BitVector32[] bCol;
		private BitVector32[] bBox;

		public Solver(InputCell[][] grid)
		{
			iGrid = grid;
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

			for (int i = 0; i < cGrid.Length; i++)
			{
				if (cGrid[i] == 0)
				{
					return;
				}
			}
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					iGrid[row][col].Value = cGrid[row * 9 + col];
					iGrid[row][col].IsInvalid = false;
				}
			}
		}
		private bool solve(int pos)
		{
			if (pos == 81)
			{
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
				if (solve(pos + 1))
				{
					cGrid[pos] = (char)(i + '1');
					return true;
				}
				else
				{
					bRow[row][val] = false;
					bCol[col][val] = false;
					bBox[box][val] = false;
				}
			}

			return false;
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
