# Tower Unite MIDI .NET

## What is Tower Unite MIDI .NET?
This program was created due to a lack of options for using a MIDI piano with Tower Unite. Native support has been in the talks for years, but so far there hasn't been any notable progress. Tower Unite MIDI .NET will convert any MIDI devices input into something Tower Unite can read. Due to popular request during development, playback of MIDI files has also been implemented.

Tower Unite MIDI .NET is written in C# and heavily utilises the [NAudio](https://github.com/naudio/NAudio) and [DryWetMidi](https://github.com/melanchall/drywetmidi) libraries.

## Okay, how do I use it? 
Using Tower Unite MIDI .NET is simple!

1. Obtain the latest release from the "Releases" page.
2. Extract the archive into it's own folder.
3. Run the program.
4. You'll notice 2 tabs in the program, **MIDI Device Setup** and **MIDI Playback**. **MIDI Device Setup** is your go-to tab for setting up and using your MIDI devices, where as **MIDI Playback** is the tab you'll need for playing back MIDI files.
5. **MIDI Device Setup**

   5A. To setup your MIDI device, please select it from the 'Input Devices' dropdown list. If it isn't showing up, please press the 'Scan for devices' button.
   
   5B. Once you have your MIDI device selected, press 'Start Listening' to start recieving input from your device. Alternatively, you can 
open your Tower Unite window and hit the **F1** key.
   
   5C. To stop recieving input from your MIDI device, press the 'Stop Listening' button, or alternatively press the **F2** key.
   
   5D. By default, the note's are transposed so that middle C (C4) on your device aligns with Tower Unite. If you're unhappy with this transposition, and your device doesn't natively support transposing, you can use the 'Octave Transposition' slider to customise it to your liking.

6. **MIDI Playback**

   6A. First, browse for your file using the 'Browse' button.
   
   6B. If you wish to transpose or modify the playback speed of your file, you can do so using the provided sliders. Note that you cannot modify this while the file is playing.
   
   6C. Open your Tower Unite window and press the **F1** key to begin file playback.
   
   6D. If you wish to stop playing the file early, hit the **F2** key.
   
7. **Options**

   7A. Ping Input
      
      On laggier connections, you'll notice some dud black keys. This is because Tower Unite doesn't have enough time to register the fact that the program has hit the shift key. To combat this, the program has a miniscule delay (15ms) built in to key presses, but sometimes that's not enough. If you're having problems, using this to input your Tower Unite ping and the program will do its best to assign a more appropriate key delay.
      
   7B. Detailed Logging
   
      Enables the logging of events such as MIDI key presses.
      
## Doesn't a program like this already exist?
Yes, it was written in python by a steam user called Mattio. This was great for a while, but my reason for creating this alternative is primarily because Mattio's version had some limitations.
1. It hasn't been updated in 2 years.
2. It required devices to specifically use MIDI channel 1.
3. It very rarely had good black key presses.
4. It didn't offer MIDI playback.
My program not only solves these limitations, but offers far more customisation.

## Isn't automation software against the rules?
Macroing is a touchy subject in Tower Unite. It's still a pretty extensive issue in terms of automation in the Casino. 
[As stated here](https://forums.pixeltailgames.com/t/regarding-piano-macros/14111/15) by the lead developer of Tower Unite, piano macro's are okay, and I've personally never experienced an issue during testing. Even though this software doesn't inject anything and only sends keypresses, I'd still take care in condo's as condo's are VAC protected, and VAC doesn't discriminate. Please always make sure to take care in starting and stopping the software only while you're on the piano, and as always, this software is used at your own risk.

## Known Issues
Sometimes, you may be required to spam the "stop" key or button when playing a MIDI, though I've rarely noticed this and cannot replicate it.

## Reporting an issue
Please either raise an issue here on Github, or [email me.](mailto:xyoshify@gmail.com)

## License
MIT License

Copyright (c) 2019 Bailey Eaton

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
