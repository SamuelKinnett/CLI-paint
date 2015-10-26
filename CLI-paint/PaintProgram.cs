﻿using System;

namespace CLI_paint
{
    class PaintProgram
    {

        public void Run()
        {
            string userInput;
            bool exitProgram = false;

            while (!exitProgram) {

                Console.Clear();
                Console.WriteLine("Welcome to Console Paint!");
                Console.WriteLine("Would you like to (C)reate or (O)pen a picture?");

                userInput = Console.ReadLine().ToUpper();

                switch (userInput) {
                    case "C":
                        Console.Write("Image Name: ");
                        string name = Console.ReadLine();
                        Console.Write("Please enter width: ");
                        int width = int.Parse(Console.ReadLine());
                        Console.Write("Please enter height: ");
                        int height = int.Parse(Console.ReadLine());
                        Paint(name, width, height);
                        exitProgram = true;
                        break;
                    case "O":
                        break;
                }
            }
        }

        void Paint(string name, int width, int height)
        {
            bool exitLoop = false;

            int[,] foreColorBuffer = new int[width, height];
            int[,] backColorBuffer = new int[width, height];
            int[,] shadingBuffer = new int[width, height];

            //Initialise the three buffers such that the image would be pure black.

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    foreColorBuffer[x, y] = 0;
                    backColorBuffer[x, y] = 0;
                    shadingBuffer[x, y] = 4;
                }
            }

            //Ensure the console window is never too small
            if (width > 60)
                Console.WindowWidth = width + 20;
            else
                Console.WindowWidth = 80;
            if (height > 21)
                Console.WindowHeight = height + 4;
            else
                Console.WindowWidth = 25;

            Console.Clear();
            DrawPaintGUI(name, width, height);

            int cursorX = 0;
            int cursorY = 0;
            int currentForeColour = 0;
            int currentBackColour = 0;
            int currentShading = 1;

            UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
            WriteCursorPosition(cursorX, cursorY);

            while (!exitLoop) {

                WriteCursorPosition(cursorX, cursorY);

                Console.SetCursorPosition(cursorX, cursorY + 3);    //Account for title bar offset

                int userInput = (int)Console.ReadKey(true).Key;

                switch (userInput) {
                    case 37:
                        //Left arrow
                        if (cursorX - 1 >= 0)
                            cursorX--;
                        break;
                    case 38:
                        //Up arrow
                        if (cursorY - 1 >= 0)
                            cursorY--;
                        break;
                    case 39:
                        //Right arrow
                        if (cursorX + 1 < width)
                            cursorX++;
                        break;
                    case 40:
                        //Down arrow
                        if (cursorY + 1 < height)
                            cursorY++;
                        break;
                    case 32:
                        //Space bar
                        foreColorBuffer[cursorX, cursorY] = currentForeColour;
                        backColorBuffer[cursorX, cursorY] = currentBackColour;
                        shadingBuffer[cursorX, cursorY] = currentShading;
                        DrawSinglePixel(currentForeColour, currentBackColour, currentShading, cursorX, cursorY);
                        break;
                    case 49:
                        //1, decrease fcolor
                        if (currentForeColour > 0)
                            currentForeColour--;
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        break;
                    case 50:
                        //2, increase fcolor
                        if (currentForeColour < 15)
                            currentForeColour++;
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        break;
                    case 51:
                        //3, decrease bcolor
                        if (currentBackColour > 0)
                            currentBackColour--;
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        break;
                    case 52:
                        //4, increase bcolor
                        if (currentBackColour < 15)
                            currentBackColour++;
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        break;
                    case 53:
                        //5, decrease shading
                        if (currentShading > 1)
                            currentShading--;
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        break;
                    case 54:
                        //6, increase shading
                        if (currentShading < 8)
                            currentShading++;
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        break;
                    case 82:
                        //r - refresh screen
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Clear();
                        DrawPaintGUI(name, width, height);
                        UpdatePaintGUI(currentForeColour, currentBackColour, currentShading);
                        WriteCursorPosition(cursorX, cursorY);
                        DrawEntirePicture(foreColorBuffer, backColorBuffer, shadingBuffer, width, height);
                        break;
                    case 81:
                        //q - quit
                        exitLoop = true;
                        break;
                }
            }

        }

        void WriteCursorPosition(int cursorX, int cursorY)
        {
            //Write the current X and Y values of the cursor to the title bar, to aid in drawing (hopefully)
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth - 28, 0);
            Console.Write("     ");
            Console.SetCursorPosition(Console.WindowWidth - 28, 0);
            Console.Write("X: " + cursorX);
            Console.SetCursorPosition(Console.WindowWidth - 28, 1);
            Console.Write("     ");
            Console.SetCursorPosition(Console.WindowWidth - 28, 1);
            Console.Write("Y: " + cursorY);
        }

        void DrawPaintGUI(string name, int width, int height)
        {
            for (int y = 0; y < Console.WindowHeight; y++) {
                Console.SetCursorPosition(Console.WindowWidth - 20, y);
                if (y == height + 3 || y == 2)
                    Console.Write("┤");
                else
                    Console.Write("│");
            }

            //Draws the horizontal line for the title bar
            Console.SetCursorPosition(0, 2);
            for (int x = 0; x < Console.WindowWidth - 20; x++) {
                Console.Write("─");
            }

            //Draws the horizontal line for the bottom of the image box
            Console.SetCursorPosition(0, height + 3);
            for (int x = 0; x < Console.WindowWidth - 20; x++) {
                Console.Write("─");
            }

            //Draw the title
            Console.SetCursorPosition(0, 0);
            Console.Write("Editing " + name + ".aspic");

            //Calculate file size
            float rawfilesize = (width.ToString().Length + height.ToString().Length + 2) * 8;   //The size of the header, 
            //given by the number of digits in the width and height plus the dividers
            rawfilesize += (width * height) * 8 * 8;    //The size of the image data. The 8 * 8 gives us the size of each 
            //pixel in the worst case scenario (e.g. "15:15:8;")  We then multiply this by the overall image size to get an approximation of the file size.

            string fileSize = "";

            if (rawfilesize.ToString().Length > 8)
                fileSize = ((rawfilesize / 1024) / 1024).ToString("0.0") + "MB";
            else if (rawfilesize.ToString().Length > 4)
                fileSize = (rawfilesize / 1024).ToString("0.0") + "KB";
            else
                fileSize = rawfilesize.ToString() + "B";

            //Draw file information
            Console.SetCursorPosition(0, 1);
            Console.Write(width + "x" + height + ", approx. file size: " + fileSize);

            Console.SetCursorPosition(Console.WindowWidth - 18, 1);
            Console.Write("Foreground:");
            Console.SetCursorPosition(Console.WindowWidth - 18, 4);
            Console.Write("Background:");
            Console.SetCursorPosition(Console.WindowWidth - 18, 7);
            Console.Write("Shading:");

            for (int currentColour = 0; currentColour < 16; currentColour++) {
                Console.BackgroundColor = (ConsoleColor)currentColour;
                Console.SetCursorPosition((Console.WindowWidth - 18) + currentColour, 2);
                Console.Write(" ");
                Console.SetCursorPosition((Console.WindowWidth - 18) + currentColour, 5);
                Console.Write(" ");
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(Console.WindowWidth - 18, 8);
            Console.Write("░");
            Console.SetCursorPosition(Console.WindowWidth - 17, 8);
            Console.Write("▒");
            Console.SetCursorPosition(Console.WindowWidth - 16, 8);
            Console.Write("▓");
            Console.SetCursorPosition(Console.WindowWidth - 15, 8);
            Console.Write("█");
            Console.SetCursorPosition(Console.WindowWidth - 14, 8);
            Console.Write("▄");
            Console.SetCursorPosition(Console.WindowWidth - 13, 8);
            Console.Write("▀");
            Console.SetCursorPosition(Console.WindowWidth - 12, 8);
            Console.Write("▌");
            Console.SetCursorPosition(Console.WindowWidth - 11, 8);
            Console.Write("▐");

            //Draw key guide

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.SetCursorPosition(Console.WindowWidth - 18, 11);
            Console.Write("Arrow Keys:     ");
            Console.SetCursorPosition(Console.WindowWidth - 18, 13);
            Console.Write("1 & 2:          ");
            Console.SetCursorPosition(Console.WindowWidth - 18, 15);
            Console.Write("3 & 4:          ");
            Console.SetCursorPosition(Console.WindowWidth - 18, 17);
            Console.Write("5 & 6:          ");
            Console.SetCursorPosition(Console.WindowWidth - 18, 19);
            Console.Write("r:              ");
            Console.SetCursorPosition(Console.WindowWidth - 18, 21);
            Console.Write("q:              ");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.SetCursorPosition(Console.WindowWidth - 16, 12);
            Console.Write("move cursor");
            Console.SetCursorPosition(Console.WindowWidth - 16, 14);
            Console.Write("inc/dec f. c.");
            Console.SetCursorPosition(Console.WindowWidth - 16, 16);
            Console.Write("inc/dec b. c.");
            Console.SetCursorPosition(Console.WindowWidth - 16, 18);
            Console.Write("inc/dec shading");
            Console.SetCursorPosition(Console.WindowWidth - 16, 20);
            Console.Write("refresh screen");
            Console.SetCursorPosition(Console.WindowWidth - 16, 22);
            Console.Write("quit w/o saving");

        }

        void UpdatePaintGUI(int currentForeColour, int currentBackColour, int currentShading)
        {
            Console.BackgroundColor = (ConsoleColor)currentBackColour;
            Console.ForegroundColor = (ConsoleColor)currentForeColour;

            Console.SetCursorPosition(Console.WindowWidth - 18, 8);
            Console.Write("░");
            Console.SetCursorPosition(Console.WindowWidth - 17, 8);
            Console.Write("▒");
            Console.SetCursorPosition(Console.WindowWidth - 16, 8);
            Console.Write("▓");
            Console.SetCursorPosition(Console.WindowWidth - 15, 8);
            Console.Write("█");
            Console.SetCursorPosition(Console.WindowWidth - 14, 8);
            Console.Write("▄");
            Console.SetCursorPosition(Console.WindowWidth - 13, 8);
            Console.Write("▀");
            Console.SetCursorPosition(Console.WindowWidth - 12, 8);
            Console.Write("▌");
            Console.SetCursorPosition(Console.WindowWidth - 11, 8);
            Console.Write("▐");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            Console.SetCursorPosition(Console.WindowWidth - 18, 3);
            Console.Write("                ");  //Erase previous marker
            Console.SetCursorPosition((Console.WindowWidth - 18) + currentForeColour, 3);
            Console.Write("^");
            Console.SetCursorPosition(Console.WindowWidth - 18, 6);
            Console.Write("                ");  //Erase previous marker
            Console.SetCursorPosition((Console.WindowWidth - 18) + currentBackColour, 6);
            Console.Write("^");
            Console.SetCursorPosition(Console.WindowWidth - 18, 9);
            Console.Write("        ");  //Erase previous marker
            Console.SetCursorPosition((Console.WindowWidth - 18) + (currentShading - 1), 9);
            Console.Write("^");
        }

        void DrawEntirePicture(int[,] fcbuffer, int[,] bcbuffer, int[,] sbuffer, int width, int height)
        {
            for (int y = 3; y < height + 3; y++) {
                Console.SetCursorPosition(0, y);
                for (int x = 0; x < width; x++) {
                    Console.ForegroundColor = (ConsoleColor)fcbuffer[x, y - 3];
                    Console.BackgroundColor = (ConsoleColor)bcbuffer[x, y - 3];

                    switch (sbuffer[x, y - 3]) {
                        case 1:
                            Console.Write("░");
                            break;
                        case 2:
                            Console.Write("▒");
                            break;
                        case 3:
                            Console.Write("▓");
                            break;
                        case 4:
                            Console.Write("█");
                            break;
                        case 5:
                            Console.Write("▄");
                            break;
                        case 6:
                            Console.Write("▀");
                            break;
                        case 7:
                            Console.Write("▌");
                            break;
                        case 8:
                            Console.Write("▐");
                            break;
                    }
                }
            }
        }

        void DrawSinglePixel(int fcolor, int bcolor, int shading, int x, int y)
        {
            Console.SetCursorPosition(x, y + 3);    //Account for title bar
            Console.ForegroundColor = (ConsoleColor)fcolor;
            Console.BackgroundColor = (ConsoleColor)bcolor;

            switch (shading) {
                case 1:
                    Console.Write("░");
                    break;
                case 2:
                    Console.Write("▒");
                    break;
                case 3:
                    Console.Write("▓");
                    break;
                case 4:
                    Console.Write("█");
                    break;
                case 5:
                    Console.Write("▄");
                    break;
                case 6:
                    Console.Write("▀");
                    break;
                case 7:
                    Console.Write("▌");
                    break;
                case 8:
                    Console.Write("▐");
                    break;
            }
        }
    }
}
