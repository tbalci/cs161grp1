using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace VirtualSifu
{
    class StreamFileReader
    {
        ArrayList headJ = new ArrayList();
        ArrayList shoulderCenterJ = new ArrayList();
        ArrayList shoulderRightJ = new ArrayList();
        ArrayList elbowRightJ = new ArrayList();
        ArrayList wristRightJ = new ArrayList();
        ArrayList handRightJ = new ArrayList();
        ArrayList shoulderLeftJ = new ArrayList();
        ArrayList elbowLeftJ = new ArrayList();
        ArrayList wristLeftJ = new ArrayList();
        ArrayList handLeftJ = new ArrayList();
        ArrayList spineJ = new ArrayList();
        ArrayList hipCenterJ = new ArrayList();
        ArrayList hipRightJ = new ArrayList();
        ArrayList kneeRightJ = new ArrayList();
        ArrayList ankleRightJ = new ArrayList();
        ArrayList footRightJ = new ArrayList();
        ArrayList hipLeftJ = new ArrayList();
        ArrayList kneeLeftJ = new ArrayList();
        ArrayList ankleLeftJ = new ArrayList();
        ArrayList footLeftJ = new ArrayList();

        String line;
        String fileName;

        public StreamFileReader(String fName)
        {
            fileName = fName;
            parseFile();
        }

        private void parseFile()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                String[] data = line.Split(' ');
                switch (data[0])
                {
                    case "Head:":
                        headJ.Add(parseJoint(data));
                        break;
                    case "ShoulderCenter:":
                        shoulderCenterJ.Add(parseJoint(data));
                        break;
                    case "ShoulderRight:":
                        shoulderRightJ.Add(parseJoint(data));
                        break;
                    case "ElbowRight:":
                        elbowRightJ.Add(parseJoint(data));
                        break;
                    case "WristRight:":
                        wristRightJ.Add(parseJoint(data));
                        break;
                    case "ShoulderLeft:":
                        shoulderLeftJ.Add(parseJoint(data));
                        break;
                    case "ElbowLeft:":
                        elbowLeftJ.Add(parseJoint(data));
                        break;
                    case "WristLeft:":
                        wristLeftJ.Add(parseJoint(data));
                        break;
                    case "Spine:":
                        spineJ.Add(parseJoint(data));
                        break;
                    case "HipCenter:":
                        hipCenterJ.Add(parseJoint(data));
                        break;
                    case "KneeRight:":
                        kneeRightJ.Add(parseJoint(data));
                        break;
                    case "AnkleRight:":
                        ankleRightJ.Add(parseJoint(data));
                        break;
                    case "FootRight:":
                        footRightJ.Add(parseJoint(data));
                        break;
                    case "KneeLeft:":
                        kneeLeftJ.Add(parseJoint(data));
                        break;
                    case "AnkleLeft:":
                        ankleLeftJ.Add(parseJoint(data));
                        break;
                    case "FootLeft:":
                        footLeftJ.Add(parseJoint(data));
                        break;
                }
            }
        }

        private JointData parseJoint(String[] lineData)
        {
            double X = Convert.ToDouble(lineData[1]);
            double Y = Convert.ToDouble(lineData[2]);
            double Z = Convert.ToDouble(lineData[3]);

            return new JointData(X, Y, Z);
        }

        public ArrayList getJointArray(String select)
        {
            switch (select)
            {
                case "Head":
                    return headJ;
                case "ShoulderCenter":
                    return shoulderCenterJ;
                case "ShoulderRight":
                    return shoulderRightJ;
                case "ElbowRight":
                    return elbowRightJ;
                case "WristRight":
                    return wristRightJ;
                case "ShoulderLeft":
                    return shoulderLeftJ;
                case "ElbowLeft":
                    return elbowLeftJ;
                case "WristLeft":
                    return wristLeftJ;
                case "Spine":
                    return spineJ;
                case "HipCenter":
                    return hipCenterJ;
                case "KneeRight":
                    return kneeRightJ;
                case "AnkleRight":
                    return ankleRightJ;
                case "FootRight":
                    return footRightJ;
                case "KneeLeft":
                    return kneeLeftJ;
                case "AnkleLeft":
                    return ankleLeftJ;
                case "FootLeft":
                    return footLeftJ;
            }
            return null;

        }
    }
}