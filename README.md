# Visualizing Matrix Decomposition

This a `.NET Core` application that generates animations. It applies LU and QR decompositions to linear transformations, and shows the effects of the factors independently. See example result:

[![Watch the video](doc/tumbler.jpg)](https://www.youtube.com/watch?v=AIOEtorHctc)

And this is an example for the QR decomposition: https://www.youtube.com/watch?v=CWEhxN0hemE

To convert the images to videos, use FFMPEG:

    "E:\ffmpeg\ffmpeg.exe" -start_number 0 -loop 1 -t 120 -i "E:\path\to\source\frame_%4d.png" -c:v libx264 -b 1800K -vf "fps=30,format=yuv420p" E:\path\to\dest\LU_composite_RT.mp4
