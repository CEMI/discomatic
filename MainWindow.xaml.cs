using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;
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
        BitmapSource BlobBitmapFeed;
        int slowdown = 0;

        int[] inbox;
        int[] previnbox;

        int gridSize = 3;
        int gridPixelSize = 240;
        int marginX = 40;
        int marginY = 0;

        int minDepth = 300;
        int maxDepth = 2500;

        int[] note;
        int noteStart = 65;

        public MainWindow()
        {
            note = new int[gridSize * gridSize];
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                note[i] = noteStart - i;
            }
            inbox = new int[gridSize * gridSize];
            previnbox = new int[gridSize * gridSize];

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
                generatemarkers(detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y, (detectedblobs[blobcount].ID - 1).ToString());

                for (int n = 1; n <= gridSize * gridSize; n++)
                {
                    setboxNvalue(n, detectedblobs[blobcount].CenterOfGravity.X, detectedblobs[blobcount].CenterOfGravity.Y);
                }
            }

            for (int boxnumber = 0; boxnumber < inbox.Length; boxnumber++)
            {
                if (inbox[boxnumber] == 1)
                {
                    Statuslbl.Content = boxnumber.ToString();
                }
            }

            previnbox = inbox;
        }

        //Processing Logic End


        // set box values starts

        public void setboxNvalue(int N, int X, int Y)
        {
            int i = (N - 1) % gridSize;
            int j = (N - 1) / gridSize;
            int minX = marginX + i * gridPixelSize / gridSize;
            int maxX = marginX + (i + 1) * gridPixelSize / gridSize;
            int minY = marginY + j * gridPixelSize / gridSize;
            int maxY = marginY + (j + 1) * gridPixelSize / gridSize;

            if ((X >= minX && X < maxX) && (Y >= minY && Y < maxY) && inbox[N - 1] != 1)
            {
                if (previnbox[N - 1] == 0)
                {
                    playN(N);
                }
                inbox[N - 1] = 1;
            }
            else
            {
                inbox[N - 1] = 0;
            }
        }

        private void playN(int N)
        {
            var midiData = new MidiData();
            midiData.Status = 0x90; // note-on for channel 1
            midiData.Parameter1 = (byte)note[N - 1]; // note number
            midiData.Parameter2 = 100; // velocity
            midiOut.ShortData(midiData);
        }


        //Generate Markers 1 Start
        public void generatemarkers(int x, int y, string id)
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

                    colorimageimg.Source = BitmapSource.Create(imageFrame.Width,
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
                if (distance <= minDepth)
                {
                    depth32[colorIndex + 2] = 0;
                    depth32[colorIndex + 1] = 0;
                    depth32[colorIndex + 0] = 0;

                }
                else if (distance > minDepth && distance <= maxDepth)
                {
                    depth32[colorIndex + 2] = 255; // red
                    depth32[colorIndex + 1] = 255; // green
                    depth32[colorIndex + 0] = 255; // blue

                }
                else if (distance > maxDepth)
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
