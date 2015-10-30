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
	public class Rendering
	{
		public Rendering ()
		{
			
		}

		public void WriteCursorPosition (int cursorX, int cursorY)
		{
			//Write the current X and Y values of the cursor to the title bar, to aid in drawing (hopefully)
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.White;
			Console.SetCursorPosition (Console.WindowWidth - 28, 0);
			Console.Write ("     ");
			Console.SetCursorPosition (Console.WindowWidth - 28, 0);
			Console.Write ("X: " + cursorX);
			Console.SetCursorPosition (Console.WindowWidth - 28, 1);
			Console.Write ("     ");
			Console.SetCursorPosition (Console.WindowWidth - 28, 1);
			Console.Write ("Y: " + cursorY);
		}

		public void DrawPaintGUI (string name, int width, int height, int viewportWidth, int viewportHeight)
		{
			for (int y = 0; y < Console.WindowHeight; y++) {
				Console.SetCursorPosition (Console.WindowWidth - 20, y);
				if (y == viewportHeight + 3 || y == 2)
					Console.Write ("┤");
				else
					Console.Write ("│");
			}

			//Draws the horizontal line for the title bar
			Console.SetCursorPosition (0, 2);
			for (int x = 0; x < Console.WindowWidth - 20; x++) {
				Console.Write ("─");
			}

			//Draws the horizontal line for the bottom of the image box
			Console.SetCursorPosition (0, viewportHeight + 3);
			for (int x = 0; x < Console.WindowWidth - 20; x++) {
				Console.Write ("─");
			}

			//Draw the title
			Console.SetCursorPosition (0, 0);
			Console.Write ("Editing " + name + ".aspic");

			//Calculate file size
			float rawfilesize = (width.ToString ().Length + height.ToString ().Length + 2) * 8;   //The size of the header, 
			//given by the number of digits in the width and height plus the dividers
			rawfilesize += (width * height) * 8 * 8;    //The size of the image data. The 8 * 8 gives us the size of each 
			//pixel in the worst case scenario (e.g. "15:15:8;")  We then multiply this by the overall image size to get an approximation of the file size.

			string fileSize = "";

			if (rawfilesize.ToString ().Length > 8)
				fileSize = ((rawfilesize / 1024) / 1024).ToString ("0.0") + "MB";
			else if (rawfilesize.ToString ().Length > 4)
				fileSize = (rawfilesize / 1024).ToString ("0.0") + "KB";
			else
				fileSize = rawfilesize.ToString () + "B";

			//Draw file information
			Console.SetCursorPosition (0, 1);
			Console.Write (width + "x" + height + ", approx. file size: " + fileSize);

			Console.SetCursorPosition (Console.WindowWidth - 18, 1);
			Console.Write ("Foreground:");
			Console.SetCursorPosition (Console.WindowWidth - 18, 4);
			Console.Write ("Background:");
			Console.SetCursorPosition (Console.WindowWidth - 18, 7);
			Console.Write ("Shading:");

			for (int currentColour = 0; currentColour < 16; currentColour++) {
				Console.BackgroundColor = (ConsoleColor)currentColour;
				Console.SetCursorPosition ((Console.WindowWidth - 18) + currentColour, 2);
				Console.Write (" ");
				Console.SetCursorPosition ((Console.WindowWidth - 18) + currentColour, 5);
				Console.Write (" ");
			}

			Console.BackgroundColor = ConsoleColor.Black;
			Console.SetCursorPosition (Console.WindowWidth - 18, 8);
			Console.Write ("░");
			Console.SetCursorPosition (Console.WindowWidth - 17, 8);
			Console.Write ("▒");
			Console.SetCursorPosition (Console.WindowWidth - 16, 8);
			Console.Write ("▓");
			Console.SetCursorPosition (Console.WindowWidth - 15, 8);
			Console.Write ("█");
			Console.SetCursorPosition (Console.WindowWidth - 14, 8);
			Console.Write ("▄");
			Console.SetCursorPosition (Console.WindowWidth - 13, 8);
			Console.Write ("▀");
			Console.SetCursorPosition (Console.WindowWidth - 12, 8);
			Console.Write ("▌");
			Console.SetCursorPosition (Console.WindowWidth - 11, 8);
			Console.Write ("▐");

			//Draw key guide

			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Black;

			Console.SetCursorPosition (Console.WindowWidth - 18, 11);
			Console.Write ("Arrow Keys:     ");
			Console.SetCursorPosition (Console.WindowWidth - 18, 13);
			Console.Write ("1 & 2:          ");
			Console.SetCursorPosition (Console.WindowWidth - 18, 15);
			Console.Write ("3 & 4:          ");
			Console.SetCursorPosition (Console.WindowWidth - 18, 17);
			Console.Write ("5 & 6:          ");
			Console.SetCursorPosition (Console.WindowWidth - 18, 19);
			Console.Write ("r:              ");
			Console.SetCursorPosition (Console.WindowWidth - 18, 21);
			Console.Write ("q:              ");

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;

			Console.SetCursorPosition (Console.WindowWidth - 16, 12);
			Console.Write ("move cursor");
			Console.SetCursorPosition (Console.WindowWidth - 16, 14);
			Console.Write ("inc/dec f. c.");
			Console.SetCursorPosition (Console.WindowWidth - 16, 16);
			Console.Write ("inc/dec b. c.");
			Console.SetCursorPosition (Console.WindowWidth - 16, 18);
			Console.Write ("inc/dec shading");
			Console.SetCursorPosition (Console.WindowWidth - 16, 20);
			Console.Write ("refresh screen");
			Console.SetCursorPosition (Console.WindowWidth - 16, 22);
			Console.Write ("quit w/o saving");

		}

		public void UpdatePaintGUI (int currentForeColour, int currentBackColour, int currentShading)
		{
			Console.BackgroundColor = (ConsoleColor)currentBackColour;
			Console.ForegroundColor = (ConsoleColor)currentForeColour;

			Console.SetCursorPosition (Console.WindowWidth - 18, 8);
			Console.Write ("░");
			Console.SetCursorPosition (Console.WindowWidth - 17, 8);
			Console.Write ("▒");
			Console.SetCursorPosition (Console.WindowWidth - 16, 8);
			Console.Write ("▓");
			Console.SetCursorPosition (Console.WindowWidth - 15, 8);
			Console.Write ("█");
			Console.SetCursorPosition (Console.WindowWidth - 14, 8);
			Console.Write ("▄");
			Console.SetCursorPosition (Console.WindowWidth - 13, 8);
			Console.Write ("▀");
			Console.SetCursorPosition (Console.WindowWidth - 12, 8);
			Console.Write ("▌");
			Console.SetCursorPosition (Console.WindowWidth - 11, 8);
			Console.Write ("▐");

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;

			Console.SetCursorPosition (Console.WindowWidth - 18, 3);
			Console.Write ("                ");  //Erase previous marker
			Console.SetCursorPosition ((Console.WindowWidth - 18) + currentForeColour, 3);
			Console.Write ("^");
			Console.SetCursorPosition (Console.WindowWidth - 18, 6);
			Console.Write ("                ");  //Erase previous marker
			Console.SetCursorPosition ((Console.WindowWidth - 18) + currentBackColour, 6);
			Console.Write ("^");
			Console.SetCursorPosition (Console.WindowWidth - 18, 9);
			Console.Write ("        ");  //Erase previous marker
			Console.SetCursorPosition ((Console.WindowWidth - 18) + (currentShading - 1), 9);
			Console.Write ("^");
		}

		/// <summary>
		/// Draws the specified area of the original image
		/// </summary>
		public void DrawSubImage (Image imageToRender, int viewportX, int viewportY, int viewportWidth, int viewportHeight)
		{
			for (int y = 3; y < viewportHeight + 3; y++) {
				Console.SetCursorPosition (0, y);
				for (int x = 0; x < viewportWidth; x++) {
                    //Get the current pixel's information
                    int[] cPixel = imageToRender.data[x + viewportX, (y - 3) + viewportY].GetPixel();

					Console.ForegroundColor = (ConsoleColor)cPixel[0];
					Console.BackgroundColor = (ConsoleColor)cPixel[1];

					switch (cPixel[2]) {
					case 1:
						Console.Write ("░");
						break;
					case 2:
						Console.Write ("▒");
						break;
					case 3:
						Console.Write ("▓");
						break;
					case 4:
						Console.Write ("█");
						break;
					case 5:
						Console.Write ("▄");
						break;
					case 6:
						Console.Write ("▀");
						break;
					case 7:
						Console.Write ("▌");
						break;
					case 8:
						Console.Write ("▐");
						break;
					}
				}
			}
		}

		public void DrawSinglePixel (Image.Pixel pixelToDraw, int x, int y)
		{
			Console.SetCursorPosition (x, y + 3);    //Account for title bar
			Console.ForegroundColor = (ConsoleColor)pixelToDraw.fc;
			Console.BackgroundColor = (ConsoleColor)pixelToDraw.bc;

			switch (pixelToDraw.s) {
			case 1:
				Console.Write ("░");
				break;
			case 2:
				Console.Write ("▒");
				break;
			case 3:
				Console.Write ("▓");
				break;
			case 4:
				Console.Write ("█");
				break;
			case 5:
				Console.Write ("▄");
				break;
			case 6:
				Console.Write ("▀");
				break;
			case 7:
				Console.Write ("▌");
				break;
			case 8:
				Console.Write ("▐");
				break;
			}
		}
	}
}

