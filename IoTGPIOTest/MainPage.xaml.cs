using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio; //Need this using statement for the GPIO
using System.Threading;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Core;
using Windows.ApplicationModel;
using System.Net.Http;
using System.Diagnostics;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IoTGPIOTest
{
    public sealed partial class MainPage : Page
    {

        private DispatcherTimer policeTimer;
        private DispatcherTimer xmasTimer;

        //Initialize SpeechRecognizer Object
        #region
        private const string SRGS_FILE = "CortanaCommands.xml";
        private SpeechRecognizer recognizer;
        private const string TAG_TARGET = "target";
        // Tag CMD
        private const string TAG_CMD = "cmd";
        // Tag Device
        private const string TAG_DEVICE = "device";
        // On State
        private const string STATE_ON = "ON";
        // Off State
        private const string STATE_OFF = "OFF";
        // LED Device
        private const string DEVICE_LED = "LED";
        // Light Device
        private const string DEVICE_LIGHT = "LIGHT";
        // Red Led
        private const string COLOR_RED = "RED";
        // Green Led
        private const string COLOR_GREEN = "GREEN";
        // Bedroom
        private const string TARGET_BEDROOM = "BEDROOM";
        // Porch
        private const string TARGET_PORCH = "PORCH";
        #endregion


        //Set up the GPIO pin numbers
        private const int yLED_PIN = 16;
        private const int bLED_PIN = 12;
        private const int rLED_PIN = 25;
        private const int gLED_PIN = 24;
        private const int wLED_PIN = 23;
        private const int rgb_RED = 5;
        private const int rgb_BLUE = 6;
        private const int rgb_GREEN = 13;

        //Assign variable to GpioPins
        private GpioPin yPin;
        private GpioPin bPin;
        private GpioPin rPin;
        private GpioPin gPin;
        private GpioPin wPin;
        private GpioPin rgb_RedPin;
        private GpioPin rgb_BluePin;
        private GpioPin rgb_GreenPin;


        //Setting up the colors for the ellipse
        private SolidColorBrush yellowBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private SolidColorBrush dYellowBrush = new SolidColorBrush(Windows.UI.Colors.LightYellow);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
        private SolidColorBrush dBlueBrush = new SolidColorBrush(Windows.UI.Colors.LightBlue);
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush dRedBrush = new SolidColorBrush(Windows.UI.Colors.LightSalmon);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush dGreenBrush = new SolidColorBrush(Windows.UI.Colors.LightGreen);
        private SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);
        private SolidColorBrush dWhiteBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);


        private GpioPinValue pinValue;
        private GpioPinValue rgbPinValue;

        public MainPage()
        {
            InitGPIO();
            this.InitializeComponent();

            //Fill the ellipses with the default colors
            yellowIMG.Fill = dYellowBrush;
            blueIMG.Fill = dBlueBrush;
            redIMG.Fill = dRedBrush;
            greenIMG.Fill = dGreenBrush;
            whiteIMG.Fill = dWhiteBrush;
            rgbIMG.Fill = whiteBrush;

        }

        private void InitGPIO()
        {
            //Gets the default GPIO if there is only one
            GpioController gpio = GpioController.GetDefault();


            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                yPin = null;
                bPin = null;
                rPin = null;
                gPin = null;
                wPin = null;
                rgb_RedPin = null;
                rgb_BluePin = null;
                rgb_GreenPin = null;

                return;
            }

            //Open the GPIO pins
            wPin = gpio.OpenPin(wLED_PIN);
            yPin = gpio.OpenPin(yLED_PIN);
            bPin = gpio.OpenPin(bLED_PIN);
            rPin = gpio.OpenPin(rLED_PIN);
            gPin = gpio.OpenPin(gLED_PIN);
            rgb_RedPin = gpio.OpenPin(rgb_RED);
            rgb_BluePin = gpio.OpenPin(rgb_BLUE);
            rgb_GreenPin = gpio.OpenPin(rgb_GREEN);


            //Set the pin value to High which turns off the GPIO pin
            pinValue = GpioPinValue.High;
            rgbPinValue = GpioPinValue.Low;

            //Write the pin value (High) to the GPIO pin
            yPin.Write(pinValue);
            bPin.Write(pinValue);
            rPin.Write(pinValue);
            gPin.Write(pinValue);
            wPin.Write(pinValue);
            rgb_RedPin.Write(rgbPinValue);
            rgb_BluePin.Write(rgbPinValue);
            rgb_GreenPin.Write(rgbPinValue);

            //Set the GPIO pins to Output
            yPin.SetDriveMode(GpioPinDriveMode.Output);
            bPin.SetDriveMode(GpioPinDriveMode.Output);
            rPin.SetDriveMode(GpioPinDriveMode.Output);
            gPin.SetDriveMode(GpioPinDriveMode.Output);
            wPin.SetDriveMode(GpioPinDriveMode.Output);
            rgb_RedPin.SetDriveMode(GpioPinDriveMode.Output);
            rgb_BluePin.SetDriveMode(GpioPinDriveMode.Output);
            rgb_GreenPin.SetDriveMode(GpioPinDriveMode.Output);

        }
        #region
        private void yellowToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (yellowToggleSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.Low;
                yPin.Write(pinValue);
                yellowIMG.Fill = yellowBrush;
            }
            else if (yellowToggleSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.High;
                yPin.Write(pinValue);
                yellowIMG.Fill = dYellowBrush;
            }
        }

        private void blueToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (blueToggleSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.Low;
                bPin.Write(pinValue);
                blueIMG.Fill = blueBrush;
            }
            else if (blueToggleSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.High;
                bPin.Write(pinValue);
                blueIMG.Fill = dBlueBrush;
            }

        }

        private void redToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (redToggleSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.Low;
                rPin.Write(pinValue);
                redIMG.Fill = redBrush;
            }
            else if (redToggleSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.High;
                rPin.Write(pinValue);
                redIMG.Fill = dRedBrush;
            }

        }

        private void greenToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (greenToggleSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.Low;
                gPin.Write(pinValue);
                greenIMG.Fill = greenBrush;
            }
            else if (greenToggleSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.High;
                gPin.Write(pinValue);
                greenIMG.Fill = dGreenBrush;
            }

        }

        private void whiteToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (whiteToggleSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.Low;
                wPin.Write(pinValue);
                whiteIMG.Fill = whiteBrush;
            }
            else if (whiteToggleSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.High;
                wPin.Write(pinValue);
                whiteIMG.Fill = dWhiteBrush;
            }

        }
        private void rgb_RedSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (rgb_RedSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.High;
                rgb_RedPin.Write(pinValue);
                rgbIMG.Fill = redBrush;
            }
            else if (rgb_RedSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.Low;
                rgb_RedPin.Write(pinValue);
                rgbIMG.Fill = whiteBrush;
            }

        }

        private void rgb_BlueSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (rgb_BlueSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.High;
                rgb_BluePin.Write(pinValue);
                rgbIMG.Fill = blueBrush;

            }
            else if (rgb_BlueSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.Low;
                rgb_BluePin.Write(pinValue);
                rgbIMG.Fill = whiteBrush;
            }

        }

        private void rgb_GreenSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (rgb_GreenSwitch.IsOn == true)
            {
                pinValue = GpioPinValue.High;
                rgb_GreenPin.Write(pinValue);
                rgbIMG.Fill = greenBrush;
            }
            else if (rgb_GreenSwitch.IsOn == false)
            {
                pinValue = GpioPinValue.Low;
                rgb_GreenPin.Write(pinValue);
                rgbIMG.Fill = whiteBrush;
            }

        }
        private void policeSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (policeSwitch.IsOn == true)
            {
                xmasSwitch.IsOn = false;
                xmasSwitch.IsEnabled = false;
                rgb_RedSwitch.IsOn = false;
                rgb_RedSwitch.IsEnabled = false;
                rgb_BlueSwitch.IsOn = false;
                rgb_BlueSwitch.IsEnabled = false;
                rgb_GreenSwitch.IsOn = false;
                rgb_GreenSwitch.IsEnabled = false;

                policeTimer = new DispatcherTimer();
                policeTimer.Interval = TimeSpan.FromMilliseconds(250);
                policeTimer.Tick += policeTimerTick;

                if (policeSwitch.IsOn == true)
                {
                    policeTimer.Start();
                }
            }
            else if (policeSwitch.IsOn == false)
            {
                xmasSwitch.IsEnabled = true;
                rgb_RedSwitch.IsEnabled = true;
                rgb_BlueSwitch.IsEnabled = true;
                rgb_GreenSwitch.IsEnabled = true;
                rgb_RedPin.Write(GpioPinValue.Low);
                rgb_BluePin.Write(GpioPinValue.Low);
                policeTimer.Stop();
                rgbIMG.Fill = whiteBrush;
            }
        }

        private void policeTimerTick(object sender, object e)
        {
            if (rgbPinValue == GpioPinValue.Low)
            {
                rgbPinValue = GpioPinValue.High;
                rgb_RedPin.Write(rgbPinValue);
                rgbIMG.Fill = redBrush;
                rgb_BluePin.Write(GpioPinValue.Low);
            }
            else
            {
                rgbPinValue = GpioPinValue.Low;
                rgb_RedPin.Write(rgbPinValue);
                rgb_BluePin.Write(GpioPinValue.High);
                rgbIMG.Fill = blueBrush;
            }
        }
        private void xmasSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (xmasSwitch.IsOn == true && policeSwitch.IsOn == false)
            {
                policeSwitch.IsOn = false;
                policeSwitch.IsEnabled = false;
                rgb_RedSwitch.IsOn = false;
                rgb_RedSwitch.IsEnabled = false;
                rgb_BlueSwitch.IsOn = false;
                rgb_BlueSwitch.IsEnabled = false;
                rgb_GreenSwitch.IsOn = false;
                rgb_GreenSwitch.IsEnabled = false;

                xmasTimer = new DispatcherTimer();
                xmasTimer.Interval = TimeSpan.FromMilliseconds(250);
                xmasTimer.Tick += xmasTimerTick;

                if (xmasSwitch.IsOn == true)
                {
                    xmasTimer.Start();
                }
            }
            else if (xmasSwitch.IsOn == false)
            {
                rgb_RedPin.Write(GpioPinValue.Low);
                rgb_GreenPin.Write(GpioPinValue.Low);
                xmasTimer.Stop();
                rgbIMG.Fill = whiteBrush;
                policeSwitch.IsEnabled = true;
                rgb_RedSwitch.IsEnabled = true;
                rgb_BlueSwitch.IsEnabled = true;
                rgb_GreenSwitch.IsEnabled = true;
            }
        }
        private void xmasTimerTick(object sender, object e)
        {
            if (rgbPinValue == GpioPinValue.Low)
            {
                rgbPinValue = GpioPinValue.High;
                rgb_RedPin.Write(rgbPinValue);
                rgbIMG.Fill = redBrush;
                rgb_GreenPin.Write(GpioPinValue.Low);
            }
            else
            {
                rgbPinValue = GpioPinValue.Low;
                rgb_RedPin.Write(rgbPinValue);
                rgb_GreenPin.Write(GpioPinValue.High);
                rgbIMG.Fill = greenBrush;
            }
        }

        #endregion

        //This turns ALL light on or off.
        private void allLights_Toggled(object sender, RoutedEventArgs e)
        {
            pinValue = GpioPinValue.Low;
            GpioPinValue pinValueOff = GpioPinValue.High;


            if (allLights.IsOn == true)
            {
                yellowToggleSwitch.IsOn = true;
                blueToggleSwitch.IsOn = true;
                redToggleSwitch.IsOn = true;
                greenToggleSwitch.IsOn = true;
                whiteToggleSwitch.IsOn = true;

                yPin.Write(pinValue);
                yellowIMG.Fill = yellowBrush;
                bPin.Write(pinValue);
                blueIMG.Fill = blueBrush;
                rPin.Write(pinValue);
                redIMG.Fill = redBrush;
                gPin.Write(pinValue);
                greenIMG.Fill = greenBrush;
                wPin.Write(pinValue);
                whiteIMG.Fill = whiteBrush;
            }
            else
            {
                yellowToggleSwitch.IsOn = false;
                blueToggleSwitch.IsOn = false;
                redToggleSwitch.IsOn = false;
                greenToggleSwitch.IsOn = false;
                whiteToggleSwitch.IsOn = false;

                yPin.Write(pinValueOff);
                yellowIMG.Fill = dYellowBrush;
                bPin.Write(pinValueOff);
                blueIMG.Fill = dBlueBrush;
                rPin.Write(pinValueOff);
                redIMG.Fill = dRedBrush;
                gPin.Write(pinValueOff);
                greenIMG.Fill = dGreenBrush;
                wPin.Write(pinValueOff);
                whiteIMG.Fill = dWhiteBrush;
            }
        }

    }
}
