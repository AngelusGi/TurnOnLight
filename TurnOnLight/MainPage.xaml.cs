using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using TurnOnLight.Analyzer;
using Windows.Devices.Gpio;



// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace TurnOnLight
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private const int LED_GREEN_PIN = 2;
        private const int LED_YELLOW_PIN = 3;
        private GpioPin pinGreen;
        private GpioPin pinYellow;

        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            pinGreen = gpio.OpenPin(LED_GREEN_PIN);
            pinYellow = gpio.OpenPin(LED_YELLOW_PIN);
            pinGreen.SetDriveMode(GpioPinDriveMode.Output);
            pinYellow.SetDriveMode(GpioPinDriveMode.Output);
        }

        private GpioPin[] SelectedPin(Entity entity)
        {
            var result = new List<GpioPin>();
            switch (entity)
            {
                case Entity.Green:
                    LightAccess(result, entity, pinGreen);
                    break;
                case Entity.Yellow:
                    LightAccess(result, entity, pinYellow);
                    break;
                case Entity.AllLights:
                    LightAccess(result, Entity.Green, pinGreen);
                    LightAccess(result, Entity.Yellow, pinYellow);
                    break;
                default:
                    break;
            }

            return result.ToArray();
        }

        private void LightAccess(IList<GpioPin> pinList, Entity light, GpioPin pin)
        {
            pinList.Add(pin);
        }

        private async void GO_Click(object sender, RoutedEventArgs e)
        {
           
            var result = await WebClientLuis.Order(CommandTextBox.Text);

            var pins = SelectedPin(result.Entity);
            foreach (var pin in pins)
            {
                switch (result.Intent)
                {
                    case Intent.LightOn:
                        pin.Write(GpioPinValue.Low);
                        break;
                    case Intent.LightOff:
                        pin.Write(GpioPinValue.High);
                        break;
                    case Intent.None:
                    default:
                        break;
                }
            }

            CommandTextBox.Text = "";
            CommandTextBox.Focus(FocusState.Keyboard);
        }

        private void SolutionName_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
