
namespace Haogle
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.Globalization;
    using Microsoft.Kinect;
    using NAudio.Wave;
    public partial class Voice : Window
    {
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        /// <summary>
        /// Number of samples captured from Kinect audio stream each millisecond.
        /// </summary>
        private const int SamplesPerMillisecond = 16;

        /// <summary>
        /// Number of bytes in each Kinect audio stream sample (32-bit IEEE float).
        /// </summary>
        private const int BytesPerSample = sizeof(float);

        /// <summary>
        /// Number of audio samples represented by each column of pixels in wave bitmap.
        /// </summary>
        private const int SamplesPerColumn = 40;

        /// <summary>
        /// Minimum energy of audio to display (a negative number in dB value, where 0 dB is full scale)
        /// </summary>
        private const int MinEnergy = -90;

        /// <summary>
        /// Width of bitmap that stores audio stream energy data ready for visualization.
        /// </summary>
        private const int EnergyBitmapWidth = 780;

        /// <summary>
        /// Height of bitmap that stores audio stream energy data ready for visualization.
        /// </summary>
        private const int EnergyBitmapHeight = 195;

        /// <summary>
        /// Bitmap that contains constructed visualization for audio stream energy, ready to
        /// be displayed. It is a 2-color bitmap with white as background color and blue as
        /// foreground color.
        /// </summary>
        private readonly WriteableBitmap energyBitmap;

        /// <summary>
        /// Rectangle representing the entire energy bitmap area. Used when drawing background
        /// for energy visualization.
        /// </summary>
        private readonly Int32Rect fullEnergyRect = new Int32Rect(0, 0, EnergyBitmapWidth, EnergyBitmapHeight);

        /// <summary>
        /// Array of background-color pixels corresponding to an area equal to the size of whole energy bitmap.
        /// </summary>
        private readonly byte[] backgroundPixels = new byte[EnergyBitmapWidth * EnergyBitmapHeight];

        /// <summary>
        /// Will be allocated a buffer to hold a single sub frame of audio data read from audio stream.
        /// </summary>
        private readonly byte[] audioBuffer = null;

        /// <summary>
        /// Buffer used to store audio stream energy data as we read audio.
        /// We store 25% more energy values than we strictly need for visualization to allow for a smoother
        /// stream animation effect, since rendering happens on a different schedule with respect to audio
        /// capture.
        /// </summary>
        private readonly float[] energy = new float[(uint)(EnergyBitmapWidth * 1.25)];

        /// <summary>
        /// Object for locking energy buffer to synchronize threads.
        /// </summary>
        private readonly object energyLock = new object();

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for audio frames
        /// </summary>
        private AudioBeamFrameReader reader = null;

        /// <summary>
        /// Last observed audio beam angle in radians, in the range [-pi/2, +pi/2]
        /// </summary>
        private float beamAngle = 0;

        /// <summary>
        /// Last observed audio beam angle confidence, in the range [0, 1]
        /// </summary>
        private float beamAngleConfidence = 0;

        /// <summary>
        /// Array of foreground-color pixels corresponding to a line as long as the energy bitmap is tall.
        /// This gets re-used while constructing the energy visualization.
        /// </summary>
        private byte[] foregroundPixels;

        /// <summary>
        /// Sum of squares of audio samples being accumulated to compute the next energy value.
        /// </summary>
        private float accumulatedSquareSum;

        /// <summary>
        /// Number of audio samples accumulated so far to compute the next energy value.
        /// </summary>
        private int accumulatedSampleCount;

        /// <summary>
        /// Index of next element available in audio energy buffer.
        /// </summary>
        private int energyIndex;

        /// <summary>
        /// Number of newly calculated audio stream energy values that have not yet been
        /// displayed.
        /// </summary>
        private int newEnergyAvailable;

        /// <summary>
        /// Error between time slice we wanted to display and time slice that we ended up
        /// displaying, given that we have to display in integer pixels.
        /// </summary>
        private float energyError;

        /// <summary>
        /// Last time energy visualization was rendered to screen.
        /// </summary>
        private DateTime? lastEnergyRefreshTime;

        /// <summary>
        /// Index of first energy element that has never (yet) been displayed to screen.
        /// </summary>
        private int energyRefreshIndex;
       

        public Voice()
        {
            InitializeComponent();

            

            // Initialize the components (controls) of the window
            this.InitializeComponent();

            // Only one Kinect Sensor is supported
            this.kinectSensor = KinectSensor.GetDefault();

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            if (this.kinectSensor != null)
            {
                // Open the sensor
                this.kinectSensor.Open();

                // Get its audio source
                AudioSource audioSource = this.kinectSensor.AudioSource;

                // Allocate 1024 bytes to hold a single audio sub frame. Duration sub frame 
                // is 16 msec, the sample rate is 16khz, which means 256 samples per sub frame. 
                // With 4 bytes per sample, that gives us 1024 bytes.
                this.audioBuffer = new byte[audioSource.SubFrameLengthInBytes];

                // Open the reader for the audio frames
                this.reader = audioSource.OpenReader();

                // Uncomment these two lines to overwrite the automatic mode of the audio beam.
                // It will change the beam mode to manual and set the desired beam angle.
                // In this example, point it straight forward.
                // Note that setting beam mode and beam angle will only work if the
                // application window is in the foreground.
                // Furthermore, setting these values is an asynchronous operation --
                // it may take a short period of time for the beam to adjust.
                /*
                audioSource.AudioBeams[0].AudioBeamMode = AudioBeamMode.Manual;
                audioSource.AudioBeams[0].BeamAngle = 0;
                */
            }
            else
            {
                // On failure, set the status text
               this.statusBarText.Text = Properties.Resources.NoSensorStatusText;
                return;
            }

            this.energyBitmap = new WriteableBitmap(EnergyBitmapWidth, EnergyBitmapHeight, 96, 96, PixelFormats.Indexed1, new BitmapPalette(new List<Color> { Colors.White, (Color)this.Resources["KinectPurpleColor"] }));

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize foreground pixels
            this.foregroundPixels = new byte[EnergyBitmapHeight];
            for (int i = 0; i < this.foregroundPixels.Length; ++i)
            {
                this.foregroundPixels[i] = 0xff;
            }

            this.waveDisplay.Source = this.energyBitmap;

            CompositionTarget.Rendering += this.UpdateEnergy;

            if (this.reader != null)
            {
                // Subscribe to new audio frame arrived events
                this.reader.FrameArrived += this.Reader_FrameArrived;
            }
        }


        private void StartBtn_Click_1(object sender, RoutedEventArgs e)
        {
            statusBlock.Text = "Never use it when it's offline";
            StartBtn.IsEnabled = false;
            StopBtn.IsEnabled = true;

            waveSource = new WaveIn();
            waveSource.WaveFormat = new WaveFormat(16000, 1);

            waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
            waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

            waveFile = new WaveFileWriter("E:\\test.wav", waveSource.WaveFormat);

            waveSource.StartRecording();
        }

        private void StopBtn_Click_1(object sender, RoutedEventArgs e)
        {
            statusBlock.Text = "recording is over recognize by cloud";
            // record.Stop();
            StopBtn.IsEnabled = false;

            waveSource.StopRecording();
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
            Thread thread = new Thread(new ThreadStart(Post));
            thread.Start();
           
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFile != null)
            {
                waveFile.Write(e.Buffer, 0, e.BytesRecorded);
                waveFile.Flush();
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveSource != null)
            {
                waveSource.Dispose();
                waveSource = null;
            }

            if (waveFile != null)
            {
                waveFile.Dispose();
                waveFile = null;
            }
            //  waveFile.Close();
            StartBtn.IsEnabled = true;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
        
            //MessageBox.Show("关闭界面");
            CompositionTarget.Rendering -= this.UpdateEnergy;

            if (this.reader != null)
            {
                // AudioBeamFrameReader is IDisposable
                this.reader.Dispose();
                this.reader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
           // record = null;
            MainWindow wm = new MainWindow();
            wm.Show();
            wm.WindowState = WindowState.Maximized;
        }


       // delegate void MyDelegate();
            /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// 
        // string token = "24.803fabb93511ae13536db13b8df6cc06.2592000.1424937486.282335-5337828";
        string token = "24.dab4a13db5d359fe8c1b792b4a2343c1.2592000.1433844751.282335-5337828";
         string apiKey = "kO4SRbRuMpNpZwrDGHHVUati";//对应百度云界面基本信息的API Key
         string secretKey = "heCEtOzbWelqIFCtZqAdLwLrj8x2DiG0";//对应百度云界面基本信息的Secret Key
        string cuid = "123456";//这个随便写  不过尽量写唯一的，比如自己创建个guid，或者你手机号码什么的都可以
        string getTokenURL = "";
        string serverURL = "http://vop.baidu.com/server_api";
       

        ////这个方法得到一个密钥，这个密钥可以使用1个月，1个月之后要重新请求一次获得一个
        //private void getToken()
        //{
        //    getTokenURL = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials" +
        //    "&client_id=" + apiKey + "&client_secret=" + secretKey;
        //    token = GetValue("access_token");
  
        //}

        //private string GetValue(string key)
        //{
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(getTokenURL);
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    StreamReader reader1 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        //    string ssss = reader1.ReadToEnd().Replace("\"", "").Replace("{", "").Replace("}", "").Replace("\n", "");
        //    string[] indexs = ssss.Split(',');
        //    foreach (string index in indexs)
        //    {
        //        string[] _indexs = index.Split(':');
        //        if (_indexs[0] == key)
        //            return _indexs[1];
        //    }
        //    return "";
        //}




        private void Post()
        {

            serverURL += "?lan=zh&cuid=kwwwvagaa&token=" + token;
            string inFile = "E:\\test.wav";
            FileStream fs = new FileStream(inFile, FileMode.Open);
            byte[] voice = new byte[fs.Length];
            fs.Read(voice, 0, voice.Length);
            fs.Close();

            HttpWebRequest request = null;

            Uri uri = new Uri(serverURL);
            request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = 50000;
            request.Method = "POST";
            request.ContentType = "audio/wav; rate=16000";
            request.ContentLength = voice.Length;
            try
            {
                using (Stream writeStream = request.GetRequestStream())
                {
                    writeStream.Write(voice, 0, voice.Length);
                    writeStream.Close();
                    writeStream.Dispose();
                }
            }
            catch
            {
                return;
            }
            string voice_result = string.Empty;
            string result2= string.Empty;
            string index1 = string.Empty;
            //如何在这前面确保这个结果不为空
            //最后如何将UTF-8码转化为汉字并且显示出来，这是我们必须要解决的两个问题
         // (HttpWebResponse)request.GetResponse())
            try
            {

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            string line = string.Empty;
                            StringBuilder sb = new StringBuilder();
                            while (!readStream.EndOfStream)
                            {
                                line = readStream.ReadLine();
                                sb.Append(line);
                                sb.Append("\r");
                            }

                            // voice_result = readStream.ReadToEnd();

                            voice_result = sb.ToString();
                           

                            //message = voice_result.Substring(voice_result.IndexOf("utterance") + 12);
                            //message = message.Substring(0, message.IndexOf("\""));
                            readStream.Close();
                            readStream.Dispose();
                            string result1 = voice_result.Replace("[", "").Replace("{", "").Replace("}", "").Replace("\n", "").Replace("]", "");
                            string[] indexs = result1.Split(',');
                            foreach (string index in indexs)
                            {
                                index1 = index.Replace("\"", "");
                                string[] _indexs = index1.Split(':');
                                if (_indexs[0] == "result")
                                {
                                    result2 = _indexs[1];
                                    break;
                                }


                            }
                            MessageBox.Show(result2);

                            string outStr = "";
                            if (!string.IsNullOrEmpty(result2))
                            {
                                string[] strlist = result2.Replace("\\", "").Split('u');
                                try
                                {
                                    for (int i = 1; i < strlist.Length; i++)
                                    {
                                        //将unicode字符转为10进制整数，然后转为char中文字符 
                                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                                    }
                                }
                                catch (FormatException ex)
                                {
                                    outStr = ex.Message;
                                }
                            }
                            //MessageBox.Show(outStr);


                        }
                        responseStream.Close();
                        responseStream.Dispose();
                    }
                    response.Close();
                }
            }
            catch
            {
                MessageBox.Show("check the internet connection and try again");
            }
          //  Console.WriteLine("语音解析 :" + voice_result);
         

        }
        /// <summary>
        /// Handles the audio frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            AudioBeamFrameReference frameReference = e.FrameReference;
            AudioBeamFrameList frameList = frameReference.AcquireBeamFrames();

            if (frameList != null)
            {
                // AudioBeamFrameList is IDisposable
                using (frameList)
                {
                    // Only one audio beam is supported. Get the sub frame list for this beam
                    IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                    // Loop over all sub frames, extract audio buffer and beam information
                    foreach (AudioBeamSubFrame subFrame in subFrameList)
                    {
                        // Check if beam angle and/or confidence have changed
                        bool updateBeam = false;

                        if (subFrame.BeamAngle != this.beamAngle)
                        {
                            this.beamAngle = subFrame.BeamAngle;
                            updateBeam = true;
                        }

                        if (subFrame.BeamAngleConfidence != this.beamAngleConfidence)
                        {
                            this.beamAngleConfidence = subFrame.BeamAngleConfidence;
                            updateBeam = true;
                        }

                        if (updateBeam)
                        {
                            // Refresh display of audio beam
                            this.AudioBeamChanged();
                        }

                        // Process audio buffer
                        subFrame.CopyFrameDataToArray(this.audioBuffer);

                        for (int i = 0; i < this.audioBuffer.Length; i += BytesPerSample)
                        {
                            // Extract the 32-bit IEEE float sample from the byte array
                            float audioSample = BitConverter.ToSingle(this.audioBuffer, i);

                            this.accumulatedSquareSum += audioSample * audioSample;
                            ++this.accumulatedSampleCount;

                            if (this.accumulatedSampleCount < SamplesPerColumn)
                            {
                                continue;
                            }

                            float meanSquare = this.accumulatedSquareSum / SamplesPerColumn;

                            if (meanSquare > 1.0f)
                            {
                                // A loud audio source right next to the sensor may result in mean square values
                                // greater than 1.0. Cap it at 1.0f for display purposes.
                                meanSquare = 1.0f;
                            }

                            // Calculate energy in dB, in the range [MinEnergy, 0], where MinEnergy < 0
                            float energy = MinEnergy;

                            if (meanSquare > 0)
                            {
                                energy = (float)(10.0 * Math.Log10(meanSquare));
                            }

                            lock (this.energyLock)
                            {
                                // Normalize values to the range [0, 1] for display
                                this.energy[this.energyIndex] = (MinEnergy - energy) / MinEnergy;
                                this.energyIndex = (this.energyIndex + 1) % this.energy.Length;
                                ++this.newEnergyAvailable;
                            }

                            this.accumulatedSquareSum = 0;
                            this.accumulatedSampleCount = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method called when audio beam angle and/or confidence have changed.
        /// </summary>
        private void AudioBeamChanged()
        {
            // Maximum possible confidence corresponds to this gradient width
            const float MinGradientWidth = 0.04f;

            // Set width of mark based on confidence.
            // A confidence of 0 would give us a gradient that fills whole area diffusely.
            // A confidence of 1 would give us the narrowest allowed gradient width.
            float halfWidth = Math.Max(1 - this.beamAngleConfidence, MinGradientWidth) / 2;

            // Update the gradient representing sound source position to reflect confidence
            this.beamBarGsPre.Offset = Math.Max(this.beamBarGsMain.Offset - halfWidth, 0);
            this.beamBarGsPost.Offset = Math.Min(this.beamBarGsMain.Offset + halfWidth, 1);

            // Convert from radians to degrees for display purposes
            float beamAngleInDeg = this.beamAngle * 180.0f / (float)Math.PI;

            // Rotate gradient to match angle
            beamBarRotation.Angle = -beamAngleInDeg;
            beamNeedleRotation.Angle = -beamAngleInDeg;

            // Display new numerical values
            beamAngleText.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.BeamAngle, beamAngleInDeg.ToString("0", CultureInfo.CurrentCulture));
            beamConfidenceText.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.BeamAngleConfidence, this.beamAngleConfidence.ToString("0.00", CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Handles rendering energy visualization into a bitmap.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void UpdateEnergy(object sender, EventArgs e)
        {
            lock (this.energyLock)
            {
                // Calculate how many energy samples we need to advance since the last update in order to
                // have a smooth animation effect
                DateTime now = DateTime.UtcNow;
                DateTime? previousRefreshTime = this.lastEnergyRefreshTime;
                this.lastEnergyRefreshTime = now;

                // No need to refresh if there is no new energy available to render
                if (this.newEnergyAvailable <= 0)
                {
                    return;
                }

                if (previousRefreshTime != null)
                {
                    float energyToAdvance = this.energyError + (((float)(now - previousRefreshTime.Value).TotalMilliseconds * SamplesPerMillisecond) / SamplesPerColumn);
                    int energySamplesToAdvance = Math.Min(this.newEnergyAvailable, (int)Math.Round(energyToAdvance));
                    this.energyError = energyToAdvance - energySamplesToAdvance;
                    this.energyRefreshIndex = (this.energyRefreshIndex + energySamplesToAdvance) % this.energy.Length;
                    this.newEnergyAvailable -= energySamplesToAdvance;
                }

                // clear background of energy visualization area
                this.energyBitmap.WritePixels(this.fullEnergyRect, this.backgroundPixels, EnergyBitmapWidth, 0);

                // Draw each energy sample as a centered vertical bar, where the length of each bar is
                // proportional to the amount of energy it represents.
                // Time advances from left to right, with current time represented by the rightmost bar.
                int baseIndex = (this.energyRefreshIndex + this.energy.Length - EnergyBitmapWidth) % this.energy.Length;
                for (int i = 0; i < EnergyBitmapWidth; ++i)
                {
                    const int HalfImageHeight = EnergyBitmapHeight / 2;

                    // Each bar has a minimum height of 1 (to get a steady signal down the middle) and a maximum height
                    // equal to the bitmap height.
                    int barHeight = (int)Math.Max(1.0, this.energy[(baseIndex + i) % this.energy.Length] * EnergyBitmapHeight);

                    // Center bar vertically on image
                    var barRect = new Int32Rect(i, HalfImageHeight - (barHeight / 2), 1, barHeight);

                    // Draw bar in foreground color
                    this.energyBitmap.WritePixels(barRect, this.foregroundPixels, 1, 0);
                }
            }
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            if (this.kinectSensor != null)
            {
                this.statusBarText.Text = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                                : Properties.Resources.SensorNotAvailableStatusText;
            }
        }
    }
}
