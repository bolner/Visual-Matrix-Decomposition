# Visualizing Matrix Decomposition

This a `.NET Core` application that generates animations. It applies LU and QR decompositions to linear transformations, and shows the effects of the factors independently. See example result:

[![Watch the video](doc/tumbler.jpg)](https://www.youtube.com/watch?v=Mc89G9_kPwk)

To convert the images to videos, use FFMPEG:

    "E:\ffmpeg\ffmpeg.exe" -start_number 0 -loop 1 -t 120 -i "E:\path\to\source\frame_%4d.png" -c:v libx264 -b 800K -vf "fps=30,format=yuv420p" E:\path\to\dest\LU_composite_RT.mp4
