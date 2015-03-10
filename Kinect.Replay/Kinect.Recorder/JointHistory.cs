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
        public DateTime[] times; // array of times for data above

        public JointHistory() // constructor, builds the arrays
        {
            locationCounter = 0;
            hasLooped = false;
            location = new float[3,ARRAY_SIZE]; // 1 dimension for each axis - x, y, z
            velocity = new float[4,ARRAY_SIZE]; // 1 dimension for each axis, as well as combined velocity - x, y, z, combined
            times = new DateTime[ARRAY_SIZE]; // the times when each data point is recorded, in order to determine velocity or beats
        }

        public void addData(float x, float y, float z) // add a piece of data to the arrays
        {
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

        public bool checkBeat(int checkLocation, float vThreshold, float yThreshold)
        /* This method is used to determine if a beat is located at the location specified, and taking into account the special thresholds.
         * Parameters:
         *      int checkLocation: the location in the data array to check for a beat
         *      float vThreshold:  the threshold for the velocity, if it is lower than the threshold discard the results
         *      float yThreshold:  the threshold for the y displacement, if it is lower discard the results.
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
            if(XBeat)
                Console.Write("X BEAT DETECTED IN FCN\n");
            bool YBeat = ((leftAvgY < 0 && rightAvgY > 0) || (rightAvgY < 0 && leftAvgY > 0)) && (Math.Abs(rightAvgY - leftAvgY) > vThreshold);
            if (YBeat)
                Console.Write("Y BEAT DETECTED IN FCN\n");
            if (XBeat || YBeat)
            {
                return (true);
            }
            return (false);
        }
        /*public bool checkBeat(int checkLocation, float vThreshold, float yThreshold) //look for a beat at the specified location, with velocity threshold and y threshold
        {
            if (checkLocation >= 0) //make sure it's valid
            {
                int prev2Loc = (checkLocation + ARRAY_SIZE - 2) % ARRAY_SIZE; 
                int prevLoc = (checkLocation + ARRAY_SIZE - 1) % ARRAY_SIZE; //location before the beat we are checking 
                int nextLoc = (checkLocation + 1) % ARRAY_SIZE; // location after the beat we are checking
                int next2Loc = (checkLocation + 2) % ARRAY_SIZE;
                if ((checkLocation < 2) && !hasLooped) //if it's at location 0 or 1 and we haven't looped, it is too early to calculate a beat
                    return (false);
                Console.Write("Velocity Change: " + (velocity[3, checkLocation] - velocity[3, prevLoc]) + " " + (velocity[3, checkLocation] - velocity[3, nextLoc]) + "\n");
                Console.Write("Location: " + (location[1, prevLoc] - location[1, checkLocation]) + " " + (location[1, nextLoc] - location[1, checkLocation]) + "\n");
                if (velocity[3, checkLocation] - velocity[3, prevLoc] > vThreshold && velocity[3, checkLocation] - velocity[3, nextLoc] > vThreshold
                    && velocity[3, checkLocation] - velocity[3, prev2Loc] > vThreshold && velocity[3, checkLocation] - velocity[3, next2Loc] > vThreshold) //if we have a local maxima, and it is above the threshold, a beat has been detected.
                {
                    Console.Write("VELOCITY-MAXIMA BEAT \n");
                    return (true);
                }
                if (location[1, prevLoc] - location[1, checkLocation] > yThreshold && location[1, nextLoc] - location[1, checkLocation] > yThreshold) //if we have a local y minima, we have a beat.
                {
                    Console.Write("Y-MINIMA BEAT \n");
                    return (true);
                }

            }
            return (false); //if neither of the above are true, we have no beat
        }*/
    }
}
