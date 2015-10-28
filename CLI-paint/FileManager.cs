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
			string header = imageToSave.width + ":" + imageToSave.height + "\r\n";
			streamWriter.Write (header);
			for (int y = 0; y < imageToSave.height; y++) {
				for (int x = 0; x < imageToSave.width; x++) {
					//Write the forecolour
					streamWriter.Write (imageToSave.fcBuffer [x, y] + ":");
					//Write the backcolour
					streamWriter.Write (imageToSave.bcBuffer [x, y] + ":");
					//Write the shading
					streamWriter.Write (imageToSave.sBuffer [x, y] + "\r\n");
				}
			}
			streamWriter.Close ();
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
					imageToReturn.fcBuffer [x, y] = int.Parse (currentPixel [0]);
					imageToReturn.bcBuffer [x, y] = int.Parse (currentPixel [1]);
					imageToReturn.sBuffer [x, y] = int.Parse (currentPixel [2]);
				}
			}
			streamReader.Close ();
			return imageToReturn;
		}
	}
}
