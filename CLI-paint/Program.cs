/*  This is a program written to allow easy editing of CLI pictures, saved in the .aspic format
 *  (AScii PICture). This format saves images as a 2D array of pixels. Each pixel contains three 
 *  values: the foreground colour (1 to 16), the background colour (1 to 16) and an integer value 
 *  between 1 and 8 inclusive indicating the blending format, as seen below:
 *  
 *  1   -   ░   -   Very light foreground colour, predominantly background colour
 *  2   -   ▒   -   50/50 mixture of foreground and background colours
 *  3   -   ▓   -   Very light background colour, predominantly foreground colour
 *  4   -   █   -   Solid block of foreground colour
 *  5   -   ▄   -   Bottom half foreground, top half background
 *  6   -   ▀   -   Bottom half background, top half foreground
 *  7   -   ▌   -   Left half foreground, right half background
 *  8   -   ▐   -   Left half background, right half foreground
 *  
 * 
 *  Each file also contains a header containing the width and height. Pixels, and the header, are
 *  separated by semicolons while individual values within each pixel and the header are separated
 *  by colons.
 */
using System;

namespace CLI_paint
{
    class Program
    {
        static void Main(string[] args)
        {
            PaintProgram program = new PaintProgram();
            program.Run();
        }
    }
}
