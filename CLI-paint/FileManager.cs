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
using System.IO;
using System.Collections.Generic;

namespace CLI_paint
{
	public class FileManager
	{
		StreamReader streamReader;
		StreamWriter streamWriter;

		string cwd;
		//The current working directory

		public FileManager ()
		{
			cwd = Directory.GetCurrentDirectory ();
		}

		/// <summary>
		/// Given an image, attempts to save said image in the current working directory. On
		/// a successful save, the function will return 0 and on a failure it will return 1.
		/// </summary>
		public int SaveImage (Image imageToSave)
		{
			streamWriter = new StreamWriter (imageToSave.name + ".aspic", false);
			string header = imageToSave.name + ":" + imageToSave.width + ":" + imageToSave.height + "\r\n";
			streamWriter.Write (header);
			for (int y = 0; y < imageToSave.height; y++) {
				for (int x = 0; x < imageToSave.width; x++) {
                    int[] cPixel = imageToSave.data[x, y].GetPixel();
					//Write the forecolour
					streamWriter.Write (cPixel[0] + ":");
					//Write the backcolour
                    streamWriter.Write(cPixel[1] + ":");
					//Write the shading
                    streamWriter.Write(cPixel[2] + "\r\n");
				}
			}
			streamWriter.Close ();
			return 0;
		}

        /// <summary>
        /// Saves an image using lossless compression
        /// </summary>
        /// <param name="imageToSave"></param>
        /// <returns></returns>
        public int SaveImageCompressed(Image imageToSave)
        {
            streamWriter = new StreamWriter(imageToSave.name + ".aspic", false);
            string header = imageToSave.name + ":" + imageToSave.width + ":" + imageToSave.height + "\r\n";
            streamWriter.Write(header);

            //Now we need to look throught the image and create a "palette" of all the different pixel types present.
            List<Image.Pixel> pixelTypes = new List<Image.Pixel>();
            pixelTypes.Add(imageToSave.data[0, 0]);

            for (int y = 0; y < imageToSave.height; y++) {
                for (int x = 0; x < imageToSave.width; x++) {
                    //Iterate through each pixel currently saved in the palette
                    bool newPixel = true;
                    foreach (Image.Pixel currentPixel in pixelTypes) {
                        if (currentPixel == imageToSave.data[x, y])
                        {
                            newPixel = false;
                            break;
                        }
                    }
                    if (newPixel) {
                        pixelTypes.Add(imageToSave.data[x, y]);
                    }
                }
            }

            //Next, write this data just after the header.

            foreach (Image.Pixel currentPixel in pixelTypes) {
                streamWriter.Write(currentPixel.fc + "," + currentPixel.bc + "," + currentPixel.s + ":");
            }
            streamWriter.Write("\r\n");

            //Now, look throught the image, finding adjacent blocks of pixels. Each adjacent block is saved as the index in the palette followed by the number that occur.
            Image.Pixel lastPixel = new Image.Pixel(-1, -1, -1);   //The pixel we are currently comparing to
            int pixelCount = 0; //The number of that pixel occurring
            int pixelIndex = 0;     //The index in the list of the pixel
            for (int y = 0; y < imageToSave.height; y++) {
                for (int x = 0; x < imageToSave.width; x++) {
                    if (imageToSave.data[x, y] == lastPixel) {
                        //We need to simply increment the pixelCount variable
                        pixelCount++;
                    } else {
                        //If we need to, write the data for the last block of pixels
                        if (pixelCount > 0) {
                            streamWriter.Write(pixelIndex + ":" + pixelCount + "\r\n");
                        }
                        pixelCount = 0;
                        pixelIndex = -1;
                        foreach (Image.Pixel cPixel in pixelTypes) {
                            pixelIndex++;
                            if (cPixel == imageToSave.data[x, y]) {
                                lastPixel = cPixel;
                                break;
                            }
                        }
                    }
                }
            }

            streamWriter.Close();
            return 0;
        }

		public Image LoadImage (string imageName)
		{
			try {
				streamReader = new StreamReader (imageName + ".aspic"); 
			} catch {
				//the provided image name must not exist
				Console.WriteLine ("Error: Image not found");
				Console.ReadLine ();
				return new Image ("Error", 1, 1);
			}

			string[] headerData = streamReader.ReadLine ().Split (':'); //reads the header data and splits it into an array
			Image imageToReturn = new Image (headerData [0], int.Parse (headerData [1]), int.Parse (headerData [2]));

			//Now, read in all of the image data
			string[] currentPixel;
			for (int y = 0; y < imageToReturn.height; y++) {
				for (int x = 0; x < imageToReturn.width; x++) {
					currentPixel = streamReader.ReadLine ().Split (':');
                    imageToReturn.data[x, y].SetPixel(int.Parse (currentPixel [0]), int.Parse (currentPixel [1]), int.Parse (currentPixel [2]));
				}
			}
			streamReader.Close ();
			return imageToReturn;
		}

        public Image LoadImageCompressed (string imageName)
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
            Image.Pixel[] palette = new Image.Pixel[paletteSize];

            for (int count = 0; count < paletteSize - 1; count++) {
                string[] pixelData = paletteRawData[count].Split(',');
                palette[count] = new Image.Pixel(int.Parse(pixelData[0]), int.Parse(pixelData[1]), int.Parse(pixelData[2]));
            }

            //With the palette created, recreate the image from the image data

            int xPointer = -1;   //Where in the 2D array our pointer is located
            int yPointer = 0;   //

            while (yPointer < imageToReturn.height) {
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
                    imageToReturn.data[xPointer, yPointer] = palette[paletteIndex];
                }
            }
            streamReader.Close();

            return imageToReturn;
        }
	}
}
