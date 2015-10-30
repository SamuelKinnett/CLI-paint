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
using System.Collections.Generic;
using System.IO;

namespace AspicLibrary
{
    /// <summary>
    /// A class for storing ASPIC image data and methods
    /// </summary>
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

        public Image(string name, int width, int height)
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

    /// <summary>
    /// A class providing methods to save and load ASPIC files
    /// </summary>
    public class FileManager
    {
        StreamReader streamReader;
        StreamWriter streamWriter;

        string cwd;
        //The current working directory

        public FileManager()
        {
            cwd = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Given an image, attempts to save said image in the current working directory. On
        /// a successful save, the function will return 0 and on a failure it will return 1.
        /// </summary>
        public int SaveImage(Image imageToSave)
        {
            streamWriter = new StreamWriter(imageToSave.name + ".aspic", false);
            string header = imageToSave.name + ":" + imageToSave.width + ":" + imageToSave.height + "\r\n";
            streamWriter.Write(header);
            for (int y = 0; y < imageToSave.height; y++) {
                for (int x = 0; x < imageToSave.width; x++) {
                    int[] cPixel = imageToSave.data[x, y].GetPixel();
                    //Write the forecolour
                    streamWriter.Write(cPixel[0] + ":");
                    //Write the backcolour
                    streamWriter.Write(cPixel[1] + ":");
                    //Write the shading
                    streamWriter.Write(cPixel[2] + "\r\n");
                }
            }
            streamWriter.Close();
            return 0;
        }

        /// <summary>
        /// Saves an image using lossless compression
        /// </summary>
        public int SaveImageCompressed(Image imageToSave)
        {
            streamWriter = new StreamWriter(imageToSave.name + ".aspic", false);
            string header = imageToSave.name + ":" + imageToSave.width + ":" + imageToSave.height + "\r\n";
            streamWriter.Write(header);

            //Now we need to look throught the image and create a "palette" of all the different pixel types present.
            List<Image.Pixel> pixelTypes = new List<Image.Pixel>();
            int[,] paletteMap = new int[imageToSave.width, imageToSave.height]; //Used to store the palette value of each pixel
            pixelTypes.Add(imageToSave.data[0, 0]);

            for (int y = 0; y < imageToSave.height; y++) {
                for (int x = 0; x < imageToSave.width; x++) {
                    //Iterate through each pixel currently saved in the palette
                    bool newPixel = true;
                    int currentPixelValue = -1;

                    foreach (Image.Pixel currentPixel in pixelTypes) {
                        currentPixelValue++;
                        if (currentPixel == imageToSave.data[x, y]) {
                            newPixel = false;
                            break;
                        }
                    }
                    if (newPixel) {
                        currentPixelValue++;
                        pixelTypes.Add(imageToSave.data[x, y]);
                    }
                    paletteMap[x, y] = currentPixelValue;
                }
            }

            //Next, write this data just after the header.

            foreach (Image.Pixel currentPixel in pixelTypes) {
                streamWriter.Write(currentPixel.fc + "," + currentPixel.bc + "," + currentPixel.s + ":");
            }
            streamWriter.Write("\r\n");

            //Now, look throught the image, finding adjacent blocks of pixels. Each adjacent block is saved as the index in the palette followed by the number that occur.
            int pixelCount = 0;     //The number of that pixel occurring
            int pixelIndex = -1;     //The palette index of the last pixel
            for (int y = 0; y < imageToSave.height; y++) {
                for (int x = 0; x < imageToSave.width; x++) {
                    if (paletteMap[x, y] == pixelIndex) {
                        //We need to simply increment the pixelCount variable
                        pixelCount++;
                    }
                    else {
                        //If we need to, write the data for the last block of pixels
                        if (pixelCount > 0) {
                            streamWriter.Write(pixelIndex + ":" + pixelCount + "\r\n");
                        }
                        pixelCount = 1; //Since we've just counted the new pixel
                        pixelIndex = paletteMap[x, y];  //The new 
                    }
                }
            }

            //Make sure to check if the last block has been written
            if (pixelCount > 0) {
                streamWriter.Write(pixelIndex + ":" + pixelCount + "\r\n");
            }

            //Close the StreamWriter, we're done here!
            streamWriter.Close();
            return 0;
        }

        public Image LoadImage(string imageName)
        {
            try {
                streamReader = new StreamReader(imageName + ".aspic");
            }
            catch {
                //the provided image name must not exist
                Console.WriteLine("Error: Image not found");
                Console.ReadLine();
                return new Image("Error", 1, 1);
            }

            string[] headerData = streamReader.ReadLine().Split(':'); //reads the header data and splits it into an array
            Image imageToReturn = new Image(headerData[0], int.Parse(headerData[1]), int.Parse(headerData[2]));

            //Now, read in all of the image data
            string[] currentPixel;
            for (int y = 0; y < imageToReturn.height; y++) {
                for (int x = 0; x < imageToReturn.width; x++) {
                    currentPixel = streamReader.ReadLine().Split(':');
                    imageToReturn.data[x, y].SetPixel(int.Parse(currentPixel[0]), int.Parse(currentPixel[1]), int.Parse(currentPixel[2]));
                }
            }
            streamReader.Close();
            return imageToReturn;
        }

        public Image LoadImageCompressed(string imageName)
        {
            try {
                streamReader = new StreamReader(imageName + ".aspic");
            }
            catch {
                //the provided image name must not exist
                Console.WriteLine("Error: Image not found");
                Console.ReadLine();
                return new Image("Error", 1, 1);
            }

            string[] headerData = streamReader.ReadLine().Split(':'); //reads the header data and splits it into an array
            Image imageToReturn = new Image(headerData[0], int.Parse(headerData[1]), int.Parse(headerData[2]));

            //Now, read in the palette data

            string[] paletteRawData = streamReader.ReadLine().Split(':');
            //Split the string up and convert it into an array of Image.Pixel types
            int paletteSize = paletteRawData.Length;
            Image.Pixel[] palette = new Image.Pixel[paletteSize - 1];

            for (int count = 0; count < paletteSize - 1; count++) {
                string[] pixelData = paletteRawData[count].Split(',');
                palette[count] = new Image.Pixel(int.Parse(pixelData[0]), int.Parse(pixelData[1]), int.Parse(pixelData[2]));
            }

            //With the palette created, recreate the image from the image data

            int xPointer = -1;   //Where in the 2D array our pointer is located
            int yPointer = 0;   //

            while (yPointer < imageToReturn.height - 1 && xPointer < imageToReturn.width - 1) {
                string[] rawData = streamReader.ReadLine().Split(':');
                int paletteIndex = int.Parse(rawData[0]);
                int pixelCount = int.Parse(rawData[1]);

                for (int count = 0; count < pixelCount; count++) {
                    if (xPointer < imageToReturn.width - 1)
                        xPointer++;
                    else {
                        xPointer = 0;
                        yPointer++;
                    }
                    imageToReturn.data[xPointer, yPointer].SetPixel(palette[paletteIndex].fc, palette[paletteIndex].bc, palette[paletteIndex].s);
                }
            }
            streamReader.Close();

            return imageToReturn;
        }
    }

    /// <summary>
    /// A class providing methods to draw ASPIC files to the console
    /// </summary>
    public class Rendering
    {
        /// <summary>
        /// Draws the specified area of the original image
        /// </summary>
        public void DrawSubImage(Image imageToRender, int imagePositionX, int imagePositionY, int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {
            for (int y = 0; y < viewportHeight; y++) {
                Console.SetCursorPosition(0, imagePositionY + y);
                for (int x = 0; x < viewportWidth; x++) {
                    //Get the current pixel's information
                    int[] cPixel = imageToRender.data[x + viewportX, y + viewportY].GetPixel();

                    Console.ForegroundColor = (ConsoleColor)cPixel[0];
                    Console.BackgroundColor = (ConsoleColor)cPixel[1];

                    switch (cPixel[2]) {
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

        public void DrawSinglePixel(Image.Pixel pixelToDraw, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = (ConsoleColor)pixelToDraw.fc;
            Console.BackgroundColor = (ConsoleColor)pixelToDraw.bc;

            switch (pixelToDraw.s) {
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
