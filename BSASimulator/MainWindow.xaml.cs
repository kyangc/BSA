using System.Windows;
using System.Windows.Controls;

namespace BSASimulator
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RadioButton[] baseStationIntensityRadioButtons;
        private readonly RadioButton[] simulateTypeRadioButtons;
        private DataProvider dataProvider;
        private Option options;

        public MainWindow()
        {
            InitializeComponent();
            simulateTypeRadioButtons = new RadioButton[4]
            {
                SimulationTypeTaRadioButton,
                SimulationTypeTdoaRadioButton,
                SimulationTypeTaaoaRadioButton,
                SimulationTypeRssRadioButton
            };

            baseStationIntensityRadioButtons = new RadioButton[3]
            {
                SimulationBsIntensityMostTaRadioButton,
                SimulationBsIntensityLessTaRadioButton,
                SimulationBsIntensityLeastTaRadioButton
            };
        }

        private void OnclickStart(object sender, RoutedEventArgs e)
        {
            options = Option.GetCustomedOption()
                .SetAvgVelocity(150)
                .SetMapProportion(50000, 50000)
                .SetAvgFetchDataInterval(2)
                .SetStartPosition(0, 25000)
                .SetInitDirection(0)
                .SetReceiveRadius(3000)
                .SetSystemType(Option.SystemType.CDMA2000);

            for (int i = 0; i < simulateTypeRadioButtons.Length; i++)
            {
                bool? isChecked = simulateTypeRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    options.SetSimulationType((Option.SimulationType) i);
                }
            }

            for (int i = 0; i < baseStationIntensityRadioButtons.Length; i++)
            {
                bool? isChecked = baseStationIntensityRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    options.SetBsIntensity((Option.BaseStationIntensity) i);
                }
            }

            dataProvider = DataProvider.NewInstance()
                .PrepareData(options);
        }
    }
}