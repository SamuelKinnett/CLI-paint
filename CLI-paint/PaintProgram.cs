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
    along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

*/
using System;

namespace CLI_paint
{
	class PaintProgram
	{

		public void Run ()
		{
			string userInput;
			bool exitProgram = false;

			while (!exitProgram) {

				//Ensure that the background is black
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;

				Console.Clear ();
				Console.WriteLine ("Welcome to Console Paint!");
				Console.WriteLine ("Would you like to (C)reate or (O)pen a picture?");

				userInput = Console.ReadLine ().ToUpper ();

				switch (userInput) {
				case "C":
					Console.Write ("Image Name: ");
					string name = Console.ReadLine ();
					Console.Write ("Please enter width: ");
					int width = int.Parse (Console.ReadLine ());
					Console.Write ("Please enter height: ");
					int height = int.Parse (Console.ReadLine ());
					Paint (name, width, height);
					exitProgram = true;
					break;
				case "O":
					break;
				}
			}
		}

		void Paint (string name, int width, int height)
		{
			bool exitLoop = false;

			int[,] foreColorBuffer = new int[width, height];
			int[,] backColorBuffer = new int[width, height];
			int[,] shadingBuffer = new int[width, height];

			int cScreenWidth;	//current screen width
			int cScreenHeight;	//current screen height

			//Initialise the three buffers such that the image would be pure black.

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					foreColorBuffer [x, y] = 0;
					backColorBuffer [x, y] = 0;
					shadingBuffer [x, y] = 4;
				}
			}

			/* Currently, window resizing is disabled in order to test the viewport
			//Ensure the console window is never too small
			if (width > 60) {
				if (height > 21)
					Console.SetWindowSize (width + 20, height + 4);
				else
					Console.SetWindowSize (width + 20, 25);
			} else if (height > 21) {
				Console.SetWindowSize (80, height + 4);
			} else {
				Console.SetWindowSize (80, 25);
			}
			*/

			cScreenWidth = Console.WindowWidth;
			cScreenHeight = Console.WindowHeight;

			Console.Clear ();

			int cursorX = 0;
			int cursorY = 0;

			int viewportX = 0;
			int viewportY = 0;
			int viewportWidth = Console.WindowWidth - 20;
			int viewportHeight = Console.WindowHeight - 4;

			int currentForeColour = 0;
			int currentBackColour = 0;
			int currentShading = 1;

			DrawPaintGUI (name, viewportWidth, viewportHeight);
			UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
			WriteCursorPosition (cursorX, cursorY);

			while (!exitLoop) {

				//Check if the window has been resized
				if (cScreenWidth != Console.WindowWidth || cScreenHeight != Console.WindowHeight) {
					Console.Clear ();
					DrawPaintGUI (name, viewportWidth, viewportHeight);
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					DrawSubImage (foreColorBuffer, backColorBuffer, shadingBuffer, width, height, viewportX, viewportY, viewportWidth, viewportHeight);

					cScreenWidth = Console.WindowWidth;
					cScreenHeight = Console.WindowHeight;
					viewportWidth = Console.WindowWidth - 20;
					viewportHeight = Console.WindowHeight - 4;
				}

				WriteCursorPosition (cursorX, cursorY);

				Console.SetCursorPosition (cursorX - viewportX, cursorY - viewportY + 3);
				Console.Write ("X");
				Console.SetCursorPosition (cursorX - viewportX, cursorY - viewportY + 3);

				int userInput = (int)Console.ReadKey (true).Key;

				switch (userInput) {
				case 37:
                        //Left arrow
					if (cursorX - 1 >= viewportX) {
						DrawSinglePixel (foreColorBuffer [cursorX, cursorY], backColorBuffer [cursorX, cursorY], shadingBuffer [cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorX--;
					} else {
						if (viewportX > 0) {
							viewportX--;
							cursorX--;
							DrawSubImage (foreColorBuffer, backColorBuffer, shadingBuffer, width, height, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 38:
                        //Up arrow
					if (cursorY - 1 >= viewportY) {
						DrawSinglePixel (foreColorBuffer [cursorX, cursorY], backColorBuffer [cursorX, cursorY], shadingBuffer [cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorY--;
					} else {
						if (viewportY > 0) {
							viewportY--;
							cursorY--;
							DrawSubImage (foreColorBuffer, backColorBuffer, shadingBuffer, width, height, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 39:
                        //Right arrow
					if (cursorX + 1 < viewportX + viewportWidth) {
						DrawSinglePixel (foreColorBuffer [cursorX, cursorY], backColorBuffer [cursorX, cursorY], shadingBuffer [cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorX++;
					} else {
						if (viewportX + viewportWidth < width) {
							viewportX++;
							cursorX++;
							DrawSubImage (foreColorBuffer, backColorBuffer, shadingBuffer, width, height, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 40:
                        //Down arrow
					if (cursorY + 1 < viewportY + viewportHeight) {
						DrawSinglePixel (foreColorBuffer [cursorX, cursorY], backColorBuffer [cursorX, cursorY], shadingBuffer [cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorY++;
					} else {
						if (viewportY + viewportHeight < height) {
							viewportY++;
							cursorY++;
							DrawSubImage (foreColorBuffer, backColorBuffer, shadingBuffer, width, height, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 32:
                        //Space bar
					foreColorBuffer [cursorX, cursorY] = currentForeColour;
					backColorBuffer [cursorX, cursorY] = currentBackColour;
					shadingBuffer [cursorX, cursorY] = currentShading;
					DrawSinglePixel (currentForeColour, currentBackColour, currentShading, cursorX - viewportX, cursorY - viewportY);
					break;
				case 49:
                        //1, decrease fcolor
					if (currentForeColour > 0)
						currentForeColour--;
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 50:
                        //2, increase fcolor
					if (currentForeColour < 15)
						currentForeColour++;
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 51:
                        //3, decrease bcolor
					if (currentBackColour > 0)
						currentBackColour--;
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 52:
                        //4, increase bcolor
					if (currentBackColour < 15)
						currentBackColour++;
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 53:
                        //5, decrease shading
					if (currentShading > 1)
						currentShading--;
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 54:
                        //6, increase shading
					if (currentShading < 8)
						currentShading++;
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 82:
                        //r - refresh screen
					Console.ForegroundColor = ConsoleColor.White;
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Clear ();
					DrawPaintGUI (name, viewportWidth, viewportHeight);
					UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					WriteCursorPosition (cursorX, cursorY);
					DrawEntirePicture (foreColorBuffer, backColorBuffer, shadingBuffer, width, height);
					break;
				case 81:
                        //q - quit
					exitLoop = true;
					break;
				}
			}

		}

		void WriteCursorPosition (int cursorX, int cursorY)
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

		void DrawPaintGUI (string name, int width, int height)
		{
			for (int y = 0; y < Console.WindowHeight; y++) {
				Console.SetCursorPosition (Console.WindowWidth - 20, y);
				if (y == height + 3 || y == 2)
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
			Console.SetCursorPosition (0, height + 3);
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

		void UpdatePaintGUI (int currentForeColour, int currentBackColour, int currentShading)
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

		void DrawEntirePicture (int[,] fcbuffer, int[,] bcbuffer, int[,] sbuffer, int width, int height)
		{
			for (int y = 3; y < height + 3; y++) {
				Console.SetCursorPosition (0, y);
				for (int x = 0; x < width; x++) {
					Console.ForegroundColor = (ConsoleColor)fcbuffer [x, y - 3];
					Console.BackgroundColor = (ConsoleColor)bcbuffer [x, y - 3];

					switch (sbuffer [x, y - 3]) {
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

		/// <summary>
		/// Draws the specified area of the original image
		/// </summary>
		/// <param name="fcbuffer">Fcbuffer.</param>
		/// <param name="bcbuffer">Bcbuffer.</param>
		/// <param name="sbuffer">Sbuffer.</param>
		/// <param name="imageWidth">Image width.</param>
		/// <param name="imageHeight">Image height.</param>
		/// <param name="viewportWidth">Viewport width.</param>
		/// <param name="viewportHeight">Viewport height.</param>
		void DrawSubImage (int[,] fcbuffer, int[,] bcbuffer, int[,] sbuffer, int imageWidth, int imageHeight, int viewportX, int viewportY, int viewportWidth, int viewportHeight)
		{
			for (int y = 3; y < viewportHeight + 3; y++) {
				Console.SetCursorPosition (0, y);
				for (int x = 0; x < viewportWidth; x++) {
					Console.ForegroundColor = (ConsoleColor)fcbuffer [x + viewportX, (y - 3) + viewportY];
					Console.BackgroundColor = (ConsoleColor)bcbuffer [x + viewportX, (y - 3) + viewportY];

					switch (sbuffer [x + viewportX, (y - 3) + viewportY]) {
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

		void DrawSinglePixel (int fcolor, int bcolor, int shading, int x, int y)
		{
			Console.SetCursorPosition (x, y + 3);    //Account for title bar
			Console.ForegroundColor = (ConsoleColor)fcolor;
			Console.BackgroundColor = (ConsoleColor)bcolor;

			switch (shading) {
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
