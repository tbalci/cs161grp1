using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileReader
{
    class JointData
    {
        public double xData { get; set; }
        public double yData { get; set; }
        public double zData { get; set; }

        public JointData(double x, double y, double z)
        {
            xData = x;
            yData = y;
            zData = z;
        }
        override
        public String ToString()
        {
            String xResult = Convert.ToString(xData);
            String yResult = Convert.ToString(yData);
            String zResult = Convert.ToString(zData);
            String result = String.Concat(xResult, ", ", yResult, ", ", zResult);
            return result;
        }
    }
}