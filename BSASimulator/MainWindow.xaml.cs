using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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

            ProgressBar.Minimum = 0;
            ProgressBar.Maximum = 1000;
        }

        private void OnclickStart(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            _options = Option.GetNewOption()
                .SetMainWindow(this)
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
                    _options.SetAlgorithmType((Option.AlgorithmType) i);
                }
            }

            for (int i = 0; i < _baseStationIntensityRadioButtons.Length; i++)
            {
                bool? isChecked = _baseStationIntensityRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    _options.SetBsIntensity((Option.BaseStationIntensity) i);
                }
            }

            for (int i = 0; i < _systemTypeRadioButtons.Length; i++)
            {
                bool? isChecked = _systemTypeRadioButtons[i].IsChecked;
                if (isChecked != null && isChecked.Value)
                {
                    _options.SetSystemType((Option.SystemType) i);
                }
            }

            _dataProvider = DataProvider.NewInstance()
                .PrepareData(_options);

            switch (_options.GetAlgorithmType())
            {
                case Option.AlgorithmType.Rss:
                    var invoke = new StartAlgorithm(() =>
                    {
                        List<double[]> res = Rss.NewIncetance()
                            .SetDataProvider(_dataProvider)
                            .SetOption(_options)
                            .StartAlgorithm()
                            .GetResultPath();
                        List<double> errorList = Utils.GetErrorList(
                            Option.SystemType.Wcdma,
                            _dataProvider.GetRealPathAll(),
                            res);
                        double avgError = Utils.GetErrorAnalysis(Utils.AnalysisType.Average, errorList);
                        Utils.OutputPaths(_dataProvider.GetRealPathAll(), res, _options);
                    });
                    invoke.BeginInvoke(Complete, invoke);
                    break;
                case Option.AlgorithmType.Ta:
                    invoke = () =>
                    {
                        List<double[]> res = Ta.NewIncetance()
                            .SetDataProvider(_dataProvider)
                            .SetOption(_options)
                            .StartAlgorithm()
                            .GetResultPath();
                        List<double> errorList = Utils.GetErrorList(
                            Option.SystemType.Wcdma,
                            _dataProvider.GetRealPathAll(),
                            res);
                        double avgError = Utils.GetErrorAnalysis(Utils.AnalysisType.Average, errorList);
                        Utils.OutputPaths(_dataProvider.GetRealPathAll(), res, _options);
                    };
                    invoke.BeginInvoke(Complete, invoke);
                    break;
                case Option.AlgorithmType.Tdoa:
                    invoke = () =>
                    {
                        List<double[]> res = Tdoa.NewIncetance()
                            .SetDataProvider(_dataProvider)
                            .SetOption(_options)
                            .StartAlgorithm()
                            .GetResultPath();
                        List<double> errorList = Utils.GetErrorList(
                            Option.SystemType.Wcdma,
                            _dataProvider.GetRealPathAll(),
                            res);
                        double avgError = Utils.GetErrorAnalysis(Utils.AnalysisType.Average, errorList);
                        Utils.OutputPaths(_dataProvider.GetRealPathAll(), res, _options);
                    };
                    invoke.BeginInvoke(Complete, invoke);
                    break;
            }
        }

        private void Complete(IAsyncResult ar)
        {
            if (ar == null) throw new ArgumentNullException("ar");

            var nr = ar.AsyncState as StartAlgorithm;
            Trace.Assert(nr != null, "Invalid object type");
            nr.EndInvoke(ar);

            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new StartAlgorithm(() =>
            {
                StartButton.IsEnabled = true;
                ProgressBar.Value = 0;
            }));
        }

        private delegate void StartAlgorithm();
    }
}