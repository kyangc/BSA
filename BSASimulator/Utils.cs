using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Xml.Serialization;

namespace BSASimulator
{
    internal class Utils
    {
        /// <summary>
        ///  Get biased value, the new value is made 0.8~1.2 times of origin value
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
        /// Get biased angle, the new angle is made +-20 degree of origin angel
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double GetBiasedAngle(double angle)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            double bias = (random.NextDouble() * 2 - 1) * 20;
            return angle + bias;
        }

        /// <summary>
        /// Get the base station position in the given index region
        /// </summary>
        /// <param name="indexX"></param>
        /// <param name="indexY"></param>
        /// <param name="regionLength"></param>
        /// <returns></returns>
        public static double[] GetBsPosition(int indexX, int indexY, double regionLength)
        {
            double[] position = new double[2];
            position[0] = (2 * indexX - 1) * regionLength / 2.0;
            position[1] = (2 * indexY - 1) * regionLength / 2.0;
            if (indexY % 2 == 0) position[0] = position[0] - regionLength / 2.0;
            return position;
        }

        /// <summary>
        /// Get the distance between the given 2 points
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
        /// Get the TA from the distance between device and base station
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static int GetTa(double distance)
        {
            return (int)Math.Ceiling(distance / 500.0);
        }

        /// <summary>
        /// Get the signal strength from the given base station
        /// </summary>
        /// <returns></returns>
        public static double GetSignalStrength(double x, double y, double distance, Option.SystemType systemType)
        {
            double frequence, pt, gr, gt, lc, lbf;
            switch (systemType)
            {
                case Option.SystemType.CDMA2000:
                    frequence = 800;
                    pt = 15;//基站发射功率
                    gr = 10;//接收天线增益
                    gt = 10;//发射天线增益
                    lc = 3;//电缆和榄头衰耗
                    lbf = 32.5 + 20 * Math.Log10(frequence) + 20 * Math.Log10(distance / 1000.0);
                    return pt + gr + gt - lc - lbf;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the AOA in degree (not radian)
        /// Attention here: the angle is the station's send off angle, an angle convert is needed if necessary.
        /// AND! If you need a 3D angle, please write oneself....
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="bsX"></param>
        /// <param name="bsY"></param>
        /// <returns></returns>
        public static double GetAoa(double x, double y, double bsX, double bsY)
        {
            double distance = GetDistanceBetween(x, y, bsX, bsY);
            if ((int)distance == 0) return 0;
            double cosTheta = (x - bsX) / distance;
            return Math.Acos(cosTheta) * 180 / Math.PI;
        }

        /// <summary>
        /// Get the error analysis list
        /// </summary>
        /// <param name="realPath"></param>
        /// <param name="allocateResultPath"></param>
        /// <returns></returns>
        public static List<double> GetErrorAnalysis(List<double[]> realPath, List<double[]> allocateResultPath)
        {
            var errorList = new List<double>();
            //TODO add method for analysing errors
            return errorList;
        }

        /// <summary>
        /// Output allocation result to .csv
        /// </summary>
        /// <param name="realPath"></param>
        /// <param name="allocatedPath"></param>
        /// <param name="errorList"></param>
        public static void OutputDatas(List<double[]> realPath, List<double[]> allocatedPath, List<double> errorList)
        {
            //TODO add method to output datas
        }
    }
}