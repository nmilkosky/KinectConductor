using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using Kinect.Replay.Record;
using Kinect.Replay.Replay;
using Kinect.Replay.Replay.Color;
using Kinect.Replay.Replay.Skeletons;
using Microsoft.Kinect;
using Microsoft.Win32;

namespace Kinect.Recorder
{
	public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables

        // Variables for interfacing with Kinect
        private KinectSensor myKinect; // variable to interface with Kinect
        private bool _kinectPresent;

        //Constant values
        private const float SWAY_THRESHOLD = 0.02f; //default swaying threshold
        private const float LEAN_THRESHOLD = 0.05f; //default leaning/rocking threshold
        private const float MIRRORING_THRESHOLD = 0.19f; //default mirroring threshold
        private const float HINGE_THRESHOLD = 0.08f; //default hinge check threshold
        private const float SVL_THRESHOLD = 2.56f; //default staccato vs legato threshold
        private const int SVL_WINDOW_SIZE = 100; //default staccato vs legato window size
        private const float BPM_V_THRESHOLD = 0.012f; //default bpm velocity threshold
        private const float BPM_D_THRESHOLD = 0.10f; //default bpm distance threshold
        private const String FILE_PREFIX = "StudentReport"; //title for file name of report generation
        private const Key SAVE_RESULTS_KEY = Key.O;

        // Variables for record/replay/display
        private Kinect.Replay.Record.KinectRecorder newrec; // to handle recording
        private Kinect.Replay.Replay.KinectReplay newrep; // to handle replay
        private bool _isRecording; // recording state (true if recording, false otherwise)
        private bool _isReplaying; // replaying state (true if replaying, false otherwise)
        private StreamWriter file = null;   // for creating .csv file
        private DateTime initialTime; //for displaying length of recording

        // Recording options; currently audio is not being recorded (uncomment end of line to reenable audio)
        private const Kinect.Replay.Record.KinectRecordOptions RecordOptions = Kinect.Replay.Record.KinectRecordOptions.Frames/* | KinectRecordOptions.Audio*/;
        
        //Hand Histories
        private JointHistory rightHandHist = new JointHistory(); //history of data on right hand
        private JointHistory leftHandHist = new JointHistory(); //history of data on left hand

        // Display-related variables
        private string msg; // for displaying messages
        private string mirfdbk; // for displaying mirror feedback
        private string swafdbk; // for displaying swaying feedback
        private string leafdbk; // for displaying leaning feedback
        private string svlfdbk; // for staccato vs legato feedback
        private string hingefdbk; // for hinge movement feedback
        private string bpmfdbk; // for bpm feedback
        private SolidColorBrush mirbrush; //for background of mirroring box
        private SolidColorBrush swabrush; //for background of swaying box
        private SolidColorBrush leabrush; //for background of leaning box
        private SolidColorBrush svlbrush; //for background of s vs l box
        private SolidColorBrush hingebrush; //for background of s vs l box
        private SolidColorBrush bpmbrush; //for background of bpm circle
        private SolidColorBrush goodBrush = new SolidColorBrush(Colors.SpringGreen); //for background of boxes
        private SolidColorBrush badBrush = new SolidColorBrush(Colors.LightPink);
        private SolidColorBrush neutralBrush = new SolidColorBrush(Colors.LightCyan);
        private SolidColorBrush staccatoBrush = new SolidColorBrush(Colors.LightBlue);
        private SolidColorBrush legatoBrush = new SolidColorBrush(Colors.LightYellow);
        private WriteableBitmap colorImageBitmap; // for color/video display
        private Skeleton[] skeletonArray; // for skeletons
        Brush skeletonBrush = new SolidColorBrush(Colors.Red); // for drawing skeleton
        
        // Variables for feedback system
        // Ready/Analysis states
        private bool isAnalyzing = false; // to keep track of analysis state
        private int readyPtr = 0; // to keep track of location in  ready buffer
        private float[] pastReady = new float[20]; // buffer to look for ready state
        private long totalPct = 0; // to keep track of total frames for percentages
        private int studentNo = 1; //keeps track of the number of the student that is being analyzed - in order to output to file

        // Swaying (side to side)
        private float[] pastSwaying = new float[20]; // buffer to check for swaying
        private int swayPtr = 0; // to keep track of location in swaying buffer
        private bool swayFull = false; // to keep track of whether or not the swaying buffer has filled (enough pts for analysis)
        private long swayPct = 0; // to keep track of percentage of frames swaying
        private float swayThreshold = SWAY_THRESHOLD; // threshold of range of motion for no swaying detected

        // Leaning/rocking (forward and back)
        private float[] pastLeaning = new float[20]; // buffer to check for leaning/rocking
        private int leanPtr = 0; // to keep track of location in leaning buffer
        private bool leanFull = false; // to keep track of whether or not leaning buffer filled (enough pts for analysis)
        private long leanPct = 0; // to keep track of percentage of frames leaning
        private float leanThreshold = LEAN_THRESHOLD; // threshold of range of motion for no leaning detected

        // Mirroring
        private int mirrorBuffer = 0; // buffer to check for mirroring
        private long mirrorPct = 0; // to keep track of percentage of frames mirroring
        private float mirrorThreshold = 0.19f; // threshold for difference in mirrored motion between hands
        
        // Staccato vs Legato
        private int windowSizeSlider = SVL_WINDOW_SIZE; // window size - from slider.
        private int windowSize = SVL_WINDOW_SIZE; //window size 
        private long staccatoPct = 0; //percentage staccato
        private int iterationNum = -4; // -3 indicates a start where we have no prior info about accel and velocity.
        private int startPos = 0; // position in array of first value
        private DateTime[] SLTimes = new DateTime[302]; // array that stores the time of recording positions
        // 6 different sections for both - 0 is X for left hand, 1 is Y for left hand, 3 is Z Left, 4 is X Right,
        // 4 is Y Right, 5 is Z Right.
        private float[,] SLVelocs = new float[10,302]; // additional storage for 6(left hand velocity), 7(left hand smoothed velocity),
        //8(right hand velocity), 9(right hand smoothed velocity)
        private bool[,] SLPeaks = new bool[2, 302]; //this array stores whether there is a peak
        private float[,] SLDist = new float[6,302]; // array to store history of hand positions 
        private int lhPeakCount = 0; //number of peaks in left hand
        private int rhPeakCount = 0; //number of peaks in right hand
        private float lhPeakAvg = 0; //avg peak in left hand
        private float rhPeakAvg = 0; //avg peak in right hand
        private float SvLThreshold = SVL_THRESHOLD; // threshold for Legato (less than this - legato, greater than is Staccato)

        //BPM
        private DateTime[] bpmlastBeats = new DateTime[20]; // store the times of recorded beats
        private int bpmPos = 0; // store the position in the array that will be used next
        private int beatCounter = 0; //what beat are we now
        private int framesSinceBeat = 0;//keep track of how many frames since last beat
        private float BPM = 0; //BPM to be displayed
        private float BPMAvg = 0; //moving average of BPM
        private float totalBPM = 0; //total average of BPM
        private float BPMVthreshold = BPM_V_THRESHOLD; //threshold for BPM - velocity
        private float BPMDthreshold = BPM_D_THRESHOLD; //threshold for bpm - distance


        // Hinges
        private float[] pastElbowR = new float[10]; // to keep track of distances moved by right elbow
        private float[] pastElbowL = new float[10]; // to keep track of distances moved by left elbow
        private int hingePtr = 0; // to keep track of position in elbow buffers
        private bool hingeFull = false; // to keep track of whether or not the buffer is filled (enough pts for analysis)
        private SkeletonPoint leLast; // to keep track of the last position of the left elbow
        private SkeletonPoint reLast; // to keep track of the last position of the right elbow
        private long hingePct = 0; // to keep track of percentage of frames with too much hinge movement
        private float hingeThreshold = HINGE_THRESHOLD; // the threshold for detection

        #endregion

        #region Setup

        // Constructor
        public MainWindow()
		{
			InitializeComponent();
		}

		public event PropertyChangedEventHandler PropertyChanged;

        // for displaying messages to top of screen
		public string Message
		{
			get { return msg; }
			set
			{
				if (value.Equals(msg)) return;
				msg = value;
				PropertyChanged.Raise(() => Message);
			}
		}

        // for displaying feedback for mirroring
        public string Mirroring
        {
            get { return mirfdbk; }
            set
            {
                if (value.Equals(mirfdbk)) return;
                mirfdbk = value;
                PropertyChanged.Raise(() => Mirroring);
            }
        }

        // for displaying feedback for swaying
        public string Swaying
        {
            get { return swafdbk; }
            set
            {
                if (value.Equals(swafdbk)) return;
                swafdbk = value;
                PropertyChanged.Raise(() => Swaying);
            }
        }

        // for displaying feedback for leaning
        public string Leaning
        {
            get { return leafdbk; }
            set
            {
                if (value.Equals(leafdbk)) return;
                leafdbk = value;
                PropertyChanged.Raise(() => Leaning);
            }
        }

        // for displaying feedback for Staccato vs Legato
        public string SvsL
        {
            get { return svlfdbk; }
            set
            {
                if (value.Equals(svlfdbk)) return;
                svlfdbk = value;
                PropertyChanged.Raise(() => SvsL);
            }
        }

        // for displaying feedback for Hinge Movement
        public string Hinge
        {
            get { return hingefdbk; }
            set
            {
                if (value.Equals(hingefdbk)) return;
                hingefdbk = value;
                PropertyChanged.Raise(() => Hinge);
            }
        }

        //for displaying BPM
        public string BPMFB
        {
            get { return bpmfdbk; }
            set
            {
                if (value.Equals(bpmfdbk)) return;
                bpmfdbk = value;
                PropertyChanged.Raise(() => BPMFB);
            }
        }
        //Mirroing Background brush
        public SolidColorBrush MirroringBG
        {
            get { return mirbrush; }
            set 
            {
                if (value.Equals(mirbrush)) return;
                mirbrush = value;
                PropertyChanged.Raise(() => MirroringBG);
            }
        }
        //Swaying Background brush
        public SolidColorBrush SwayingBG
        {
            get { return swabrush; }
            set
            {
                if (value.Equals(swabrush)) return;
                swabrush = value;
                PropertyChanged.Raise(() => SwayingBG);
            }
        }
        //Leaning Background brush
        public SolidColorBrush LeaningBG
        {
            get { return leabrush; }
            set
            {
                if (value.Equals(leabrush)) return;
                leabrush = value;
                PropertyChanged.Raise(() => LeaningBG);
            }
        }
        //Staccato vs Legato background brush
        public SolidColorBrush SvsLBG
        {
            get { return svlbrush; }
            set
            {
                if (value.Equals(svlbrush)) return;
                svlbrush = value;
                PropertyChanged.Raise(() => SvsLBG);
            }
        }

        public SolidColorBrush HingeBG
        {
            get { return hingebrush; }
            set
            {
                if (value.Equals(hingebrush)) return;
                hingebrush = value;
                PropertyChanged.Raise(() => HingeBG);
            }
        }

        //BPM Circle brush
        public SolidColorBrush BPMBG
        {
            get { return bpmbrush; }
            set
            {
                if (value.Equals(bpmbrush)) return;
                bpmbrush = value;
                PropertyChanged.Raise(() => BPMBG);
            }
        }

        // update video image
		public WriteableBitmap ImageSource
		{
			get { return colorImageBitmap; }
			set
			{
				if (value.Equals(colorImageBitmap)) return;
				colorImageBitmap = value;
				PropertyChanged.Raise(() => ImageSource);
			}
		}

        // Check if Kinect is present/connected
		public bool KinectPresent
		{
			get { return _kinectPresent; }
			set
			{
				if (value.Equals(_kinectPresent)) return;
				_kinectPresent = value;
				PropertyChanged.Raise(() => KinectPresent);
			}
		}

        // set recording state
		public bool IsRecording
		{
			get { return _isRecording; }
			set
			{
				if (value.Equals(_isRecording)) return;
				_isRecording = value;
				PropertyChanged.Raise(() => IsRecording);
			}
		}

        // set replaying state
		public bool IsReplaying
		{
			get { return _isReplaying; }
			set
			{
				if (value.Equals(_isReplaying)) return;
				_isReplaying = value;
				PropertyChanged.Raise(() => IsReplaying);
			}
		}

        // Checks for Kinect and initializes components if found; otherwise returns error message
		private void MainWindowLoaded(object sender, RoutedEventArgs e)
		{
			try
			{
				KinectSensor.KinectSensors.StatusChanged += KinectSensorsStatusChanged;

				myKinect = KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
				if (myKinect == null)
				{
					Message = "No Kinect found on startup";
					KinectPresent = false;
				}
				else
					Initialize();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

        // Report Kinect connection status; displays message appropriate to status
		void KinectSensorsStatusChanged(object sender, StatusChangedEventArgs e)
		{
			switch (e.Status)
			{
				case KinectStatus.Disconnected:
					if (myKinect == e.Sensor)
					{
						Clean();
						Message = "Kinect disconnected";
					}
					break;
				case KinectStatus.Connected:
					myKinect = e.Sensor;
					Initialize();
					break;
				case KinectStatus.NotPowered:
					Message = "Kinect is not powered";
					Clean();
					break;
				case KinectStatus.NotReady:
					Message = "Kinect is not ready";
					break;
				case KinectStatus.Initializing:
					Message = "Initializing";
					break;
				default:
					Message = string.Concat("Status: ", e.Status);
					break;
			}
		}

        // Release memory at conclusion of program
		private void Clean()
		{
			KinectPresent = false;
			if (newrec != null && IsRecording)
				newrec.Stop();
			if (newrep != null)
			{
				newrep.Stop();
				newrep.Dispose();
			}
			if (myKinect == null)
				return;
			if (myKinect.IsRunning)
				myKinect.Stop();
			myKinect.AllFramesReady -= KinectSensorAllFramesReady;
			myKinect.Dispose();
			myKinect = null;
		}

        // initialize components if the Kinect is connected
        // currently depth and audio streams are disabled
        private void Initialize()
        {
            if (myKinect == null)
                return;

            myKinect.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            myKinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            myKinect.SkeletonStream.Enable();
            // Uncomment the following to reenable depth
            //myKinect.DepthStream.Enable();
            myKinect.Start();
            // Uncomment the following to reenable audio
            //myKinect.AudioSource.Start();
            Message = "Kinect connected";
            KinectPresent = true;
            myKinect.AllFramesReady += KinectSensorAllFramesReady;
            for (int i = 0; i < 302; i++)
            {
                SLPeaks[0, i] = false;
                SLPeaks[1, i] = false;
            }
            MirroringBG = neutralBrush;
            SwayingBG = neutralBrush;
            LeaningBG = neutralBrush;
            SvsLBG = neutralBrush;
            HingeBG = neutralBrush;
            initialTime = DateTime.Now;
        }

        // check for changed properties
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        // Checks that all frames are ready to be sent/updated
        // Depth is currently disabled (and must be reenabled in setup to reenable here)
		void KinectSensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
		{
			if (newrep != null && !newrep.IsFinished)
				return;

            using (var frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    if (newrec != null)
                        newrec.Record(frame);
                    UpdateColorFrame(frame);
                }
            }

            // Uncomment the following to reenable depth
            //using (var frame = e.OpenDepthImageFrame())
            //{
            //    if (frame != null)
            //    {
            //        if (newrec != null)
            //            newrec.Record(frame);
            //    }
            //}

			using (var frame = e.OpenSkeletonFrame())
			{
				if (frame != null)
				{
					if (newrec != null)
						newrec.Record(frame);
					UpdateSkeletons(frame);
				}
			}
		}

        #endregion

        #region Color
        // update video image on screen
        private void UpdateColorFrame(ReplayColorImageFrame frame)
		{
			var pixelData = new byte[frame.PixelDataLength];
			frame.CopyPixelDataTo(pixelData);
			if (ImageSource == null)
				ImageSource = new WriteableBitmap(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null);

			var stride = frame.Width * PixelFormats.Bgr32.BitsPerPixel / 8;
			ImageSource.WritePixels(new Int32Rect(0, 0, frame.Width, frame.Height), pixelData, stride, 0);
		}

        #endregion

        #region Skeleton
        // update skeleton image on screen, display status of joints being tracked
        // if recording, create and output a file of joint coordinates
        private void UpdateSkeletons(ReplaySkeletonFrame frame)
		{
			string coordinates = ""; // for writing data to .csv file
            Brush brush = new SolidColorBrush(Colors.Red); // for drawing skeleton
            skeletonArray = frame.Skeletons; // get array of tracked skeletons
            long time = frame.TimeStamp; // get timestamp for the frame
            Skeleton trackedSkeleton = null; // skeleton of the conductor

            // get first skeleton, if available
            if (skeletonArray != null)
                trackedSkeleton = skeletonArray.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);

            // if the first skeleton is null, return
			if (trackedSkeleton == null)
				return;

            // if valid skeleton exists, get all data for all tracked joints
            if (trackedSkeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                // Get all necessary joints (from the waist up)
                Joint leftHand = trackedSkeleton.Joints[JointType.HandLeft];
                SkeletonPoint lhposition = leftHand.Position;
                Joint rightHand = trackedSkeleton.Joints[JointType.HandRight];
                SkeletonPoint rhposition = rightHand.Position;
                Joint leftWrist = trackedSkeleton.Joints[JointType.WristLeft];
                SkeletonPoint lwposition = leftWrist.Position;
                Joint rightWrist = trackedSkeleton.Joints[JointType.WristRight];
                SkeletonPoint rwposition = rightWrist.Position;
                Joint leftElbow = trackedSkeleton.Joints[JointType.ElbowLeft];
                SkeletonPoint leposition = leftElbow.Position;
                Joint rightElbow = trackedSkeleton.Joints[JointType.ElbowRight];
                SkeletonPoint reposition = rightElbow.Position;
                Joint leftShoulder = trackedSkeleton.Joints[JointType.ShoulderLeft];
                SkeletonPoint lsposition = leftShoulder.Position;
                Joint rightShoulder = trackedSkeleton.Joints[JointType.ShoulderRight];
                SkeletonPoint rsposition = rightShoulder.Position;
                Joint centerShoulder = trackedSkeleton.Joints[JointType.ShoulderCenter];
                SkeletonPoint csposition = centerShoulder.Position;
                Joint head = trackedSkeleton.Joints[JointType.Head];
                SkeletonPoint hposition = head.Position;

                // check if subject is ready for feedback/analysis
                if (!isAnalyzing)
                {
                    checkReady(lhposition, rhposition);

                    // If cutoff, display message
                    Message = "Skeleton Status: ";
                    if (head.TrackingState != JointTrackingState.NotTracked)
                    {
                        if (head.TrackingState == JointTrackingState.Inferred)
                        {
                            Message += "Head is not visible.";
                        }
                    }
                    if (leftHand.TrackingState == JointTrackingState.Tracked && rightHand.TrackingState == JointTrackingState.Tracked)
                    {
                        Message += "Good Quality.";
                    }
                    else
                    {
                        Message += "Please recenter the subject.";
                    }

                    Message += "Awaiting ready state";
                }

                // Send data to be analyzed
                else
                {
                    Message = "Running analysis";
                    totalPct++; // increment total percentage counter
                    isMirroring(rhposition, lhposition, csposition); // check for mirroring
                    isSwaying(csposition); // check for swaying
                    isLeaning(hposition); // check for leaning/rocking
                    hingeCheck(reposition, leposition); // check for excessive hinge movement
                    rightHandHist.addData(rhposition.X, rhposition.Y, rhposition.Z);
                    //leftHandHist.addData(lhposition.X, lhposition.Y, lhposition.Z);
                    StaccatoLegato(rhposition, lhposition); //Check Stacc vs Leg.
                    beatsPM(rightHandHist); // Calculate beats per min
                }

                // Create string to output coordinates to file if recording
                if (_isRecording && file != null)
                {
                    coordinates = time.ToString() + ",";
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", lhposition.X, lhposition.Y, lhposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", rhposition.X, rhposition.Y, rhposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", lwposition.X, lwposition.Y, lwposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", rwposition.X, rwposition.Y, rwposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", leposition.X, leposition.Y, leposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", reposition.X, reposition.Y, reposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", lsposition.X, lsposition.Y, lsposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", rsposition.X, rsposition.Y, rsposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", csposition.X, csposition.Y, csposition.Z);
                    coordinates += string.Format("{0:0.00},{1:0.00},{2:0.00},", hposition.X, hposition.Y, hposition.Z);

                    file.WriteLine(coordinates);
                }

                // draw skeleton
                SkeletonCanvas.Children.Clear(); // clear last skeleton
                // Spine
                addLine(head, centerShoulder);

                // Left arm
                addLine(centerShoulder, leftShoulder);
                addLine(leftShoulder, leftElbow);
                addLine(leftElbow, leftWrist);
                addLine(leftWrist, leftHand);

                // Right arm
                addLine(centerShoulder, rightShoulder);
                addLine(rightShoulder, rightElbow);
                addLine(rightElbow, rightWrist);
                addLine(rightWrist, rightHand);
            } // end skeleton tracked if statement
              // For some reason, even after people have left the screen, the previously tracked skeleton still reports back as tracked
              // This is a problem with the API.  Else statements here are never entered as a result.
		}

        // draw lines to represent skeleton and joints
        private void addLine(Joint j1, Joint j2)
        {
            // only draw lines if the joints are being tracked and not inferred
            if (j1.TrackingState == JointTrackingState.Tracked && j2.TrackingState == JointTrackingState.Tracked)
            {
                Line boneLine = new Line();
                boneLine.Stroke = skeletonBrush;
                boneLine.StrokeThickness = 5;
                ColorImagePoint j1P, j2P;

                if (!_isReplaying)
                {
                    j1P = myKinect.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
                    j2P = myKinect.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
                }
                else // play skeleton from replay file
                {
                    j1P = newrep.CoordinateMapper.MapSkeletonPointToColorPoint(j1.Position, ColorImageFormat.RgbResolution640x480Fps30);
                    j2P = newrep.CoordinateMapper.MapSkeletonPointToColorPoint(j2.Position, ColorImageFormat.RgbResolution640x480Fps30);
                }

                boneLine.X1 = j1P.X;
                boneLine.Y1 = j1P.Y;
                boneLine.X2 = j2P.X;
                boneLine.Y2 = j2P.Y;

                SkeletonCanvas.Children.Add(boneLine);
            }
        }

        #endregion

        #region Buttons

        // If recording button is clicked, start recording.  If already recording, stop.
        // Audio recording is currently disabled
        private void RecordClick(object sender, RoutedEventArgs e)
		{
			if (IsRecording)
			{
				newrec.Stop();
                save_Click(null, null);
				newrec = null;
				IsRecording = false;
				Message = "";
				return;
			}
			//var saveFileDialog = new SaveFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };
			//if (saveFileDialog.ShowDialog() != true) return;
            String filename = FILE_PREFIX + studentNo.ToString("D3");

			newrec = new KinectRecorder(RecordOptions, (filename + ".replay"), myKinect);
			Message = string.Format("Recording {0}", RecordOptions.ToString());

            file = new StreamWriter(filename + ".csv", true);

            // Creates the headers for the .csv file
            string header = "Time Interval,Left Hand X,Left Hand Y,Left Hand Z,Right Hand X,Right Hand Y,Right Hand Z,";
            header += "Left Wrist X,Left Wrist Y,Left Wrist Z,Right Wrist X,Right Wrist Y,Right Wrist Z,";
            header += "Left Elbow X,Left Elbow Y,Left Elbow Z,Right Elbow X,Right Elbow Y,Right Elbow Z,";
            header += "Left Shoulder X,Left Shoulder Y,Left Shoulder Z,Right Shoulder X,Right Shoulder Y,Right Shoulder Z,";
            header += "Center Shoulder X,Center Shoulder Y,Center Shoulder Z,Head X,Head Y,Head Z";
            file.WriteLine(header);
            reset_Click(null, null);
            // Uncomment the following to reenable audio
			//newrec.StartAudioRecording();
			IsRecording = true;
		}

        // if replay button is clicked, bring up file selection and start replaying.
        // if already replaying, stop and reset all percentage counters and ready state.
        // While replaying, also create an output file of joint coordinates
		private void ReplayClick(object sender, RoutedEventArgs e)
		{
			// reset analysis numbers
            isAnalyzing = false; // to keep track of analysis state
            readyPtr = 0; // to keep track of location in  ready buffer
            totalPct = 0; // to keep track of total frames for percentages

            swayPtr = 0; // to keep track of location in swaying buffer
            swayFull = false; // to keep track of whether or not the swaying buffer has filled (enough pts for analysis)
            swayPct = 0; // to keep track of percentage of frames swaying

            // Leaning/rocking (forward and back)
            leanPtr = 0; // to keep track of location in leaning buffer
            leanFull = false; // to keep track of whether or not leaning buffer filled (enough pts for analysis)
            leanPct = 0; // to keep track of percentage of frames leaning

            // Mirroring
            mirrorBuffer = 0;
            mirrorPct = 0;
        
            // Hinges
            hingePtr = 0;
            hingeFull = false;
            hingePct = 0; // to keep track of percentage of frames with too much hinge movement

            Message = "Awaiting ready state";

            if (IsReplaying)
			{
				CleanupReplay();
				Message = "";
				return;
			}
            // Uncomment the following to reenable audio
            //_startedAudio = false;
			var openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };

			if (openFileDialog.ShowDialog() == true)
			{
                Message = "Opening file...";
                newrep = new KinectReplay(openFileDialog.FileName);
                Message = string.Format("Replaying {0}", RecordOptions.ToString());
				newrep.AllFramesReady += ReplayAllFramesReady;
				newrep.ReplayFinished += CleanupReplay;
				newrep.Start();
                IsReplaying = true;
			}
			
		}

        // return memory used for replay upon completion of replay
		private void CleanupReplay()
		{
			if (!IsReplaying) return;
			Message = "";
            // Uncomment the following to reenable audio
            //if(_soundPlayer!=null && _startedAudio)
            //    _soundPlayer.Stop();

            newrep.AllFramesReady -= ReplayAllFramesReady;
			newrep.Stop();
			newrep.Dispose();
			newrep = null;
            IsReplaying = false;
            SkeletonCanvas.Children.Clear();
            if(file != null)
                file.Close();
		}

        // Choose which frames/streams to replay.  Audio is currently disabled.  Uncomment to reenable
		void ReplayAllFramesReady(ReplayAllFramesReadyEventArgs e)
		{
            // Uncomment the following to reenable audio
            //if ((newrep.Options & KinectRecordOptions.Audio) != 0 && !_startedAudio)
            //{
            //    _soundPlayer = new SoundPlayer(newrep.AudioFilePath);
            //    _soundPlayer.Play();
            //    _startedAudio = true;
            //}

            var colorImageFrame = e.AllFrames.ColorImageFrame;
            if (colorImageFrame != null)
                UpdateColorFrame(colorImageFrame);

			var skeletonFrame = e.AllFrames.SkeletonFrame;
			if (skeletonFrame != null)
				UpdateSkeletons(skeletonFrame);
		}

        // reset ready state, all percentages, and clear skeleton
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            isAnalyzing = false; // to keep track of analysis state
            readyPtr = 0; // to keep track of location in  ready buffer
            totalPct = 0; // to keep track of total frames for percentages
            pastReady = new float[20]; //past ready
            
            //Swaying 
            swayPtr = 0; // to keep track of location in swaying buffer
            swayFull = false; // to keep track of whether or not the swaying buffer has filled (enough pts for analysis)
            swayPct = 0; // to keep track of percentage of frames swaying
            swayThreshold = SWAY_THRESHOLD;
            Swaying = "";

            // Leaning/rocking (forward and back)
            leanPtr = 0; // to keep track of location in leaning buffer
            leanFull = false; // to keep track of whether or not leaning buffer filled (enough pts for analysis)
            leanPct = 0; // to keep track of percentage of frames leaning
            leanThreshold = LEAN_THRESHOLD;
            Leaning = "";

            // Mirroring
            mirrorBuffer = 0;
            mirrorPct = 0;
            mirrorThreshold = MIRRORING_THRESHOLD;
            Mirroring = "";

            // Hinges
            hingePtr = 0;
            hingeFull = false;
            hingePct = 0; // to keep track of percentage of frames with too much hinge movement
            hingeThreshold = HINGE_THRESHOLD;
            Hinge = "";

            //SVL
            windowSizeSlider = SVL_WINDOW_SIZE;
            windowSize = SVL_WINDOW_SIZE;
            iterationNum = -4;
            startPos = 0; 
            lhPeakCount = 0;
            rhPeakCount = 0;
            lhPeakAvg = 0;
            rhPeakAvg = 0;
            staccatoPct = 0;
            SLVelocs = new float[10,302];
            SLDist = new float[6,302];
            SLTimes = new DateTime[302];
            for (int i = 0; i < 302; i++)
            {
                SLPeaks[0, i] = false;
                SLPeaks[1, i] = false;
            }
            SvsL = "";
            SvLThreshold = SVL_THRESHOLD;

            //BPM
            bpmlastBeats = new DateTime[20];
            bpmPos = 0;
            beatCounter = 0; 
            framesSinceBeat = 0;
            BPM = 0;
            BPMFB = "";
            totalBPM = 0;
            BPMAvg = 0; 
            BPMVthreshold = BPM_V_THRESHOLD; 
            BPMDthreshold = BPM_D_THRESHOLD; 

            //Brushes
            MirroringBG = neutralBrush;
            SwayingBG = neutralBrush;
            LeaningBG = neutralBrush;
            SvsLBG = neutralBrush;
            HingeBG = neutralBrush;

            SkeletonCanvas.Children.Clear(); // clear skeleton on canvas

            Message = "Awaiting ready state";
        }

        // tilt Kinect upward slightly if Tilt Up button clicked
        // This will create an exception if the Kinect motors do not respond,
        // which can be caused by the cooldown period.
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myKinect.ElevationAngle += 10;
            }
            catch
            {
                MessageBox.Show("An error has occurred. See documentation for details.");
            }
        }

        // tilt Kinect down slightly if Tilt Down button clicked
        // This will create an exception if the Kinect motors do not respond,
        // which can be caused by the cooldown period.
        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myKinect.ElevationAngle -= 10;
            }
            catch
            {
                MessageBox.Show("An error has occurred. See documentation for details.");
            }
        }

        //open settings window
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            //Convert all of the current settings into percentage integers to be used with the sliders
            int WSSmult = Convert.ToInt32(windowSizeSlider * 100 / SVL_WINDOW_SIZE);
            int MTmult = Convert.ToInt32(mirrorThreshold * 100 / MIRRORING_THRESHOLD);
            int STmult = Convert.ToInt32(swayThreshold * 100 / SWAY_THRESHOLD);
            int LTmult = Convert.ToInt32(leanThreshold * 100 / LEAN_THRESHOLD);
            int HTmult = Convert.ToInt32(hingeThreshold * 100 / HINGE_THRESHOLD);
            int SVLTmult = Convert.ToInt32(SvLThreshold * 100 / SVL_THRESHOLD);
            int BVTmult = Convert.ToInt32(BPMVthreshold * 100 / BPM_V_THRESHOLD);
            int BDTmult = Convert.ToInt32(BPMDthreshold * 100 / BPM_D_THRESHOLD);

            SettingsForm settings = new SettingsForm(WSSmult, MTmult, STmult, LTmult, HTmult, SVLTmult, BVTmult, BDTmult);
            settings.ShowDialog();

            //Convert the percentage integers back into the float thresholds
            windowSizeSlider = settings.windowSize;
            mirrorThreshold = MIRRORING_THRESHOLD * ((float)settings.mirThresh) / 100;
            swayThreshold = SWAY_THRESHOLD * ((float)settings.swayThresh) / 100;
            leanThreshold = LEAN_THRESHOLD * ((float)settings.leanThresh) / 100;
            hingeThreshold = HINGE_THRESHOLD * ((float)settings.hingeThresh) / 100;
            SvLThreshold = SVL_THRESHOLD * ((float)settings.svlThresh) / 100;
            BPMVthreshold = BPM_V_THRESHOLD * ((float)settings.bpmvThresh) / 100;
            BPMDthreshold = BPM_D_THRESHOLD * ((float)settings.bpmdThresh) / 100;
        }
        #endregion

        #region Feedback System

        // Checks to see if user is ready by looking at hand positions and seeing if user is still
        // sets ready state to true if user is still so that analysis can begin
        private void checkReady(SkeletonPoint lh, SkeletonPoint rh)
        {
            // get squared distance and add to buffer; only until buffer is full
            if (readyPtr != pastReady.Length)
            {
                float sqdist = (lh.X - rh.X) * (lh.X - rh.X) + (lh.Y - rh.Y) * (lh.Y - rh.Y) + (lh.Z - rh.Z) * (lh.Z - rh.Z);
                pastReady[readyPtr] = sqdist;
                readyPtr++;
            }
            else // if buffer has been filled already
            {
                // if distance hasn't changed
                if (pastReady.Max() - pastReady.Min() < 0.05)
                {
                    isAnalyzing = true;
                }
                else // start over
                {
                    readyPtr = 0;
                }
            }
        }

        // Detects if swaying is occurring and displays a message if so.
        private void isSwaying(SkeletonPoint centerShoulder)
        {


            double percent = (double)swayPct / totalPct * 100;
            Swaying = "Swaying:  " + string.Format("{0:0.0}", percent) + "%";

            // store new X coordinate
            pastSwaying[swayPtr] = centerShoulder.X;
            swayPtr++; // point to next empty spot

            // check for full buffer; if full, go back to beginning and overwrite oldest value
            if (swayPtr == pastSwaying.Length)
            {
                swayPtr = 0;

                // buffer has been filled --> enough points to start analyzing if not already
                if(!swayFull)
                    swayFull = true;
            }

            // if enough points to do analysis, look to see if range of motion is above threshold
            if (swayFull)
            {
                if (Math.Abs(pastSwaying.Max() - pastSwaying.Min()) > swayThreshold) //swaying is occuring
                {
                    SwayingBG = badBrush;
                    swayPct++;
                }

                else { SwayingBG = goodBrush; }
            }

        }

        // Detects if leaning/rocking is occurring and displays a message if so.
        private void isLeaning(SkeletonPoint head)
        {
            double percent = (double)leanPct / totalPct * 100;
            Leaning = "Rocking: " + string.Format("{0:0.0}", percent) + "%";

            // store new X coordinate
            pastLeaning[leanPtr] = head.Z;
            leanPtr++; // point to next empty spot

            // check for full buffer; if full, go back to beginning and overwrite oldest value
            if (leanPtr == pastLeaning.Length)
            {
                leanPtr = 0;

                // buffer has been filled --> enough points to start analyzing if not already
                if(!leanFull)
                    leanFull = true;
            }

            // if enough points to do analysis, look to see if range of motion is above threshold
            if (leanFull)
            {
                if (Math.Abs(pastLeaning.Max() - pastLeaning.Min()) > leanThreshold)
                {
                    LeaningBG = badBrush;
                    leanPct++;
                }
                else { LeaningBG = goodBrush; }
            }
        }

        // Detects if mirroring of hands is occurring and displays a message if so
        // also displays percentage of time spent mirroring out of total running time
        private void isMirroring(SkeletonPoint rh, SkeletonPoint lh, SkeletonPoint cs)
        {
            // Display mirroring percentage
            double percent = (double) mirrorPct / totalPct * 100;
            Mirroring = "Mirroring: " + string.Format("{0:0.0}",percent) + "%";
            
            float diffX = Math.Abs(lh.X - (cs.X - rh.X + cs.X)); // left hand X - reflected right hand X
            float diffY = Math.Abs(lh.Y - rh.Y);
            float diffZ = Math.Abs(lh.Z - rh.Z);

            if ((diffX < mirrorThreshold) && (diffY < mirrorThreshold) && (diffZ < mirrorThreshold))
            {
                mirrorBuffer++;

                if (mirrorBuffer > 30) // if user has been mirroring consistently
                {
                    MirroringBG = badBrush;
                    mirrorPct++; // increment mirroring percentage counter
                }
            }
            else // user is not mirroring
            {
                MirroringBG = goodBrush;
                mirrorBuffer = 0; // reset buffer
            }
        }

        private void StaccatoLegato(SkeletonPoint rh, SkeletonPoint lh)
        /* This method stores and calculates all data relevant to Staccato vs Legato, then uses it to determine whether the user is conducting in staccato or legato.
         * Parameters:
         *  SkeletonPoint rh: The right hand coordinates
         *  SkeletonPoint lh: The left hand coordinates
         * Results:
         *  The UI is updated with information about Staccato vs Legato. Update brushes, display relevant debug information.
         * Notes:
         *  There is a lot of cleaning up to do here, need to implement JointHistory instead of calculating everything here.*/
        {
            int currentPos = startPos + iterationNum; //most recent position
            int lastPos = currentPos - 1; //most recent position - 1
            int lastPos2 = currentPos - 2; //most recent - 2
            int lastPos3 = currentPos - 3; // most recent - 3

            if (currentPos > windowSize) //if the current position is outside of the array, wrap around to the beginning. 
            {
                currentPos = currentPos - windowSize;
                if(windowSize != windowSizeSlider) //if the slider is at a different position, update the window size
                {
                    windowSize = windowSizeSlider;
                }
                iterationNum = 0;
            }

            else if (iterationNum == -4) //If the iteration is -4, we can't calculate anything, can only store displacement of hands
            {
                //Store the position for each of the coordinates
                SLDist[0, 0] = lh.X; 
                SLDist[1, 0] = lh.Y;
                SLDist[2, 0] = lh.Z;
                SLDist[3, 0] = rh.X;
                SLDist[4, 0] = rh.Y;
                SLDist[5, 0] = rh.Z;
                SLTimes[0] = DateTime.Now; //Store the time that the points were collected
            }

            else if (iterationNum == -3) //If the iteration is -3, we can calculate velocity the first entry
            {
                //Store position
                SLDist[0, 1] = lh.X;
                SLDist[1, 1] = lh.Y;
                SLDist[2, 1] = lh.Z;
                SLDist[3, 1] = rh.X;
                SLDist[4, 1] = rh.Y;
                SLDist[5, 1] = rh.Z;
                SLTimes[1] = DateTime.Now; //Store time the data was collected
                int timeDiff = SLTimes[1].Subtract(SLTimes[0]).Milliseconds; //Calculate time difference
                
                //Store calculated velocities
                SLVelocs[0, 1] = calcVeloc(SLDist[0, 1], SLDist[0, 0], timeDiff); //lh x
                SLVelocs[1, 1] = calcVeloc(SLDist[1, 1], SLDist[1, 0], timeDiff); //lh y
                SLVelocs[2, 1] = calcVeloc(SLDist[2, 1], SLDist[2, 0], timeDiff); //lh z
                SLVelocs[3, 1] = calcVeloc(SLDist[3, 1], SLDist[3, 0], timeDiff); //rh x
                SLVelocs[4, 1] = calcVeloc(SLDist[4, 1], SLDist[4, 0], timeDiff); //rh y
                SLVelocs[5, 1] = calcVeloc(SLDist[5, 1], SLDist[5, 0], timeDiff); //rh z

                //Store composite velocity
                SLVelocs[6, 1] = calcTotalVeloc(SLVelocs[0, 1], SLVelocs[1, 1], SLVelocs[2, 1]);
                SLVelocs[8, 1] = calcTotalVeloc(SLVelocs[3, 1], SLVelocs[4, 1], SLVelocs[5, 1]);

            }

            else if (iterationNum == -2) //If the iteration is -2, we can calculate everything but smoothed velocity and peaks
            {
                //Assign values into the distance array
                SLDist[0, 2] = lh.X;
                SLDist[1, 2] = lh.Y;
                SLDist[2, 2] = lh.Z;
                SLDist[3, 2] = rh.X;
                SLDist[4, 2] = rh.Y;
                SLDist[5, 2] = rh.Z;

                //Assign time information
                SLTimes[2] = DateTime.Now;
                int timeDiff = SLTimes[2].Subtract(SLTimes[1]).Milliseconds;

                //Assign values into the velocity array
                SLVelocs[0, 2] = calcVeloc(SLDist[0, 2], SLDist[0, 1], timeDiff); //lh x
                SLVelocs[1, 2] = calcVeloc(SLDist[1, 2], SLDist[1, 1], timeDiff); //lh y
                SLVelocs[2, 2] = calcVeloc(SLDist[2, 2], SLDist[2, 1], timeDiff); //lh z
                SLVelocs[3, 2] = calcVeloc(SLDist[3, 2], SLDist[3, 1], timeDiff); //rh x
                SLVelocs[4, 2] = calcVeloc(SLDist[4, 2], SLDist[4, 1], timeDiff); //rh y
                SLVelocs[5, 2] = calcVeloc(SLDist[5, 2], SLDist[5, 1], timeDiff); //rh z
                
                //Store composite velocity
                SLVelocs[6, 2] = calcTotalVeloc(SLVelocs[0, 2], SLVelocs[1, 2], SLVelocs[2, 2]);
                SLVelocs[8, 2] = calcTotalVeloc(SLVelocs[3, 2], SLVelocs[4, 2], SLVelocs[5, 2]);
            }

            else if (iterationNum == -1)  //If the iteration is -1, we can calculate everything but peaks
            {
                //Assign values into the distance array
                SLDist[0, 3] = lh.X;
                SLDist[1, 3] = lh.Y;
                SLDist[2, 3] = lh.Z;
                SLDist[3, 3] = rh.X;
                SLDist[4, 3] = rh.Y;
                SLDist[5, 3] = rh.Z;

                //Assign time information
                SLTimes[3] = DateTime.Now;
                int timeDiff = SLTimes[3].Subtract(SLTimes[2]).Milliseconds;

                //Assign values into the velocity array
                SLVelocs[0, 3] = calcVeloc(SLDist[0, 3], SLDist[0, 2], timeDiff); //lh x
                SLVelocs[1, 3] = calcVeloc(SLDist[1, 3], SLDist[1, 2], timeDiff); //lh y
                SLVelocs[2, 3] = calcVeloc(SLDist[2, 3], SLDist[2, 2], timeDiff); //lh z
                SLVelocs[3, 3] = calcVeloc(SLDist[3, 3], SLDist[3, 2], timeDiff); //rh x
                SLVelocs[4, 3] = calcVeloc(SLDist[4, 3], SLDist[4, 2], timeDiff); //rh y
                SLVelocs[5, 3] = calcVeloc(SLDist[5, 3], SLDist[5, 2], timeDiff); //rh z
                //(Left Hand) Store composite velocity and smoothed velocity for previous number
                SLVelocs[6, 3] = calcTotalVeloc(SLVelocs[0, 3], SLVelocs[1, 3], SLVelocs[2, 3]);
                SLVelocs[7, 2] = calcSmoothed(SLVelocs[6, 1], SLVelocs[6, 2], SLVelocs[6, 3]);
                //(Right Hand) Store composite velocity and smoothed velocity for previous number
                SLVelocs[8, 3] = calcTotalVeloc(SLVelocs[3, 3], SLVelocs[4, 3], SLVelocs[5, 3]);
                SLVelocs[9, 2] = calcSmoothed(SLVelocs[8, 1], SLVelocs[8, 2], SLVelocs[8, 3]);
                startPos = 4;
            }

            else //If the iteration is 0 or more, we can calculate everything
            {
                //Assign values into the distance array
                SLDist[0, currentPos] = lh.X;
                SLDist[1, currentPos] = lh.Y;
                SLDist[2, currentPos] = lh.Z;
                SLDist[3, currentPos] = rh.X;
                SLDist[4, currentPos] = rh.Y;
                SLDist[5, currentPos] = rh.Z;

                //Assign time information
                SLTimes[currentPos] = DateTime.Now;
                int timeDiff = SLTimes[currentPos].Subtract(SLTimes[lastPos]).Milliseconds;

                //Assign values into the velocity array
                SLVelocs[0, currentPos] = calcVeloc(SLDist[0, currentPos], SLDist[0, lastPos], timeDiff); //lh x
                SLVelocs[1, currentPos] = calcVeloc(SLDist[1, currentPos], SLDist[1, lastPos], timeDiff); //lh y
                SLVelocs[2, currentPos] = calcVeloc(SLDist[2, currentPos], SLDist[2, lastPos], timeDiff); //lh z
                SLVelocs[3, currentPos] = calcVeloc(SLDist[3, currentPos], SLDist[3, lastPos], timeDiff); //rh x
                SLVelocs[4, currentPos] = calcVeloc(SLDist[4, currentPos], SLDist[4, lastPos], timeDiff); //rh y
                SLVelocs[5, currentPos] = calcVeloc(SLDist[5, currentPos], SLDist[5, lastPos], timeDiff); //rh z

                //(Left Hand) Store composite velocity, smoothed velocity for previous number, peaks
                SLVelocs[6, currentPos] = calcTotalVeloc(SLVelocs[0, currentPos], SLVelocs[1, currentPos], SLVelocs[2, currentPos]);
                SLVelocs[7, lastPos] = calcSmoothed(SLVelocs[6,lastPos2],SLVelocs[6,lastPos],SLVelocs[6,currentPos]);
                SLPeaks[0, lastPos] = isPeak(SLVelocs[7, lastPos3], SLVelocs[7, lastPos2], SLVelocs[7, lastPos], SLVelocs[7, currentPos]);

                //(Right Hand) Store composite velocity and smoothed velocity for previous number
                SLVelocs[8, currentPos] = calcTotalVeloc(SLVelocs[3, currentPos], SLVelocs[4, currentPos], SLVelocs[5, currentPos]);
                SLVelocs[9, lastPos] = calcSmoothed(SLVelocs[8, lastPos2], SLVelocs[8, lastPos], SLVelocs[8, currentPos]);
                SLPeaks[1, lastPos] = isPeak(SLVelocs[9, lastPos3], SLVelocs[9, lastPos2], SLVelocs[9, lastPos], SLVelocs[9, currentPos]);
                
                if(SLPeaks[0,lastPos] == true) //if there is a peak at the most recent calculated position for the left hand, increase the peak height average
                {
                    lhPeakCount++;
                    if (lhPeakCount != 0)
                        lhPeakAvg = (lhPeakAvg * (lhPeakCount - 1) + SLVelocs[7, lastPos]) / lhPeakCount;
                    else
                        lhPeakAvg = 0;
                }

                if (SLPeaks[1, lastPos] == true) //if there is a peak at the most recent calculated position for the right hand, increase the peak height average
                {
                    rhPeakCount++;
                    if (rhPeakCount != 0)
                        rhPeakAvg = (rhPeakAvg * (rhPeakCount - 1) + SLVelocs[9, lastPos]) / rhPeakCount;
                    else
                        rhPeakAvg = 0;
                }

                if (SLPeaks[0, currentPos] == true) //if there is a peak at the position that will be overwritten for the left hand, take it out of the average
                {
                    lhPeakCount--;
                    if (lhPeakCount != 0)
                        lhPeakAvg = (lhPeakAvg * (lhPeakCount + 1) - SLVelocs[7, currentPos]) / lhPeakCount;
                    else
                        lhPeakAvg = 0;
                }

                if (SLPeaks[1, currentPos] == true) //if there is a peak at the position that will be overwritten for the right hand, take it out of the average
                {
                    rhPeakCount--;
                    if (rhPeakCount != 0)
                        rhPeakAvg = (rhPeakAvg * (rhPeakCount + 1) - SLVelocs[9, currentPos]) / rhPeakCount;
                    else
                        rhPeakAvg = 0;
                }
                //Below is debug output
                //SvsL = "Pk: " + SLPeaks[0, lastPos] + "/" + SLPeaks[1, lastPos] + " #Pks: " + lhPeakCount + "/" + rhPeakCount + " PkAvg: " + lhPeakAvg + "/" + rhPeakAvg; //Display peak information
                SvsL = "Staccato: " + ((double)staccatoPct / totalPct).ToString("P");
                if (rhPeakAvg < SvLThreshold) // If we are under the Staccato threshold, we are legato, so update the brush accordingly
                {
                    SvsLBG = legatoBrush;
                }
                else //we are not legato, so update the brush.
                {
                    SvsLBG = staccatoBrush;
                    staccatoPct++;
                }
            }
            iterationNum++;
        }

        private void beatsPM(JointHistory rh)
        /* This method is used to handle the detection of beats - this method does not actually include the detection, but it handles the calculation and display of the BPM and
         * the 'beat circle' on the UI.
         * Parameters:
         *  JointHistory rh: the history of the right hand - this processes the data.
         * Results:
         *  The user interface will be updated and display information about the beat detected and the 'beat circle' will turn green for a few frames when a beat is detected. */
        {
            int checkPosition = (rh.locationCounter - 2 + rh.ARRAY_SIZE) % rh.ARRAY_SIZE; // The check position is 2 behind the most recent data point - have to use modulus to avoid going out of bounds
            int lastBeatPos = bpmPos; //assign the last beat position to the position that will be updated in this method
            if (framesSinceBeat > 12 && rh.checkBeat(checkPosition, BPMVthreshold, BPMDthreshold)) //if there is a beat and there hasn't been one in a few frames, store it and calculate bpm
            {
                bpmlastBeats[bpmPos] = rh.times[checkPosition]; //add latest beat to the array of beats
                bpmPos++; //increment counter to next open position
                if (bpmPos >= 10) //if we go out of bounds, move the next open position to 0
                    bpmPos = 0;
                beatCounter++; //increase the number of beats counter
                framesSinceBeat = 0; //reset the frames since beat
            }
            framesSinceBeat++; //increment the frames since last beat
            if (framesSinceBeat <= 6) //we make use of the frames since beat to allow the 'bpm circle' to have the color linger instead of flickering when a beat is detected
                BPMBG = goodBrush;
            else //reset the circle to 'no beat' after a few frames
                BPMBG = neutralBrush;
            if (beatCounter >= 10 && lastBeatPos != bpmPos) //if we have enough data to calculate bpm, and if we have calculated a beat
            {
                TimeSpan diff = bpmlastBeats[lastBeatPos].Subtract(bpmlastBeats[bpmPos]); //calculate the difference between the most recent (lastBeatPos) and the oldest (bpmPos)
                float diffTime = (float)diff.TotalSeconds;  //convert the total seconds to a float in order to calculate the BPM
                BPM = 60*9 / diffTime; //Since we are using the difference over the last 10 beats using difference in seconds, we divide 60 seconds/min * 9 beats by the difference in time
                totalBPM = ((totalBPM * beatCounter) + (BPM)) / beatCounter;
            }
            if (BPMAvg == 0) //the first time we calculate the bpm, the Avg will be 0 so we instead just update it to the calculated number
                BPMAvg = BPM;
            else
                BPMAvg = (BPMAvg * 9 + BPM) / 10; //calculate moving average
            BPMFB = "BPM: " + Math.Round(BPMAvg, 0);
        }

        private float calcVeloc(float currDisp, float pastDisp, int timeDiff)
        /* This method calculates the velocity using displacement and time parameters.
         * Parameters: 
         *  float currDisp: the more recent displacement data point
         *  float pastDisp: the older displacement data point
         *  int timeDiff: the difference (in milliseconds) between the times of the two displacement parameters
         * Results:
         *  Returns the calculated velocity using the velocity formula, in units/ms */
        {
            return ((currDisp - pastDisp)*1000/ timeDiff);
        }

        private float calcSmoothed(float prevPos, float curPos, float nextPos)
        /* This method will average the value of the 3 parameters. This is used to smooth the data in order to better calculate peaks.
         * Parameters:
         *  float prevPos, curPos, nextPos: the 3 values that will be smoothed together. They are named in terms of position that will be passed from the Staccato vs Legato detection.
         * Results:
         *  The average of the 3 positions is returned. */
        {
            return ((prevPos + curPos + prevPos)/3);
        }

        private bool isPeak(float prevPos2, float prevPos, float curPos, float nextPos)
        /* This method will determine if there is a peak around a specific data point.
         * Parameters:
         *  float prevPos2: the value of the data point that is two positions before the point being examined for a peak
         *  float prevPos: the value of the data point before the point being examined for a peak
         *  float curPos: the value of the data point being examined
         *  float nextPos: the value of the data point after the one being examined
         * Results:
         *  A boolean is returned, true if a peak has been found, or false if the formula does not find a peak. */
        {
            return((curPos > (prevPos2+prevPos)/2)&&(curPos > nextPos));
        }

        private float calcTotalVeloc(float x, float y, float z)
        /* This method calculates the total velocity using the sum of squares (the square root has been left out to save computation time).
         * Parameters:
         *  float x: the x velocity
         *  float y: the y velocity
         *  float z: the z velocity
         * Results:
         *  The sum of squares is returned. */
        {
            return((x*x) + (y*y) + (z*z));
        }

        // performs check of hinge movement
        // takes following parameters as SkeletonPoints for coordinates:
        // re = right elbow
        // le = left elbow
        private void hingeCheck(SkeletonPoint re, SkeletonPoint le)
        /* Performs check of hinge movement.
         * Parameters:
         *  SkeletonPoint re: Point for the right elbow.
         *  SkeletonPoint le: Point for the left elbow.
         * Results:
         *  Detects whether there is excessive hinge movement. */
        {
            int movAvgNum = 7; // size of moving average; this should be smaller than the array length

            double percent = (double)hingePct / totalPct * 100;
           // Feedback += "\nPercentage of excessive hinge movement: " + string.Format("{0:0.0}", percent) + "%\n";

            // calculate distances from last point and store them (if a last point exists)
            if (reLast != null && leLast != null)
            {
                pastElbowR[hingePtr] = (float)Math.Sqrt((re.X - reLast.X) * (re.X - reLast.X) + (re.Y - reLast.Y) * (re.Y - reLast.Y) + (re.Z - reLast.Z) * (re.Z - reLast.Z));
                pastElbowL[hingePtr] = (float)Math.Sqrt((le.X - leLast.X) * (le.X - leLast.X) + (le.Y - leLast.Y) * (le.Y - leLast.Y) + (le.Z - leLast.Z) * (re.Z - reLast.Z));
                hingePtr++;

                // check for full buffer; if full, go back to beginning and overwrite oldest value
                if (hingePtr == pastElbowR.Length)
                {
                    hingePtr = 0;

                    hingeFull = true;
                }
            }

            // set most recent positions to current position
            reLast = re;
            leLast = le;

            // If buffer is full, analyze
            if (hingeFull)
            {
                int tmpPtr = hingePtr; // set a temporary pointer to hingePtr
                float elbowL = 0; // moving-average distance for left elbow
                float elbowR = 0;
                
                for (int i = 0; i < movAvgNum; i++)
                {
                    
                    tmpPtr--;

                    if (tmpPtr < 0) // reset pointer to end of array if we hit the beginning
                    {
                        tmpPtr = pastElbowL.Length - 1;
                    }

                    elbowL += pastElbowL[tmpPtr];
                    elbowR += pastElbowR[tmpPtr];
                }
                //Feedback += elbowL + "  " + elbowR;
                elbowL = elbowL / movAvgNum;
                elbowR = elbowR / movAvgNum;
                
                if (pastElbowL.Max() > hingeThreshold || pastElbowR.Max() > hingeThreshold)
                {
                    hingePct++;
                    Hinge = "Excessive Hinge Movement: " + string.Format("{0:0.0}", percent) + "%";
                    HingeBG = badBrush;
                }
                else
                {
                    HingeBG = goodBrush;
                }
                
            } // close if statement for full buffer
        } //end hingecheck

        private void save_Click(object sender, RoutedEventArgs e)
        /* This method handles the things that occur once the save button is clicked.
         * Parameters: 
         *  object sender - sender of click (not used)
         *  RoutedEventArgs e - arguments for click event (not used)
         * Results:
         *  Results of conducting trial are printed to a text file, then go back to initial state. */
        {
            String filename = FILE_PREFIX + studentNo.ToString("D3") + ".txt"; //studentNo.ToString("D3") displays it in decimal format with 3 digits (padding with 0 until there are 3)
            System.IO.StreamWriter reportFile = new StreamWriter(filename);
            TimeSpan lengthOfRec = DateTime.Now.Subtract(initialTime);
            String timeString = lengthOfRec.Minutes.ToString() + "m " + lengthOfRec.Seconds.ToString() + "s";
            reportFile.WriteLine("Student Report #" + studentNo + " Length: " + timeString);
            reportFile.WriteLine("--------------------------------------------------");
            reportFile.WriteLine("Mirroring Percent: " + ((double)mirrorPct / totalPct).ToString("P")); // .ToString("P") converts a double to a percent (ex from .267 -> 26.7%)
            reportFile.WriteLine("Swaying Percent: " + ((double)swayPct / totalPct).ToString("P"));
            reportFile.WriteLine("Leaning Percent: " + ((double)leanPct / totalPct).ToString("P"));
            reportFile.WriteLine("Excessive Hinge Movement Percent: " + ((double)hingePct / totalPct).ToString("P"));
            reportFile.WriteLine("Staccato Percent: " + ((double)staccatoPct / totalPct).ToString("P"));
            reportFile.WriteLine("Average BPM: " + string.Format("{0:0.0}", totalBPM));
            reportFile.WriteLine("--------------------------------------------------");
            reportFile.Close();
            reset_Click(sender, e);
            studentNo++;
        }
        private void window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        /* This method detects if a key is down.
         * Parameters:
         *  object sender - sender of keypress event 
         *  System.Windows.Input.KeyEventArgs e - arguments from the keypress event.
         * Results:
         *  If the save results keypress occurs, run the save results function. */
        {
            if (e.Key == SAVE_RESULTS_KEY)
                save_Click(sender, e);
        }

        #endregion
    }
}