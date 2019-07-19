using System;
using System.Collections.Generic;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;



// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace TurnOnLight
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private const int LedGreenPin = 2;
        private const int LedYellowPin = 3;
        private GpioPin _pinGreen;
        private GpioPin _pinYellow;

        public MainPage()
        {
            InitializeComponent();
            InitGpio();
        }

        private void InitGpio()
        {
            GpioController gpio = GpioController.GetDefault();

            _pinGreen = gpio.OpenPin(LedGreenPin);
            _pinYellow = gpio.OpenPin(LedYellowPin);
            _pinGreen.SetDriveMode(GpioPinDriveMode.Output);
            _pinYellow.SetDriveMode(GpioPinDriveMode.Output);
        }

        private IEnumerable<GpioPin> SelectedPin(Entity entity)
        {
            List<GpioPin> result = new List<GpioPin>();
            switch (entity)
            {
                case Entity.Green:
                    LightAccess(result, entity, _pinGreen);
                    break;
                case Entity.Yellow:
                    LightAccess(result, entity, _pinYellow);
                    break;
                case Entity.AllLights:
                    LightAccess(result, Entity.Green, _pinGreen);
                    LightAccess(result, Entity.Yellow, _pinYellow);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(entity), entity, null);
            }

            return result.ToArray();
        }

        private void LightAccess(IList<GpioPin> pinList, Entity light, GpioPin pin)
        {
            pinList.Add(pin);
        }

        private async void GO_Click(object sender, RoutedEventArgs e)
        {

            Analyzer result = await WebClientLuis.Order(CommandTextBox.Text);

            IEnumerable<GpioPin> pins = SelectedPin(result.Entity);
            foreach (GpioPin pin in pins)
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
