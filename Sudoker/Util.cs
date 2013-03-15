using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoker
{
	public class Util
	{
		public static Random Random = new Random();
		public static void Shuffle<T>(T[] array)
		{
			Shuffle(array, 0);
		}
		public static void Shuffle<T>(T[] array, int startIndex)
		{
			for (int i = array.Length; i > startIndex; i--)
			{
				int j = Random.Next(startIndex, i);
				T tmp = array[j];
				array[j] = array[i - 1];
				array[i - 1] = tmp;
			}
		}
	}
}
