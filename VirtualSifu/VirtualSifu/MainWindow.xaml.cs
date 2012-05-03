using System;
using System.Collections.Generic;
using System.Collections;
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
        bool playback = false;

        public MainWindow()
        {
            InitializeComponent();
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
                    byte[] pixels = new byte[colorFrame.PixelDataLength];
                    dataStream.Read(pixels, 0, colorFrame.PixelDataLength);
                    int stride = colorFrame.Width * 4;
                    masterView.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
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
                            }
                        }
                    } 
                }
            }
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
            dataStream = new FileStream(FileText.Text + ".dat", FileMode.Open, FileAccess.Read);
        }

    }
}
