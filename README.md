# LED's w/ RGB
####This is a beginner project written in C# for those that are new to the Raspberry Pi 2 Model B.  

#####This project uses:

1. Raspberry Pi 2 Model B
2. Breadboard
3. LED's
4. RGB LED (optional)


This project will allow you to control 5 LED's with On/Off ToggleSwitches.  This will also allow you to control an RGB LED but that is optinal.  This project also briefly touches on `dispatchTimer` which we use when we creat our Christmas light effect and also the red and blue lights for our Police light effect.

##Hardware Setup

![Hardware Setup](http://i63.tinypic.com/11gqf11.jpg)


##Code

I used Visual Studio (C#) Universal Windows App to create this project.

Add the following using statement

```cs
using Windows.Devices.Gpio; //Need this using statement for the GPIO
```

We will be using th efollowing pins

```cs
        private const int yLED_PIN = 16;
        private const int bLED_PIN = 12;
        private const int rLED_PIN = 25;
        private const int gLED_PIN = 24;
        private const int wLED_PIN = 23;
        private const int rgb_RED = 5;
        private const int rgb_BLUE = 6;
        private const int rgb_GREEN = 13;
```

Email me with questions: duanekord@gmail.com
