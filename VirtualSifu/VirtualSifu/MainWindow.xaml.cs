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
        int frameNumber = 0;

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
                    //for (int i = 0; i < pixels.Length; i++)
                    //{
                    //    writer.Write(pixels[i]);
                    //    writer.Write(' ');
                    //}
                    //writer.WriteLine("end");

                    colorFrame.CopyPixelDataTo(pixels);
                    
                    int stride = colorFrame.Width * 4;
                    masterView.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                }
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
                {
                    //TimeSpan timeSpan = DateTime.Now.Subtract(referenceTime);
                    //String referenceTime = DateTime.Now;
                    writer.Write(frameNumber);
                    writer.Write(skeletonFrame.FloorClipPlane);
                    //writer.Write((int)skeletonFrame.Quality);
                    //writer.Write(skeletonFrame.NormalToGravity);

                    writer.Write(skeletonFrame.SkeletonArrayLength);
                    Skeleton[] data = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(data);
                    foreach (Skeleton skeleton in data)
                    {
                        writer.Write((int)skeleton.TrackingState);
                        writer.Write(skeleton.Position);
                        writer.Write(skeleton.TrackingId);
                        writer.Write(skeleton.TrackingState);
                        writer.Write(skeleton.Joints);
                        writer.Write((int)skeleton.ClippedEdges);

                        writer.Write(skeleton.Joints.Count);
                        foreach (Joint joint in skeleton.Joints)
                        {
                            writer.Write((int)joint.JointType);
                            writer.Write((int)joint.TrackingState);
                            writer.Write(joint.Position);
                        }
                    }
                    
                }
            }
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            writer = new StreamWriter(FileText.Text);
            recordSet = true;
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            recordSet = false;

            writer.Close();
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

    }
}
