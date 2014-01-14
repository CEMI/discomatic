using System;
using System.Collections.Generic;
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
using Microsoft.Kinect;
using System.Drawing;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using AForge.Math.Geometry;
using AForge;
using CannedBytes.Midi;

namespace StepsMIDI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public byte[] pixelData;
        byte[] depth32;
        int colorIndex;
        BitmapSource BlobBitmapFeed;
        int slowdown = 0;
        int[] currentblobxpos = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] currentblobypos = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        int[] inbox = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ,0 , 0 , 0 , 0 };
        int[] previnbox = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        int[] pinbox = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        int[] allzeros = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
       
 
        int[] prevblobxpos = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        string blobsfound = null;
        string prevblobsfound = null;
        int peopleinside = 0;
        int peopleinlobby = 0;
        int peopleoutside = 0;
        string dir = "Unknown";
        string currentpos = "";
        String prevposition = "";



        public MainWindow()
        {
            InitializeComponent();
        }



        //Depth camera feed generator start Sensor 1

        void sensor1_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {

            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }
                if (slowdown == 3)
                {
                    short[] pixelData = new short[depthimageFrame.PixelDataLength];
                    int stride = depthimageFrame.Width * 2;
                    depthimageFrame.CopyPixelDataTo(pixelData);

                    depth32 = new byte[depthimageFrame.PixelDataLength * 4];
                    this.GetColorPixelDataWithDistance(pixelData);

                    deplthimageimg.Source = BlobBitmapFeed = BitmapSource.Create(
                    depthimageFrame.Width, depthimageFrame.Height, 72, 72, PixelFormats.Bgr24, null, depth32, depthimageFrame.Width * 4);
                  Blob[] MyBlobs = blobcounter(BlobBitmapFeed);
                  canvas1.Children.Clear();
                    blobprocess(MyBlobs);
                    slowdown = 0;


                   
                }
                else //if (slowdown ==30)
                {
                    slowdown++;
                }
            }

            


        }

        //Depth camera feed generator END Sensor 1


        //Processing Logic Start
        void blobprocess(Blob[] detectedblobs)
        {
            for (int blobcount = 0; blobcount < detectedblobs.Length; blobcount++)
            {
               
                gereratemarkers(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y, (detectedblobs[blobcount].ID - 1).ToString());

                setbox1value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox2value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox3value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox4value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox5value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox6value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox7value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox8value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                setbox9value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox10value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox11value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox12value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox13value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox14value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox15value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                //setbox16value(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                
                
            }


            
            for (int boxnumber = 0; boxnumber < inbox.Length; boxnumber++)
            {
                if (inbox[boxnumber] == 1)
                {

                    Statuslbl.Content = boxnumber.ToString();

            
                }

            }

            pinbox = inbox;
            }
        
        

        
        //Processing Logic End


        // set box values starts
        public void setbox1value(int X, int Y)
        {
            if ((X > 40 && X < 120) && (Y > 0 && Y < 80) && inbox[0] != 1)
            {
                if (previnbox[0] == 0)
                {
                    play0();
                
                }

                inbox[0] = 1;
            }
            else
            {
                inbox[0] = 0;
            }
        }

        public void setbox2value(int X, int Y)
        {
            if ((X > 121 && X < 200) && (Y > 0 && Y < 80) && inbox[1] != 1)
            {
                if (previnbox[1] == 0)
                {
                    play1();

                }

                inbox[1] = 1;
            }
            else
            {
                inbox[1] = 0;
            }
        }

        public void setbox3value(int X, int Y)
        {
            if ((X > 201 && X < 280) && (Y > 0 && Y < 80) && inbox[2] != 1)
            {
                if (previnbox[2] == 0)
                {
                    play2();

                }

                inbox[2] = 1;
            }
            else
            {
                inbox[2] = 0;
            }
        }

        public void setbox4value(int X, int Y)
        {
            if ((X > 40 && X < 120) && (Y > 81 && Y < 160) && inbox[3] != 1)
            {
                if (previnbox[3] == 0)
                {
                    play3();

                }

                inbox[3] = 1;
            }
            else
            {
                inbox[3] = 0;
            }
        }


        public void setbox5value(int X, int Y)
        {
            if ((X > 121 && X < 200) && (Y > 81 && Y < 160) && inbox[4] != 1)
            {
                if (pinbox[4] == 0)
                {
                    play4();

                }

                inbox[4] = 1;
               
            }
            else
            {
                inbox[4] = 0;
            }
        }

        public void setbox6value(int X, int Y)
        {
            if ((X > 201 && X < 280 && (Y > 81 && Y < 160) ))
            {
                if (previnbox[5] == 0)
                {
                    play5();

                }

                inbox[5] = 1;
            }
            else
            {
                inbox[5] = 0;
            }
        }

        public void setbox7value(int X, int Y)
        {
            if ((X > 40 && X < 120) && (Y > 161 && Y < 240) && inbox[6] != 1)
            {
                if (previnbox[6] == 0)
                {
                    play6();

                }

                inbox[6] = 1;
            }
            else
            {
                inbox[6] = 0;
            }
        }

        public void setbox8value(int X, int Y)
        {
            if ((X > 121 && X < 200) && (Y > 161 && Y < 240) && inbox[7] != 1)
            {
                if (previnbox[7] == 0)
                {
                    play7();

                }

                inbox[7] = 1;
            }
            else
            {
                inbox[7] = 0;
            }
        }



        public void setbox9value(int X, int Y)
        {
            if ((X > 201 && X < 280) && (Y > 161 && Y < 240) && inbox[8] != 1)
            {
                if (previnbox[8] == 0)
                {
                    play8();

                }

                inbox[8] = 1;
            }
            else
            {
                inbox[8] = 0;
            }
        }

        public void setbox10value(int X, int Y)
        {
            if ((X > 101 && X < 160) && (Y > 121 && Y < 180) && inbox[9] != 1)
            {
                if (previnbox[9] == 0)
                {
                    play9();

                }

                inbox[9] = 1;
            }
            else
            {
                inbox[9] = 0;
            }
        }

        public void setbox11value(int X, int Y)
        {
            if ((X > 161 && X < 220) && (Y > 121 && Y < 180) && inbox[10] != 1)
            {
                if (previnbox[10] == 0)
                {
                    play10();

                }

                inbox[10] = 1;
            }
            else
            {
                inbox[10] = 0;
            }
        }

        public void setbox12value(int X, int Y)
        {
            if ((X > 221 && X < 280) && (Y > 121 && Y < 180) && inbox[11] != 1)
            {
                if (previnbox[11] == 0)
                {
                    play11();

                }

                inbox[11] = 1;
            }
            else
            {
                inbox[11] = 0;
            }
        }


        public void setbox13value(int X, int Y)
        {
            if ((X > 40 && X < 100) && (Y > 181 && Y < 240) && inbox[12] != 1)
            {
                if (previnbox[12] == 0)
                {
                    play12();

                }

                inbox[12] = 1;
            }
            else
            {
                inbox[12] = 0;
            }
        }

        public void setbox14value(int X, int Y)
        {
            if ((X > 101 && X < 160) && (Y > 181 && Y < 240) && inbox[13] != 1)
            {
                if (previnbox[13] == 0)
                {
                    play13();

                }

                inbox[13] = 1;
            }
            else
            {
                inbox[13] = 0;
            }
        }

        public void setbox15value(int X, int Y)
        {
            if ((X > 161 && X < 220) && (Y > 181 && Y < 240) && inbox[14] != 1)
            {
                if (previnbox[14] == 0)
                {
                    play14();

                }

                inbox[14] = 1;
            }
            else
            {
                inbox[14] = 0;
            }
        }

        public void setbox16value(int X, int Y)
        {
            if ((X > 221 && X < 280) && (Y > 181 && Y < 240) && inbox[15] != 1)
            {
                if (previnbox[15] == 0)
                {
                    play15();

                }

                inbox[15] = 1;
            }
            else
            {
                inbox[15] = 0;
            }
        }


        // set box values end
        
        

        //MIDI Generators Start

        public void play0()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 65; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play1()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 64; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData); 
        }

        public void play2()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 63; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play3()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 62; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play4()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 61; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play5()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 60; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play6()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 59; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play7()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 58; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play8()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 57; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play9()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 56; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play10()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 57; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play11()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 56; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play12()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 55; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play13()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 54; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play14()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 53; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }

        public void play15()
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = 52; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }
        //MIDI Generators END



        
        
        //Generate Markers 1 Start
        public void gereratemarkers(int x, int y, string id)
        {

            int posx = Convert.ToInt16((x));
            int posy = Convert.ToInt16((y));
            System.Windows.Shapes.Rectangle Rectangle1 = new System.Windows.Shapes.Rectangle();
            Rectangle1.Width = 10;
            Rectangle1.Height = 10;
            Rectangle1.Fill = System.Windows.Media.Brushes.Red;
            Canvas.SetLeft(Rectangle1, posx - 5);
            Canvas.SetTop(Rectangle1, posy - 5);
            canvas1.Children.Add(Rectangle1);

        }

        //Generate Markers 1 End

        //Bitmap Source to Bitmap Convertor Start
        public System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        //Bitmap Source to Bitmap Convertor End



        //Blob Counter Start

        public Blob[] blobcounter(BitmapSource bsource)
        {
            System.Drawing.Bitmap b = BitmapFromSource(bsource);

            BlobCounter BCounter = new BlobCounter();

            ColorFiltering FilterObjects = new ColorFiltering();

            BCounter.FilterBlobs = true;
            BCounter.MinWidth = 50;
            BCounter.MinHeight = 50;

            BCounter.ProcessImage(b);


            Blob[] detectedblobs = BCounter.GetObjectsInformation();

            return detectedblobs;
        }

        //Blob Counter End





        //color camera feed generator start sensor 1

        void sensor1_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {

            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                // Check if the incoming frame is not null
                if (imageFrame == null)
                {
                    return;
                }
                else
                {
                    depth32 = new byte[imageFrame.PixelDataLength * 4];

                    pixelData = new byte[imageFrame.PixelDataLength];
                    // Copy the pixel data

                    imageFrame.CopyPixelDataTo(this.pixelData);
                    // Calculate the stride

                    int stride = imageFrame.Width * imageFrame.BytesPerPixel;
                    // assign the bitmap image source into image control

                    colorimageimg.Source = BitmapSource.Create(
                    imageFrame.Width,
                    imageFrame.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null,
                    pixelData,
                    stride);
                }

            }



        }

        //color camera feed generator end sensor 1





        //change depth stream to blobs on picture start Kinect 1
        private void GetColorPixelDataWithDistance(short[] depthFrame)
        {
            for (int depthIndex = 0, colorIndex = 0; depthIndex < depthFrame.Length && colorIndex < this.depth32.Length; depthIndex++, colorIndex += 4)
            {
                // Calculate the depth distance
                int distance = depthFrame[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                // Colorize pixels for a range of distance
                if (distance <= 300)
                {
                    depth32[colorIndex + 2] = 0;
                    depth32[colorIndex + 1] = 0;
                    depth32[colorIndex + 0] = 0;

                }
                else if (distance > 301 && distance <= 2500)
                {
                    depth32[colorIndex + 2] = 255; // red
                    depth32[colorIndex + 1] = 255; // green
                    depth32[colorIndex + 0] = 255; // blue

                }
                else if (distance > 2501)
                {
                    depth32[colorIndex + 2] = 0;
                    depth32[colorIndex + 1] = 0;
                    depth32[colorIndex + 0] = 0;
                }
            }
        }

        //change depth stream to blobs on picture end kinect 1

        MidiOutPort midiOut = new MidiOutPort();

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
          
            midiOut.Open(1);

            //kinect initialize start
            KinectSensor sensor1;

            if (KinectSensor.KinectSensors.Count < 1)
            {
                MessageBox.Show("No device is connected with system!");
                this.Close();
            }

            else
            {
                //check for two sensor begins
                if (KinectSensor.KinectSensors.Count < 2)
                {

                  //  Statuslbl.Content = "Status: Detecting Counting";
                    sensor1 = KinectSensor.KinectSensors[0];
                    sensor1.Start();
                    sensor1.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                    sensor1.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);

                    //kinect initialize end

                    //capture frame events
                    sensor1.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor1_ColorFrameReady);
                    sensor1.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor1_DepthFrameReady);
                }
                //check for two sensors end
            }

        }

    }
}
