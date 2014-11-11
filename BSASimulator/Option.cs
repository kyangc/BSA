using System;

namespace BSASimulator
{
    /// <summary>
    ///     Author Chengkangyang @8/11/14
    /// </summary>
    internal class Option
    {
        public enum BaseStationIntensity
        {
            Default = 0,
            Less = 1,
            Least = 2
        }

        public enum AlgorithmType
        {
            Ta = 0,
            Tdoa = 1,
            TaAoa = 2,
            Rss = 3
        }

        public enum SystemType
        {
            Cdma2000 = 0,
            Wcdma = 1
        }

        private double _avgVelocity;
        private double _height;
        private double _initDirection;
        private double _mapHeight;
        private double _mapWidth;
        private double _receiveRadius;
        private double _startX, _startY;
        private MainWindow _mainWindow;
        private SystemType _systemType;
        private BaseStationIntensity _baseStationIntensity;
        private AlgorithmType _algorithmType;

        public static Option GetNewOption()
        {
            return new Option();
        }

        public Option SetBsIntensity(BaseStationIntensity baseStationIntensity)
        {
            _baseStationIntensity = baseStationIntensity;
            return this;
        }

        public double GetBsIntencity()
        {
            if (_baseStationIntensity == null)
            {
                throw new Exception("没有初始化基站密度参数");
            }
            switch (_baseStationIntensity)
            {
                case BaseStationIntensity.Default:
                    return 500;
                case BaseStationIntensity.Less:
                    return 1000;
                case BaseStationIntensity.Least:
                    return 3000;
                default:
                    return 500;
            }
        }

        public double[] GetIntervalPattern()
        {
            if (_systemType == null)
            {
                throw new Exception("没有初始化通信系统");
            }
            switch (_systemType)
            {
                case SystemType.Cdma2000:
                    return new double[] {2};
                case SystemType.Wcdma:
                    return new double[] {2};
                    //TODO more system support is on the way
                default:
                    return new double[] {2};
            }
        }

        public double GetTaDistance()
        {
            if (_systemType == null)
            {
                throw new Exception("没有初始化通信系统");
            }
            switch (_systemType)
            {
                case SystemType.Cdma2000:
                    return 500;
                case SystemType.Wcdma:
                    return 500;
                    //TODO more system support is on the way
                default:
                    return 500;
            }
        }

        public Option SetAlgorithmType(AlgorithmType algorithmType)
        {
            _algorithmType = algorithmType;
            return this;
        }

        public AlgorithmType GetAlgorithmType()
        {
            if (_algorithmType == null)
            {
                throw new Exception("没有初始化仿真模式");
            }
            return _algorithmType;
        }

        public Option SetStartPosition(double x, double y)
        {
            _startX = x;
            _startY = y;
            return this;
        }

        public double[] GetStartPosition()
        {
            if (_startX == null || _startY == null)
            {
                throw new Exception("没有初始化起始位置");
            }
            return new double[2] {_startX, _startY};
        }

        public Option SetAvgVelocity(double velocity)
        {
            _avgVelocity = velocity;
            return this;
        }

        public double GetAvgVelocity()
        {
            if (_avgVelocity == null)
            {
                throw new Exception("没有初始化速度");
            }
            return _avgVelocity;
        }

        public Option SetMapProportion(double width, double height)
        {
            _mapWidth = width;
            _mapHeight = height;
            return this;
        }

        public double[] GetMapProportion()
        {
            if (_mapHeight == null || _mapWidth == null)
            {
                throw new Exception("没有初始化地图大小");
            }
            return new double[2] {_mapWidth, _mapHeight};
        }

        public Option SetInitDirection(double initDirection)
        {
            _initDirection = initDirection;
            return this;
        }

        public double GetInitDirection()
        {
            if (_initDirection == null)
            {
                throw new Exception("没有初始化起始方向");
            }
            return _initDirection;
        }

        public Option SetReceiveRadius(double receiveRadius)
        {
            _receiveRadius = receiveRadius;
            return this;
        }

        public double GetReceiveRadius()
        {
            if (_receiveRadius == null)
            {
                throw new Exception("没有初始化接收信号半径");
            }
            return _receiveRadius;
        }

        public Option SetSystemType(SystemType systemType)
        {
            _systemType = systemType;
            return this;
        }

        public SystemType GetSystemType()
        {
            if (_systemType == null)
            {
                throw new Exception("没有初始化系统类型");
            }
            return _systemType;
        }

        public Option SetHeight(double height)
        {
            _height = height;
            return this;
        }

        public double GetHeight()
        {
            if (_height == null)
            {
                throw new Exception("没有初始化飞行高度");
            }
            return _height;
        }

        public Option SetMainWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            return this;
        }

        public MainWindow GetMainWindow()
        {
            if (_mainWindow == null)
            {
                throw new Exception("没有初始化工作窗体");
            }
            return _mainWindow;
        }
    }
}