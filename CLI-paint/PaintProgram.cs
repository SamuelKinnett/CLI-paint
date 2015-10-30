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
	class PaintProgram
	{

		Rendering GUI;
		FileManager fileManager;

		public PaintProgram ()
		{
			GUI = new Rendering ();
			fileManager = new FileManager ();
		}

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
				Console.WriteLine ("Would you like to (C)reate or (OC/O)pen a picture, or (Q)uit?");

				userInput = Console.ReadLine ().ToUpper ();

				switch (userInput) {
				case "C":
					Console.Write ("Image Name: ");
					string name = Console.ReadLine ();
					Console.Write ("Please enter width: ");
					int width = int.Parse (Console.ReadLine ());
					Console.Write ("Please enter height: ");
					int height = int.Parse (Console.ReadLine ());
					Paint (new Image (name, width, height));
					break;
				case "O":
					Console.Write ("Name of image to load: ");
					string imageName = Console.ReadLine ();
					Paint (fileManager.LoadImage (imageName));
					break;
                case "OC":
                    Console.Write ("Name of compressed image to load: ");
					string imageName2 = Console.ReadLine ();
					Paint (fileManager.LoadImageCompressed (imageName2));
                    break;
                case "Q":
                    exitProgram = true;
                    break;
				}
			}
		}

		void Paint (Image image)
		{
			bool exitLoop = false;

			int cScreenWidth;	//current screen width
			int cScreenHeight;	//current screen height

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
			int viewportWidth = 0;
			int viewportHeight = 0;

			//If the viewport is smaller than the drawing area, we need to set the viewport size to the image size
			if (image.width < Console.WindowWidth - 20) {
				viewportWidth = image.width;
			} else {
				viewportWidth = Console.WindowWidth - 20;
			}
			if (image.height < Console.WindowHeight - 4) {
				viewportHeight = image.height;
			} else {
				viewportHeight = Console.WindowHeight - 4;
			}

			int currentForeColour = 0;
			int currentBackColour = 0;
			int currentShading = 1;

			GUI.DrawPaintGUI (image.name, image.width, image.height, viewportWidth, viewportHeight);
			GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
			GUI.WriteCursorPosition (cursorX, cursorY);
            GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);

			while (!exitLoop) {

				//Check if the window has been resized
				if (cScreenWidth != Console.WindowWidth || cScreenHeight != Console.WindowHeight) {
					Console.Clear ();
					GUI.DrawPaintGUI (image.name, image.width, image.height, viewportWidth, viewportHeight);
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
                    GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);

					cScreenWidth = Console.WindowWidth;
					cScreenHeight = Console.WindowHeight;
					viewportWidth = Console.WindowWidth - 20;
					viewportHeight = Console.WindowHeight - 4;
				}

				GUI.WriteCursorPosition (cursorX, cursorY);

				Console.SetCursorPosition (cursorX - viewportX, cursorY - viewportY + 3);
				Console.Write ("X");
				Console.SetCursorPosition (cursorX - viewportX, cursorY - viewportY + 3);

				int userInput = (int)Console.ReadKey (true).Key;

				switch (userInput) {
				case 37:
                        //Left arrow
					if (cursorX - 1 >= viewportX) {
						GUI.DrawSinglePixel (image.data[cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorX--;
					} else {
						if (viewportX > 0) {
							viewportX--;
							cursorX--;
                            GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 38:
                        //Up arrow
					if (cursorY - 1 >= viewportY) {
                        GUI.DrawSinglePixel(image.data[cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorY--;
					} else {
						if (viewportY > 0) {
							viewportY--;
							cursorY--;
                            GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 39:
                        //Right arrow
					if (cursorX + 1 < viewportX + viewportWidth) {
                        GUI.DrawSinglePixel(image.data[cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorX++;
					} else {
						if (viewportX + viewportWidth < image.width) {
							viewportX++;
							cursorX++;
                            GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 40:
                        //Down arrow
					if (cursorY + 1 < viewportY + viewportHeight) {
                        GUI.DrawSinglePixel(image.data[cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
						cursorY++;
					} else {
						if (viewportY + viewportHeight < image.height) {
							viewportY++;
							cursorY++;
                            GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
						}
					}
					break;
				case 32:
                        //Space bar
                    image.data[cursorX, cursorY].SetPixel(currentForeColour, currentBackColour, currentShading);
					GUI.DrawSinglePixel (image.data[cursorX, cursorY], cursorX - viewportX, cursorY - viewportY);
					break;
				case 49:
                        //1, decrease fcolor
					if (currentForeColour > 0)
						currentForeColour--;
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 50:
                        //2, increase fcolor
					if (currentForeColour < 15)
						currentForeColour++;
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 51:
                        //3, decrease bcolor
					if (currentBackColour > 0)
						currentBackColour--;
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 52:
                        //4, increase bcolor
					if (currentBackColour < 15)
						currentBackColour++;
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 53:
                        //5, decrease shading
					if (currentShading > 1)
						currentShading--;
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 54:
                        //6, increase shading
					if (currentShading < 8)
						currentShading++;
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					break;
				case 82:
                        //r - refresh screen
					Console.ForegroundColor = ConsoleColor.White;
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Clear ();
					GUI.DrawPaintGUI (image.name, image.width, image.height, viewportWidth, viewportHeight);
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					GUI.WriteCursorPosition (cursorX, cursorY);
                    GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
					break;
				case 83:
					//s - save the image
					Console.ForegroundColor = ConsoleColor.White;
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Clear ();
					Console.WriteLine ("Saving...");
					fileManager.SaveImage (image);
					Console.Clear ();
					GUI.DrawPaintGUI (image.name, image.width, image.height, viewportWidth, viewportHeight);
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					GUI.WriteCursorPosition (cursorX, cursorY);
                    GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
					break;
                case 67:
                    //c - save a compressed image
                    Console.ForegroundColor = ConsoleColor.White;
					Console.BackgroundColor = ConsoleColor.Black;
					Console.Clear ();
					Console.WriteLine ("Compressing & Saving...");
					fileManager.SaveImageCompressed (image);
					Console.Clear ();
					GUI.DrawPaintGUI (image.name, image.width, image.height, viewportWidth, viewportHeight);
					GUI.UpdatePaintGUI (currentForeColour, currentBackColour, currentShading);
					GUI.WriteCursorPosition (cursorX, cursorY);
                    GUI.DrawSubImage(image, viewportX, viewportY, viewportWidth, viewportHeight);
					break;
				case 81:
                        //q - quit
					exitLoop = true;
					break;
				}
			}

		}
	}
}
