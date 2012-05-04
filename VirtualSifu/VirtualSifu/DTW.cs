using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace VirtualSifu
{
    class DTW
    {
        private double EuclideanDistance(JointData masterJoint, JointData studentJoint)
        {
            return Math.Sqrt(Math.Pow((masterJoint.xData - studentJoint.xData), 2) + Math.Pow((masterJoint.yData - studentJoint.yData), 2)
                                + Math.Pow((masterJoint.zData - studentJoint.zData), 2));
        }

        public double DTWDistance(ArrayList masterList, ArrayList studentList)
        {
            int size = masterList.Count;
            var DTWMatrix = new double[size + 1, size + 1];

            // Initialize DTW matrix
            DTWMatrix[0, 0] = 0;
            for (int j = 1; j <= size; j++)
            {
                DTWMatrix[j, 0] = double.PositiveInfinity;
            }
            for (int i = 1; i <= size; i++)
            {
                DTWMatrix[0, i] = double.PositiveInfinity;
            }
            // End of Init

            for (int i = 1; i <= size; i++)
            {
                for (int j = 1; j <= size; j++)
                {
                    double cost = EuclideanDistance((JointData)masterList[i - 1], (JointData)studentList[j - 1]);
                    DTWMatrix[i, j] = cost + Math.Min(DTWMatrix[i - 1, j], Math.Min(DTWMatrix[i, j - 1], DTWMatrix[i - 1, j - 1]));
                }
            }
            return DTWMatrix[size, size];
        }
    }
}