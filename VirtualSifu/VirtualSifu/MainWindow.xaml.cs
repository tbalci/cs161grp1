﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Kinect;
using System.Drawing;

namespace VirtualSifu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool recordSet = false;
        ArrayList recordBuffer = new ArrayList();
        StreamWriter writer;
        FileStream dataStream;
        int frameNumber = 0;
        int playbackFrameNumber = 0;
        bool playback = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];

        String[] jointsTracked = { "AnkleRight", "AnkleLeft", "KneeRight", "KneeLeft", "HipRight", "HipLeft", "ShoulderRight", "ShoulderLeft", "ElbowRight", "ElbowLeft", "WristRight", "WristLeft" };

//<<<<<<< .mine
       //StreamFileReader masterData;
//=======
        //DTW Based Variables
        //UNCOMMENT THE STREAMFILEREADERS for USAGE.
        //StreamFileReader is kept Global because we only want to parse it once.
        //It returns any joint array we ask it for.
        StreamFileReader masterData; //= new StreamFileReader("C:\\Users\\Shadow\\Desktop\\motiondata.txt"); //NOTE THIS PART NEEDS TO BE RE-DIRECTED AFTER MOTION DATA IS SAVED
        //StreamFileReader studentData; //= new StreamFileReader("C:\\Users\\Shadow\\Desktop\\motiondata.txt"); //NOTE THIS SHOULD BE RE-DIRECTED AFTER MOTION DATA IS SAVED
        DTW dtw = new DTW();
        double threshold = 1.0; //dummy value
        int startFrame = 0;
// .r33
       ProfileData studentData;
        


        JointData[] studentLeftWristData = new JointData[30];
        JointData[] studentRightWristData = new JointData[30];
        JointData[] studentLeftElbowData = new JointData[30];
        JointData[] studentRightElbowData = new JointData[30];
        JointData[] studentLeftShoulderData = new JointData[30];
        JointData[] studentRightShoulderData = new JointData[30];
        JointData[] studentLeftAnkleData = new JointData[30];
        JointData[] studentRightAnkleData = new JointData[30];
        JointData[] studentLeftKneeData = new JointData[30];
        JointData[] studentRightKneeData = new JointData[30];
        JointData[] studentLeftHipData = new JointData[30];
        JointData[] studentRightHipData = new JointData[30];

        ArrayList masterLeftWristData = new ArrayList();
        ArrayList masterRightWristData = new ArrayList();
        ArrayList masterLeftElbowData = new ArrayList();
        ArrayList masterRightElbowData = new ArrayList();
        ArrayList masterLeftShoulderData = new ArrayList();
        ArrayList masterRightShoulderData = new ArrayList();
        ArrayList masterLeftAnkleData = new ArrayList();
        ArrayList masterRightAnkleData = new ArrayList();
        ArrayList masterLeftKneeData = new ArrayList();
        ArrayList masterRightKneeData = new ArrayList();
        ArrayList masterLeftHipData = new ArrayList();
        ArrayList masterRightHipData = new ArrayList();
       



        public MainWindow()
        {
            studentData = new ProfileData(jointsTracked);
            InitializeComponent();
        }

        //method to run DTW and return an [array] containing the DTW data.
        //This has been adapted from Gina's code
        //@param joint corresponds to the joint that you desire (see JointData.cs for full list of usable joints)
       /* private ArrayList runDTW(String joint)
        {
            ArrayList masterList = masterData.getJointArray(joint); //Note: you better have defined masterData to something before running this part.
            //ArrayList studentList = studentData.getJointArray(joint); // NOte: you better have defined studentData to something before running this part.
            ArrayList jData = new ArrayList();
            int numFrames = 6;
            int start = 0;
           // int size = studentList.Count;
            double val = 0;

            while (start < size)
            {
                if (start + numFrames >= size)
                {   
                    numFrames = size - start;
                }
                val = dtw.DTWDistance(masterList.GetRange(start, numFrames), studentList.GetRange(start, numFrames));
                jData.Add(val);
                start += numFrames;
            }
            return jData;
        } */

        //Second iteration of runDTW.
        //this one will instead return an array containing DTW information for each joint (this is to save memory and lines of code later)
        //startFrame is global [i planned to make it static, but can't be bothered right now] so that we can keep track of which frame we're at
        //haven't tried this one out yet, so i dont know if it'll work. It's just a concept idea for now
        private ArrayList runDTW2()
        {
            int numFrames = 6;
            string[] jointsList = {"Head", "ShoulderLeft", "ShoulderRight", "ElbowLeft", "ElbowRight", "WristLeft", "WristRight", "KneeLeft", "KneeRight",
                                    "AnkleLeft", "AnkleRight"};
            ArrayList jData = new ArrayList();
            foreach (String joint in jointsList)
            {
                ArrayList masterList = masterData.getJointArray(joint);
                ArrayList studentList = masterData.getJointArray(joint);
                int size = studentList.Count;
                if (startFrame + numFrames > size)
                {
                    numFrames = size - startFrame;
                }
                double val = dtw.DTWDistance(masterList.GetRange(startFrame, numFrames), studentList.GetRange(startFrame, numFrames));
                jData.Add(val);
            }
            startFrame += numFrames;
            return jData;
        }

        private OrderedDictionary getJointDict()
        {
            OrderedDictionary jointDict = new OrderedDictionary();
            foreach (String joint in jointsTracked)
                jointDict.Add(joint, new ArrayList());
            return jointDict;
        }

        //helper method
        private Joint getJoint(String joint, Skeleton skeleton)
        {
            if (joint.Equals("AnkleRight")) return skeleton.Joints[JointType.AnkleRight];
            else if (joint.Equals("AnkleLeft")) return skeleton.Joints[JointType.AnkleLeft];
            else if (joint.Equals("KneeRight")) return skeleton.Joints[JointType.KneeRight];
            else if (joint.Equals("KneeLeft")) return skeleton.Joints[JointType.KneeLeft];
            else if (joint.Equals("HipRight")) return skeleton.Joints[JointType.HipRight];
            else if (joint.Equals("HipLeft")) return skeleton.Joints[JointType.HipLeft];
            else if (joint.Equals("ShoulderRight")) return skeleton.Joints[JointType.ShoulderRight];
            else if (joint.Equals("ShoulderLeft")) return skeleton.Joints[JointType.ShoulderLeft];
            else if (joint.Equals("ElbowRight")) return skeleton.Joints[JointType.ElbowRight];
            else if (joint.Equals("ElbowLeft")) return skeleton.Joints[JointType.ElbowLeft];
            else if (joint.Equals("WristRight")) return skeleton.Joints[JointType.WristRight];
            else if (joint.Equals("WristLeft")) return skeleton.Joints[JointType.WristLeft];
            else return skeleton.Joints[JointType.Head];

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor oldSensor = (KinectSensor)e.OldValue;
            StopKinect(oldSensor);

            KinectSensor newSensor = (KinectSensor)e.NewValue;

            newSensor.ColorStream.Enable();
            newSensor.DepthStream.Enable();
            newSensor.SkeletonStream.Enable();
            newSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            try
            {
                newSensor.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();

            }

              
        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {


            if (playback == true)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame == null)
                    {
                        return;
                    }
                    playbackFrameNumber++;
                    byte[] pixels = new byte[colorFrame.PixelDataLength];
                    dataStream.Read(pixels, 0, colorFrame.PixelDataLength);
                    int stride = colorFrame.Width * 4;
                    masterView.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);




                    using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                    {
                        if (skeletonFrame != null)
                        {
                            Skeleton[] data = new Skeleton[skeletonFrame.SkeletonArrayLength];
                            skeletonFrame.CopySkeletonDataTo(data);

                            foreach (Skeleton skeleton in data)
                            {

                                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                                {

                                    SkeletonPoint studentPoint = skeleton.Joints[JointType.WristRight].Position;
                                    foreach (String joint in jointsTracked)
                                    {
                                        ((ArrayList)studentData.Get(joint)).Insert(playbackFrameNumber % 30, new JointData(1, 2, 3));
                                       //((ArrayList) studentData.Get(joint))[playbackFrameNumber % 30] = new JointData(1,2,3);
                                    }
                                    /*studentRightWristData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.WristLeft].Position;
                                    studentLeftWristData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.ElbowRight].Position;
                                    studentRightElbowData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.ElbowLeft].Position;
                                    studentLeftElbowData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.ShoulderRight].Position;
                                    studentRightShoulderData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.ShoulderLeft].Position;
                                    studentLeftShoulderData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.AnkleRight].Position;
                                    studentRightAnkleData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.AnkleLeft].Position;
                                    studentLeftAnkleData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.KneeRight].Position;
                                    studentRightKneeData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.KneeLeft].Position;
                                    studentLeftKneeData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.HipRight].Position;
                                    studentRightHipData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z);
                                    studentPoint = skeleton.Joints[JointType.HipLeft].Position;
                                    studentLeftHipData[playbackFrameNumber % 30] = new JointData(studentPoint.X, studentPoint.Y, studentPoint.Z); */

                                   

                                    if (1 == 1)
                                    {
                                        //something might go here
                                        if (playbackFrameNumber % 30 == 0)
                                        {
                                            //run DTW for each joint
//<<<<<<< .mine


//=======
                                            //each runDTW will return an arraylist of doubles that contain all the necessary information
//>>>>>>> .r33
                                            Random random = new Random();

                                            foreach (Object obj in MainCanvas.Children)
                                            {
                                                Ellipse ellipse = obj as Ellipse;
                                                if (ellipse != null)
                                                    colorJoint(ellipse, random.Next(0, 4));

                                            }
                                                //colorJoint(ellipse, random.Next(0, 4));
                                            //Probably can do this part like Gina's
                                            //Get a joint list that you want calculated
                                            //perform runDTW on each individual joint
                                            //then do your coloring/drawing for it.


                                        }
                                    }

                                    ScalePosition(wristRight, skeleton.Joints[JointType.WristLeft]);
                                    ScalePosition(wristLeft, skeleton.Joints[JointType.WristLeft]);
                                    ScalePosition(elbowRight, skeleton.Joints[JointType.ElbowRight]);
                                    ScalePosition(elbowLeft, skeleton.Joints[JointType.ElbowLeft]);
                                    ScalePosition(shoulderRight, skeleton.Joints[JointType.ShoulderRight]);
                                    ScalePosition(shoulderLeft, skeleton.Joints[JointType.ShoulderLeft]);
                                    ScalePosition(ankleRight, skeleton.Joints[JointType.AnkleRight]);
                                    ScalePosition(ankleLeft, skeleton.Joints[JointType.AnkleLeft]);
                                    ScalePosition(kneeRight, skeleton.Joints[JointType.KneeRight]);
                                    ScalePosition(kneeLeft, skeleton.Joints[JointType.KneeLeft]);
                                    ScalePosition(hipRight, skeleton.Joints[JointType.HipRight]);
                                    ScalePosition(hipLeft, skeleton.Joints[JointType.HipLeft]);
                                    GetCameraPoint(skeleton, e); 
                                }
                            }
                        }

                    }


                }
            }

            if (recordSet == true)
            {
                using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
                {
                    if (colorFrame == null)
                    {
                        return;
                    }
                    frameNumber++;
                    byte[] pixels = new byte[colorFrame.PixelDataLength];
                    colorFrame.CopyPixelDataTo(pixels);

                    dataStream.Write(pixels, 0, colorFrame.PixelDataLength);
                    
                    int stride = colorFrame.Width * 4;
                    masterView.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                    

                }
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    if (skeletonFrame != null)
                    {
                        Console.Write(masterLeftWristData.Count);
                        Skeleton[] data = new Skeleton[skeletonFrame.SkeletonArrayLength];
                        skeletonFrame.CopySkeletonDataTo(data);

                        foreach (Skeleton skeleton in data)
                        {

                            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                            {

                                SkeletonPoint point = skeleton.Joints[JointType.Head].Position;
                                writer.Write("Head: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.ShoulderCenter].Position;



                                writer.Write("ShoulderCenter: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.ShoulderRight].Position;
                                writer.Write("ShoulderRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.ElbowRight].Position;
                                writer.Write("ElbowRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.WristRight].Position;
                                writer.Write("WristRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.HandRight].Position;
                                writer.Write("HandRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.ShoulderLeft].Position;
                                writer.Write("ShoulderLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.ElbowLeft].Position;
                                writer.Write("ElbowLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.WristLeft].Position;
                                writer.Write("WristLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.HandLeft].Position;
                                writer.Write("HandLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.Spine].Position;
                                writer.Write("Spine: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.HipCenter].Position;
                                writer.Write("HipCenter: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.HipRight].Position;
                                writer.Write("HipRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.KneeRight].Position;
                                writer.Write("KneeRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.AnkleRight].Position;
                                writer.Write("AnkleRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.FootRight].Position;
                                writer.Write("FootRight: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.HipLeft].Position;
                                writer.Write("HipLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.KneeLeft].Position;
                                writer.Write("KneeLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.AnkleLeft].Position;
                                writer.Write("AnkleLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                point = skeleton.Joints[JointType.FootLeft].Position;
                                writer.Write("FootLeft: " + point.X + " " + point.X + " " + point.Y + "\r\n");
                                writer.Write("\r\n");

                                //Somewhere after all this code has run, we need to finish construction of 
                                //our FileStreamReader for Student [and master?]
                            }
                        }
                    } 
                }
            }
        }

        void colorJoint(Ellipse ellipse, int accuracy)
        {
            if (accuracy == 0)
                ellipse.Fill = new SolidColorBrush(Colors.Red);
            else if (accuracy == 1)
                ellipse.Fill = new SolidColorBrush(Colors.Yellow);
            else
                ellipse.Fill = new SolidColorBrush(Colors.Green);

        }
        void markAtPoint(ColorImagePoint p, Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.DrawEllipse(Pens.Red, p.X - 20, p.Y - 20, 40, 40);
            //g.DrawEllipse(Pens.Red, new Rectangle(p.X - 20, p.Y - 20, 40, 40));
        }

        void StopKinect(KinectSensor _sensor)
        {
            if (_sensor != null)
            {
                _sensor.Stop();
                _sensor.AudioSource.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(kinectSensorChooser1.Kinect);
        }

        private void TiltSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TiltAngle.Content = (int)TiltSlider.Value;
        }

        private void setTilt_Click(object sender, RoutedEventArgs e)
        {
            if (kinectSensorChooser1.Kinect != null && kinectSensorChooser1.Kinect.IsRunning)
            {
                kinectSensorChooser1.Kinect.ElevationAngle = (int)TiltSlider.Value;
            }
        }

        private void image2_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (!recordSet)
            {
                // begin writing
                writer = new StreamWriter(FileText.Text + ".txt");
                dataStream = new FileStream(FileText.Text + ".dat", FileMode.Create);

                masterData = new StreamFileReader(FileText.Text + ".txt");


                masterLeftWristData = masterData.getJointArray("WristLeft");
                masterRightWristData = masterData.getJointArray("WristRight");
                masterLeftElbowData = masterData.getJointArray("WristLeft");
                masterRightElbowData = masterData.getJointArray("ElbowRight");
                masterLeftShoulderData = masterData.getJointArray("ShoulderLeft");
                masterRightShoulderData = masterData.getJointArray("ShoulderRIght");
                masterLeftAnkleData = masterData.getJointArray("AnkleLeft");
                masterRightAnkleData = masterData.getJointArray("AnkleRight");
                masterLeftKneeData = masterData.getJointArray("KneeLeft");
                masterRightKneeData = masterData.getJointArray("KneeRight");
                masterLeftHipData = masterData.getJointArray("HipLeft");
                masterRightHipData = masterData.getJointArray("HipRight");

                

                // swap image to stop.png
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("/VirtualSifu;component/Images/stop.png", UriKind.Relative);
                bitmap.EndInit();
                image2.Stretch = Stretch.Fill;
                image2.Source = bitmap;

                // set state
                recordSet = true;
            }
            else
            {
                // stop writing
                writer.Close();
                dataStream.Close();

                // swap image to play.png
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("/VirtualSifu;component/Images/play.png", UriKind.Relative);
                bitmap.EndInit();
                image2.Stretch = Stretch.Fill;
                image2.Source = bitmap;

                // set state
                recordSet = false;
            }

            
        }

        private void image2_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            playback = true;
            StreamFileReader masterData = new StreamFileReader(FileText.Text + ".txt");
            dataStream = new FileStream(FileText.Text + ".dat", FileMode.Open, FileAccess.Read);
        }

        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {

            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
                {
                    return;
                }


                //Map a joint location to a point on the depth map


                DepthImagePoint rightWristPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.WristRight].Position);
                DepthImagePoint leftWristDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.WristLeft].Position);
                DepthImagePoint rightElbowPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.ElbowRight].Position);
                DepthImagePoint leftElbowDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.ElbowLeft].Position);
                DepthImagePoint rightShoulderPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.ShoulderRight].Position);
                DepthImagePoint leftSholderDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.ShoulderLeft].Position);
                DepthImagePoint rightAnkleDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.AnkleRight].Position);
                DepthImagePoint leftAnkleDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.AnkleLeft].Position);
                DepthImagePoint rightKneePoint = depth.MapFromSkeletonPoint(first.Joints[JointType.KneeRight].Position);
                DepthImagePoint leftKneeDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.KneeLeft].Position);
                DepthImagePoint rightHipPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.HipRight].Position);
                DepthImagePoint leftHipDepthPoint = depth.MapFromSkeletonPoint(first.Joints[JointType.HipLeft].Position);



                //Map a depth point to a point on the color image
                ColorImagePoint rightWristColorPoint = depth.MapToColorImagePoint(rightWristPoint.X, rightWristPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint leftWristColorPoint = depth.MapToColorImagePoint(leftWristDepthPoint.X, leftWristDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint rightElbowColorPoint = depth.MapToColorImagePoint(rightElbowPoint.X, rightElbowPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint leftElbowColorPoint = depth.MapToColorImagePoint(leftElbowDepthPoint.X, leftElbowDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint rightShoulderColorPoint = depth.MapToColorImagePoint(rightShoulderPoint.X, rightShoulderPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint leftShoulderColorPoint = depth.MapToColorImagePoint(leftSholderDepthPoint.X, leftSholderDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint rightAnkleColorPoint = depth.MapToColorImagePoint(rightAnkleDepthPoint.X, rightAnkleDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint leftAnkleColorPoint = depth.MapToColorImagePoint(leftAnkleDepthPoint.X, leftAnkleDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint rightKneeColorPoint = depth.MapToColorImagePoint(rightKneePoint.X, rightKneePoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint leftKneeColorPoint = depth.MapToColorImagePoint(leftKneeDepthPoint.X, leftKneeDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint rightHipColorPoint = depth.MapToColorImagePoint(rightHipPoint.X, rightHipPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint leftHipColorPoint = depth.MapToColorImagePoint(leftHipDepthPoint.X, leftHipDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);


                //Set location
                CameraPosition(wristRight, rightWristColorPoint);
                CameraPosition(wristLeft, leftWristColorPoint);
                CameraPosition(elbowRight, rightElbowColorPoint);
                CameraPosition(elbowLeft, leftElbowColorPoint);
                CameraPosition(shoulderRight, rightShoulderColorPoint);
                CameraPosition(shoulderLeft , leftShoulderColorPoint);
                CameraPosition(ankleRight, rightAnkleColorPoint);
                CameraPosition(ankleLeft, leftAnkleColorPoint);
                CameraPosition(kneeRight, rightKneeColorPoint);
                CameraPosition(kneeLeft, leftKneeColorPoint);
                CameraPosition(hipRight, rightHipColorPoint);
                CameraPosition(hipLeft, leftHipColorPoint);
            }
        }

        private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            //Divide by 2 for width and height so point is right in the middle 
            // instead of in top/left corner
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);

        }


        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            //convert the value to X/Y
            //Joint scaledJoint = joint.ScaleTo(1280, 720); 

            //convert & scale (.3 = means 1/3 of joint distance)
            //Joint scaledJoint = ScaleTo(joint, 1280, 720, .3f, .3f);

            Microsoft.Kinect.SkeletonPoint pos = new SkeletonPoint()
            {
                X = Scale(1280, 3f, joint.Position.X),
                Y = Scale(720, 3f, -joint.Position.Y),
                Z = joint.Position.Z
            };

            joint.Position = pos;
            
            Canvas.SetLeft(element, joint.Position.X);
            Canvas.SetTop(element, joint.Position.Y);

        }

        private static float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }

        class ProfileData
        {
            public String[] joints;
            public ArrayList[] data;

            public ProfileData(String[] joints)
            {
                this.joints = joints;
                int i = 0;
                data = new ArrayList[joints.Length];
                foreach (String joint in joints)
                {
                    data[i] = new ArrayList(30);
                    for (int j = 0; j < 30; j++)
                        data[i].Add(new JointData(0, 0, 0));
                    i++;
                }
            }

            public ArrayList Get(String joint)
            {
                for (int i = 0; i < joints.Length; i++)
                    if (joints[i].Equals(joint))
                        return data[i];
                    else
                        return data[0];
                return data[0];
            }

        }
    }


}