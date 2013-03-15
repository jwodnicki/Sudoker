using System;
using System.Collections.Specialized;

namespace Sudoker
{
	class Explorer
	{
		private SudokerGrid sGrid;
		private InputCell[][] iGrid;
		private BitVector32[][] bGrid;

		public Explorer(SudokerGrid grid)
		{
			sGrid = grid;
			iGrid = grid.Grid;
			bGrid = new BitVector32[9][];
			for (int row = 0; row < 9; row++)
			{
				bGrid[row] = new BitVector32[9];
			}
			oneBits();
		}

		public void Explore()
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					char value = iGrid[row][col].Value;
					if (!value.Equals(' '))
					{
						Explore(row, col, value - '1');
					}
				}
			}
		}
		public void Explore(int row, int col, char value)
		{
			if (value == ' ')
			{
				iGrid[row][col].Clear();
			}

			iGrid[row][col].Value = value;

			sGrid.ClearNonInput();
			oneBits();
			Explore();
		}
		public void Explore(int row, int col, int value)
		{
			iGrid[row][col].IsInvalid = !bGrid[row][col][1 << value];
			if (!iGrid[row][col].IsInvalid)
			{
				removeNumber(row, col, value);
			}
		}

		private void removeNumber(int row, int col, int value)
		{
			char cValue = (char)(value + '1');
			iGrid[row][col].Value = cValue;
			bGrid[row][col] = new BitVector32(1 << value);

			for (int i = 0; i < 9; i++)
			{
				if (i != row)
				{
					if (iGrid[i][col].Value.Equals(' '))
					{
						bGrid[i][col][1 << value] = false;
						for (int j = 0; j < 9; j++)
						{
							if (bGrid[i][col].Equals(new BitVector32(1 << j)))
							{
								removeNumber(i, col, j);
							}
						}
					}
					else if (iGrid[i][col].Value.Equals(cValue))
					{
						iGrid[i][col].IsInvalid = true;
						iGrid[row][col].IsInvalid = true;
					}
				}
				if (i != col)
				{
					if (iGrid[row][i].Value.Equals(' '))
					{
						bGrid[row][i][1 << value] = false;
						for (int j = 0; j < 9; j++)
						{
							if (bGrid[row][i].Equals(new BitVector32(1 << j)))
							{
								removeNumber(row, i, j);
							}
						}
					}
					else if (iGrid[row][i].Value.Equals(cValue))
					{
						iGrid[row][i].IsInvalid = true;
						iGrid[row][col].IsInvalid = true;
					}
				}
			}
			int boxRow0 = (row / 3) * 3;
			int boxCol0 = (col / 3) * 3;
			for (int i = boxRow0; i < boxRow0 + 3; i++)
			{
				for (int j = boxCol0; j < boxCol0 + 3; j++)
				{
					if (i != row && j != col)
					{
						if (iGrid[i][j].Value.Equals(' '))
						{
							bGrid[i][j][1 << value] = false;
							for (int k = 0; k < 9; k++)
							{
								if (bGrid[i][j].Equals(new BitVector32(1 << k)))
								{
									removeNumber(i, j, k);
								}
							}
						}
						else if (iGrid[i][j].Value.Equals(cValue))
						{
							iGrid[i][j].IsInvalid = true;
							iGrid[row][col].IsInvalid = true;
						}
					}
				}
			}
		}

		private void oneBits()
		{
			for (int row = 0; row < 9; row++)
			{
				for (int col = 0; col < 9; col++)
				{
					bGrid[row][col] = new BitVector32(0x1ff);
				}
			}
		}
	}
}
