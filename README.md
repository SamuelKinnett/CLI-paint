# CLI-paint
This program is an MSPaint-esque image editor with a command line interface. It can be used to create and edit .aspic files,
a simple file format for which I'll soon create a small parsing library. This program will (hopefully) be useful to anyone 
creating graphics for CLI programs, such as roguelikes, and was written specifically to make image creation for my own roguelike
easier. 

The .aspic (AScii PICture) file format saves images as a 2D array of pixels. Each pixel contains three values: the foreground 
colour (1 to 16), the background colour (1 to 16) and an integer value between 1 and 8 inclusive indicating the blending format, 
as seen below:

1   -   ░   -   Very light foreground colour, predominantly background colour

2   -   ▒   -   50/50 mixture of foreground and background colours

3   -   ▓   -   Very light background colour, predominantly foreground colour

4   -   █   -   Solid block of foreground colour

5   -   ▄   -   Bottom half foreground, top half background

6   -   ▀   -   Bottom half background, top half foreground

7   -   ▌   -   Left half foreground, right half background

8   -   ▐   -   Left half background, right half foreground


Each file also contains a header containing the width and height. Pixels, and the header, are separated by semicolons while 
individual values within each pixel and the header are separated by colons. In this way, images can be created, saved and read
relatively easily. 
