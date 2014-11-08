using System.Windows;
using System.Windows.Controls;

namespace BSASimulator
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RadioButton[] _baseStationIntensityRadioButtons;
        private readonly RadioButton[] _simulateTypeRadioButtons;
        private readonly RadioButton[] _systemTypeRadioButtons;
        private DataProvider _dataProvider;
        private Option _options;

        public MainWindow()
        {
            InitializeComponent();
            _simulateTypeRadioButtons = new[]
            {
                SimulationTypeTaRadioButton,
                SimulationTypeTdoaRadioButton,
                SimulationTypeTaaoaRadioButton,
                SimulationTypeRssRadioButton
            };

            _baseStationIntensityRadioButtons = new[]
            {
                SimulationBsIntensityMostTaRadioButton,
                SimulationBsIntensityLessTaRadioButton,
                SimulationBsIntensityLeastTaRadioButton
            };

            _systemTypeRadioButtons = new[]
            {
                SystemTypeCdmaRadioButton,
                SystemTypeWcdmaRadioButton
            };
        }

        private void OnclickStart(object sender, RoutedEventArgs e)
        {
            _options = Option.GetCustomedOption()
                .SetAvgVelocity(150)
                .SetMapProportion(500000, 500000)
                .SetStartPosition(0, 250000)
                .SetInitDirection(0)
                .SetReceiveRadius(5000)
                .SetHeight(5000);

            for (int i = 0; i < _simulateTypeRadioButtons.Length; i++)
            {
                bool? isChecked = _simulateTypeRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    _options.SetSimulationType((Option.SimulationType)i);
                }
            }

            for (int i = 0; i < _baseStationIntensityRadioButtons.Length; i++)
            {
                bool? isChecked = _baseStationIntensityRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    _options.SetBsIntensity((Option.BaseStationIntensity)i);
                }
            }

            for (int i = 0; i < _systemTypeRadioButtons.Length; i++)
            {
                bool? isChecked = _systemTypeRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    _options.SetSystemType((Option.SystemType)i);
                }
            }

            _dataProvider = DataProvider.NewInstance()
                .PrepareData(_options);
        }
    }
}