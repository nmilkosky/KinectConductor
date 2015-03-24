using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect.Recorder
{
    class JointHistory
    {
        public int ARRAY_SIZE = 300; // maximum size of array - will loop around to beginning when this point is reached
        private bool hasLooped = false; // if the counter has wrapped around once - this is useful in determining if we can determine velocity
        public int locationCounter; // counter that determines where we are in the array
        public float[,] location; // array of locations in Kinect Units
        public float[,] velocity; // array of velocities in Kinect Units/sec
        public float[] lastBeatLoc; //the position in x and y coordinates of the last beat location
        public DateTime[] times; // array of times for data above

        public JointHistory() // constructor, builds the arrays
        /* The constructor for the JointHistory class.
         * Parameters: None
         * Results:
         *  A JointHistory object is constructed. */
        {
            locationCounter = 0;
            hasLooped = false;
            location = new float[3,ARRAY_SIZE]; // 1 dimension for each axis - x, y, z
            velocity = new float[4,ARRAY_SIZE]; // 1 dimension for each axis, as well as combined velocity - x, y, z, combined
            times = new DateTime[ARRAY_SIZE]; // the times when each data point is recorded, in order to determine velocity or beats
            lastBeatLoc = new float[2]; // index 0 - x location, index 1 - y location
            lastBeatLoc[0] = -1000; //initial value doesn't matter as long as it isn't close to where the actual location could be
            lastBeatLoc[1] = -1000;
        }

        public void addData(float x, float y, float z) // add a piece of data to the arrays
        {
        /* This method is used to add data to the JointHistory object.
         * Parameters:
         *      float x:  the x (horizontal) location for the point
         *      float y:  the y (vertical) location for the point
         *      float z:  the z (depth) location for the point
         * Results:
         *      the location, times, and velocity arrays are updated with the most recent information.
         * */
            location[0, locationCounter] = x; //assign x position
            Console.Write("X: " + x + "\n");
            location[1, locationCounter] = y; //assign y position
            Console.Write("Y: " + y + "\n");
            //location[2, locationCounter] = z; //assign z position
            times[locationCounter] = DateTime.Now; //record current time
            float timeDiff = 0;
            if (locationCounter != 0) //as long as it's not 0, it is not a special case
            {
                timeDiff = (float)((times[locationCounter].Subtract(times[locationCounter - 1])).TotalSeconds); //calculate the time difference between the last two data points in seconds
                //Console.Write("Time Diff: " + timeDiff + "\n");
                velocity[0, locationCounter] = (x - location[0, locationCounter - 1]); //calculate x velocity
                velocity[1, locationCounter] = (y - location[1, locationCounter - 1]); //calc y velocity
                Console.Write("Velocity: " + velocity[0, locationCounter] + " | " + velocity[1, locationCounter] + "\n");
                //velocity[2, locationCounter] = (z - location[1, locationCounter - 1]) / timeDiff; //calc z velocity
                velocity[3, locationCounter] = (float)Math.Atan2(velocity[1, locationCounter], velocity[0, locationCounter]); //calculate absolute velocity - using sum of squares w/o
                                             //+ (velocity[1, locationCounter] * velocity[1, locationCounter]);//square root to save computations
               //                              + (velocity[2, locationCounter] * velocity[2, locationCounter]);
            }

            else if (locationCounter == 0 && hasLooped == true) //if we are at 0 and we have looped, we need to access location 299
            {
                timeDiff = (float)(times[0].Subtract(times[ARRAY_SIZE - 1]).TotalSeconds); //calculate the time difference between the last two data points in seconds
                velocity[0, 0] = (x - location[0, ARRAY_SIZE-1]) / timeDiff; //calculate x velocity
                velocity[1, 0] = (y - location[1, ARRAY_SIZE-1]) / timeDiff; //calc y velocity
                //velocity[2, 0] = (z - location[1, ARRAY_SIZE-1]) / timeDiff; //calc z velocity
                velocity[3, 0] = (float)Math.Atan2(velocity[1, 0], velocity[0, 0]); //calculate absolute velocity - using sum of squares w/o
                               //+ (velocity[1, 0] * velocity[1, 0]); //square root to save computations
                               //+ (velocity[2, 0] * velocity[2, 0]);
                
            }
            else //we are at 0 and we haven't looped, meaning we have no previous data, so velocity is 0
            {
                velocity[0, 0] = 0.0F;
                velocity[1, 0] = 0.0F;
                //velocity[2, 0] = 0.0F;
                velocity[3, 0] = 0.0F;
            }
            locationCounter++;
            if (locationCounter >= ARRAY_SIZE)
            {
                hasLooped = true;
                locationCounter = 0;
            }
        }

        public bool checkBeat(int checkLocation, float vThreshold, float dThreshold)
        /* This method is used to determine if a beat is located at the location specified, and taking into account the special thresholds.
         * Parameters:
         *      int checkLocation: the location in the data array to check for a beat
         *      float vThreshold:  the threshold for the velocity, if it is lower than the threshold discard the results
         *      float dThreshold:  the threshold that determines whether the beat is far enough away to be considered its own beat.
         * Results:
         *      returns a true or false value. True indicates that a beat was detected at the checked location, false indicates that there was no beat.
         * */
        {
            int[] index = new int[4]; // has to be even
            for(int i = 0; i < index.Length; i++)
                index[i] = (checkLocation + ARRAY_SIZE + i - index.Length / 2 ) % ARRAY_SIZE;
            float leftAvgX = 0; //average velocities on left side
            float rightAvgX = 0; //average velocities on right side
            float leftAvgY = 0; //average velocities on left side
            float rightAvgY = 0; //average velocities on right side
            for (int j = 0; j < index.Length / 2 - 1; j++)
            {
                leftAvgX += velocity[0, index[j]];
                leftAvgY += velocity[1, index[j]];
            }
            for (int k = index.Length / 2; k < index.Length; k++)
            {
                rightAvgX += velocity[0, index[k]];
                rightAvgY += velocity[1, index[k]];
            }
            leftAvgX = leftAvgX / (index.Length / 2);
            leftAvgY = leftAvgY / (index.Length / 2);
            rightAvgX = rightAvgX / (index.Length / 2);
            rightAvgY = rightAvgY / (index.Length / 2);
            Console.Write("Avges: " + (leftAvgX) + " | " + (rightAvgX) +  " || " + leftAvgY + " | " + rightAvgY +  "\n");
            //If the two sides are oppositely signed, and sufficiently different to believe they are a beat for X and Y
            bool XBeat = ((leftAvgX < 0 && rightAvgX > 0) || (rightAvgX < 0 && leftAvgX > 0)) && (Math.Abs(rightAvgX - leftAvgX) > vThreshold);
            bool YBeat = ((leftAvgY < 0 && rightAvgY > 0) || (rightAvgY < 0 && leftAvgY > 0)) && (Math.Abs(rightAvgY - leftAvgY) > vThreshold);
            //Make sure it is a sufficent distance away from the last beat (if it's false, that means it is too close, true means it is sufficiently far to be its own beat).
            float distance = Math.Abs((lastBeatLoc[0]*lastBeatLoc[0] + lastBeatLoc[1]*lastBeatLoc[1]) - (location[0,checkLocation]*location[0,checkLocation] + location[1,checkLocation]*location[1,checkLocation])); // no sqrt to save computation
            bool notTooClose = (distance > dThreshold*dThreshold);
            Console.Write(notTooClose);
            Console.Write("\n Distance: " + distance + "\n");
            if ((XBeat || YBeat) && notTooClose)
            {
                lastBeatLoc[0] = location[0, checkLocation]; // update the last beat location
                lastBeatLoc[1] = location[1, checkLocation]; // update the last beat location
                return (true);
            }
            return (false);
        }
    }
}
