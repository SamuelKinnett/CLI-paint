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
        /// <summary>
        /// Class for storing pixel information and methods
        /// </summary>
        public class Pixel
        {
            public int fc;  //Forecolour
            public int bc;  //Backcolour
            public int s;   //Shading

            public Pixel(int fc, int bc, int s)
            {
                this.fc = fc;
                this.bc = bc;
                this.s = s;
            }

            public void SetPixel(int fc, int bc, int s)
            {
                this.fc = fc;
                this.bc = bc;
                this.s = s;
            }

            public int[] GetPixel()
            {
                return new int[3] { fc, bc, s };
            }

            //Comparison of two pixels
            public static bool operator ==(Pixel pxOne, Pixel pxTwo)
            {
                if (pxOne.fc == pxTwo.fc && pxOne.bc == pxTwo.bc && pxOne.s == pxTwo.s)
                    return true;
                else
                    return false;
            }

            public static bool operator !=(Pixel pxOne, Pixel pxTwo)
            {
                if (pxOne.fc == pxTwo.fc && pxOne.bc == pxTwo.bc && pxOne.s == pxTwo.s)
                    return false;
                else
                    return true;
            }
        }
        public Pixel[,] data;

        public int width;
        public int height;
        public string name;

		public Image (string name, int width, int height)
		{
			this.name = name;
			this.width = width;
			this.height = height;

            data = new Pixel[width, height];

			//Initialise the three buffers such that the image would be pure black.

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
                    data[x, y] = new Pixel(0, 0, 4);
				}
			}
		}
	}
}

