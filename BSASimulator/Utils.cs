using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BSASimulator
{
    /// <summary>
    ///     Author Chengkangyang @8/11/14
    /// </summary>
    internal class Utils
    {
        public enum AnalysisType
        {
            Average = 0
        }

        /// <summary>
        ///     Get biased value, the new value is made 0.8~1.2 times of origin value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetBiasedValue(double value)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            double bias = (random.NextDouble() - 0.5) * 0.4;
            return (bias + 1) * value;
        }

        /// <summary>
        ///     Get biased angle, the new angle is made a variable bias (-5 ~ +10)
        ///     degree of origin angel
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double GetBiasedAngle(double angle)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            double bias = (random.NextDouble() * 3 - 1) * 5;
            return angle + bias;
        }

        /// <summary>
        ///     Get the base station position in the given index region
        /// </summary>
        /// <param name="indexX"></param>
        /// <param name="indexY"></param>
        /// <param name="regionLength"></param>
        /// <returns></returns>
        public static double[] GetBsPosition(int indexX, int indexY, double regionLength)
        {
            var position = new double[2];
            position[0] = (2 * indexX - 1) * regionLength / 2.0;
            position[1] = (2 * indexY - 1) * regionLength / 2.0;
            if (indexY % 2 == 0) position[0] = position[0] - regionLength / 2.0;
            return position;
        }

        /// <summary>
        ///     Get the distance between the given 2 points
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double GetDistanceBetween(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x1 - x2), 2) + Math.Pow((y1 - y2), 2));
        }

        /// <summary>
        ///     Get the TA from the distance between device and base station
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="systemType"></param>
        /// <returns></returns>
        public static int GetTa(double distance, Option.SystemType systemType)
        {
            switch (systemType)
            {
                case Option.SystemType.Cdma2000:
                    return (int)Math.Ceiling(distance / 500.0);
                case Option.SystemType.Wcdma:
                    return (int)Math.Ceiling(distance / 500.0);
                default:
                    return 0;
            }
        }

        /// <summary>
        ///     Get the signal strength from the given base station
        /// </summary>
        /// <returns></returns>
        public static double GetSignalStrength(double x, double y, double distance, Option.SystemType systemType)
        {
            double frequence, pt, gr, gt, lc, lbf;
            switch (systemType)
            {
                case Option.SystemType.Cdma2000:
                    frequence = 800;
                    pt = 15; //基站发射功率
                    gr = 10; //接收天线增益
                    gt = 10; //发射天线增益
                    lc = 3; //电缆和榄头衰耗
                    lbf = 32.5 + 20 * Math.Log10(frequence) + 20 * Math.Log10(distance / 1000.0);
                    return pt + gr + gt - lc - lbf;
                case Option.SystemType.Wcdma:
                    frequence = 800;
                    pt = 15; //基站发射功率
                    gr = 10; //接收天线增益
                    gt = 10; //发射天线增益
                    lc = 3; //电缆和榄头衰耗
                    lbf = 32.5 + 20 * Math.Log10(frequence) + 20 * Math.Log10(distance / 1000.0);
                    return pt + gr + gt - lc - lbf;
                default:
                    return 0;
            }
        }

        /// <summary>
        ///     Get the AOA in degree (not radian)
        ///     Attention here: the angle is the station's send off angle, an angle convert is needed if necessary.
        ///     AND! If you need a 3D angle, please write oneself....
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bsX"></param>
        /// <param name="bsY"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static double GetAoa(double x, double y, double bsX, double bsY, double height)
        {
            double distance = Math.Sqrt(Math.Pow(GetDistanceBetween(x, y, bsX, bsY), 2) + Math.Pow(height, 2));
            if ((int)distance == 0) return 0;
            double cosTheta = (x - bsX) / distance;
            return Math.Acos(cosTheta) * 180 / Math.PI;
        }

        /// <summary>
        ///     Get the error analysis list
        /// </summary>
        /// <param name="systemType"></param>
        /// <param name="realPath"></param>
        /// <param name="allocateResultPath"></param>
        /// <returns></returns>
        public static List<double> GetErrorList(Option.SystemType systemType, List<double[]> realPath,
            List<double[]> allocateResultPath)
        {
            switch (systemType)
            {
                case Option.SystemType.Cdma2000:
                case Option.SystemType.Wcdma:
                    var errorList = new List<double>();
                    int count = realPath.Count > allocateResultPath.Count ? allocateResultPath.Count : realPath.Count;
                    for (int i = 0; i < count; i++)
                    {
                        double error =
                            Math.Sqrt(Math.Pow(realPath[i][0] - allocateResultPath[i][0], 2) +
                                      Math.Pow(realPath[i][1] - allocateResultPath[i][1], 2));
                        errorList.Add(error);
                    }
                    return errorList;
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Output allocation result to .csv
        /// </summary>
        /// <param name="realPath"></param>
        /// <param name="resultPath"></param>
        /// <param name="option"></param>
        public static void OutputPaths(List<double[]> realPath, List<double[]> resultPath, Option option)
        {
            FileInfo fi;
            StringBuilder sb;
            int filenum = 0;

            do
            {
                sb = new StringBuilder();
                sb.Append(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                sb.Append("\\real");
                sb.Append("_" + option.GetAlgorithmType());
                sb.Append("_" + option.GetSystemType());
                sb.Append("_" + option.GetBsIntencity());
                sb.Append("(" + filenum + ")");
                sb.Append(".csv");
                fi = new FileInfo(sb.ToString());
                filenum++;
            } while (fi.Exists);

            var sw = new StreamWriter(sb.ToString());
            for (int i = 0; i < realPath.Count; i++)
            {
                var line = new StringBuilder();
                line.Append(realPath[i][0]);
                line.Append(',');
                line.Append(realPath[i][1]);
                sw.WriteLine(line.ToString());
                sw.Flush();
            }
            sw.Close();

            filenum = 0;
            do
            {
                sb = new StringBuilder();
                sb.Append(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
                sb.Append("\\result");
                sb.Append("_" + option.GetAlgorithmType());
                sb.Append("_" + option.GetSystemType());
                sb.Append("_" + option.GetBsIntencity());
                sb.Append("(" + filenum + ")");
                sb.Append(".csv");
                fi = new FileInfo(sb.ToString());
                filenum++;
            } while (fi.Exists);

            sw = new StreamWriter(sb.ToString());
            for (int i = 0; i < resultPath.Count; i++)
            {
                var line = new StringBuilder();
                line.Append(resultPath[i][0]);
                line.Append(',');
                line.Append(resultPath[i][1]);
                sw.WriteLine(line.ToString());
                sw.Flush();
            }
            sw.Close();
        }

        /// <summary>
        ///     Calculate the Euclidean distance
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <returns></returns>
        public static double GetEucDisatance(double[] data1, double[] data2)
        {
            if (data1.Length <= 0 || data2.Length <= 0 || data1.Length != data2.Length)
            {
                throw new Exception("无法计算欧式距离");
            }
            double sum = data1.Select((t, i) => Math.Pow(t - data2[i], 2)).Sum();
            return Math.Sqrt(sum);
        }

        /// <summary>
        ///     Return the error analysis of the given analysis type
        /// </summary>
        /// <param name="analysisType"></param>
        /// <param name="errorList"></param>
        /// <returns></returns>
        public static double GetErrorAnalysis(AnalysisType analysisType, List<double> errorList)
        {
            switch (analysisType)
            {
                case AnalysisType.Average:
                    if (errorList.Count == 0) return -1;
                    double sum = errorList.Sum();
                    return sum / errorList.Count;
                default:
                    return -1;
            }
        }

        /// <summary>
        ///     Show load file dialog
        /// </summary>
        /// <param name="title">dialog title</param>
        /// <returns></returns>
        public static string ReadFileDialog(string title)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "CSV文件|*.csv",
                Title = title,
            };
            if (ofd.ShowDialog() != DialogResult.OK || ofd.FileName.Length == 0) return null;
            return ofd.FileName;
        }

        /// <summary>
        ///    Get avgInterval based on velocity
        /// </summary>
        /// <param name="avgInterval"></param>
        /// <param name="currentVelocity"></param>
        /// <returns></returns>
        public static double GetAvgIntervalBasedOnVelocity(double avgInterval, double currentVelocity)
        {
            double lowestSpeedLosePackageRatio = 0;
            double HighestSpeedLosePackageRatio = 0.8;
            double lowSpeed = 50;
            double HighSpeed = 250;
            double currentRatio;
            if (currentVelocity <= lowSpeed)
            {
                currentRatio = lowestSpeedLosePackageRatio;
            }
            else if (currentVelocity >= HighSpeed)
            {
                currentRatio = HighestSpeedLosePackageRatio;
            }
            else
            {
                currentRatio = (HighestSpeedLosePackageRatio - lowestSpeedLosePackageRatio)
                                  * (currentVelocity - lowSpeed) /
                                  (HighSpeed - lowSpeed) + lowestSpeedLosePackageRatio;
            }
            Random random = new Random();
            double currentInterval = avgInterval;
            while (random.NextDouble() < currentRatio)
            {
                currentInterval += GetBiasedValue(avgInterval);
            }
            return currentInterval;
        }
    }
}