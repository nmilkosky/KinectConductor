import numpy as np                                  # external tools, must be downloaded from www.numpy.org

class BPMHist:    
    def __init__(self):
        self.lastBeatsTime = [0] * 10               # time for each beat
        self.counter = 0                            # keep track of position
        self.hasReachedTen = False                  # can't correctly calculate without 10 beats, so keep track of that here
    def addBeat(self, time):                        # method that adds beat and returns bpm
        self.counter += 1                           # increment counter
        if self.counter >= 10:                      # prevent out of bounds
            if not self.hasReachedTen:              # we can start calculating bpm now that we have 10 data points
                self.hasReachedTen = True           
            self.counter = 0                        # prevent out of bounds
        self.lastBeatsTime[self.counter] = time     # record time
        oldestBeat = self.counter + 1               # oldest beat will be one position ahead of the newest one
        if oldestBeat >= 10:                        # prevent out of bounds
            oldestBeat = 0
        BPM = ((9*1000*60)/(self.lastBeatsTime[self.counter] - self.lastBeatsTime[oldestBeat])) # Calculate the BPM
        print(self.lastBeatsTime[self.counter] - self.lastBeatsTime[oldestBeat])                # for debugging
        if self.hasReachedTen:                                                                  # if we have 10 we can return useful data
            return BPM
        return 0                                                                                # otherwise return 0
        
def analyzeBPM(data):                               # this function will analyze the CSV data and generate BPM from it
    TimeStamps = data[:,0]                          # extract the first column - which are timestamps
    RHX_Pos = data[:,4]                             # extract the 5th column, the RH X position
    RHY_Pos = data[:,5]                             # extract the 6th column, the RH Y position
    RHXVel = [None] * len(TimeStamps)               # initialize an empty aray with length equal to the length of the TimeStamp list
    RHYVel = [None] * len(TimeStamps)               # initialize an empty aray with length equal to the length of the TimeStamp list
    Beat = [0] * len(TimeStamps)                    # initialize an empty aray with length equal to the length of the TimeStamp list
    bpmHistory = BPMHist()
    bpmList = [0] * len(TimeStamps)
    last_Beat_X = -10                               # last beat's x position
    last_Beat_Y = -10                               # last beat's y position
    for index in range(1,len(TimeStamps)):          # iterates for every index in the column, will calc velocity
        RHXVel[index] = (RHX_Pos[index] - RHX_Pos[index -1])/(TimeStamps[index] - TimeStamps[index-1])
        RHYVel[index] = (RHX_Pos[index] - RHX_Pos[index -1])/(TimeStamps[index] - TimeStamps[index-1])
    for index in range(3,len(TimeStamps)-1):
        leftAvgX = (RHXVel[index-2] + RHXVel[index-1])/2        # two values left of the point being examined
        leftAvgY = (RHYVel[index-2] + RHYVel[index-1])/2        # two y values left of the pt being examined
        rightAvgX = (RHXVel[index] + RHXVel[index+1])/2         # value being examined, and the one after it
        rightAvgY = (RHYVel[index] + RHYVel[index+1])/2         # y value being examined, and one after it
        XBeat = ((leftAvgX < 0 and rightAvgX > 0) or (rightAvgX < 0 and leftAvgX > 0)) #and (abs(rightAvgX - leftAvgX) > .008)
        YBeat = ((leftAvgY < 0 and rightAvgY > 0) or (rightAvgY < 0 and leftAvgY > 0)) #and (abs(rightAvgY - leftAvgY) > .008)
        distBetween = abs((RHX_Pos[index]**2 + RHY_Pos[index]**2) - (last_Beat_X**2 + last_Beat_Y**2))
        if ((XBeat or YBeat) and (distBetween >= .02)):         # if we have a beat and its far enough from the previous
            Beat[index] = 1                                     # we have a beat
            last_Beat_X = RHX_Pos[index]                        # log this beat for next last beat formula
            last_Beat_Y = RHY_Pos[index]
            bpmList[index] = bpmHistory.addBeat(TimeStamps[index])  #calculate BPM
            print(str(bpmList[index]) + " " + str(distBetween))     #print for debugging
        else:                                                   #if no beat, can't calculate BPM
            bpmList[index] = 0                                  #just set it to 0
    outputData = [TimeStamps, Beat, bpmList]                    #create a matrix of info
    return outputData
list_nums = ['002','003','011','012','015','016','021','023','025','028']   # number in the title of the CSV file
for i in list_nums:                                             # for each number in list_nums
    CSV_Filename = 'StudentReport' + i + '.csv'                 # generates the filename
    filedata = np.genfromtxt(CSV_Filename, delimiter=",")       # generates the array
    beats = analyzeBPM(filedata)                                # analyze the data
    beats_a = np.transpose(np.asarray(beats))                   # transpose the data from rows to columns
    np.savetxt(('BPMReport' + i + '.csv'), beats_a, delimiter=",")  # save the data
