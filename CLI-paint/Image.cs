/*
    Copyright 2015 Samuel Kinnett
     

    This file is part of CLI-paint.

    CLI-paint is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CLI-paint is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CLI-paint.  If not, see <http://www.gnu.org/licenses/>.

*/
using System;

namespace CLI_paint
{
	public class Image
	{
		public int[,] fcBuffer;
		public int[,] bcBuffer;
		public int[,] sBuffer;

        public int width;
        public int height;
        public string name;

		public Image (string name, int width, int height)
		{
			this.name = name;
			this.width = width;
			this.height = height;

			fcBuffer = new int[width, height];
			bcBuffer = new int[width, height];
			sBuffer = new int[width, height];

			//Initialise the three buffers such that the image would be pure black.

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					fcBuffer [x, y] = 0;
					bcBuffer [x, y] = 0;
					sBuffer [x, y] = 4;
				}
			}
		}
	}
}

