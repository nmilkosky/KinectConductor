import numpy as np                                  # external tools, must be downloaded from www.numpy.org
def analyzeBPM(data):                               # this function will analyze the CSV data and generate BPM from it
    TimeStamps = data[:,0]                          # extract the first column - which are timestamps
    RHX_Pos = data[:,4]                             # extract the 5th column, the RH X position
    RHY_Pos = data[:,5]                             # extract the 6th column, the RH Y position
    RHXVel = [None] * len(TimeStamps)              # initialize an empty aray with length equal to the length of the TimeStamp list
    RHYVel = [None] * len(TimeStamps)              # initialize an empty aray with length equal to the length of the TimeStamp list
    Beat = [0] * len(TimeStamps)                    # initialize an empty aray with length equal to the length of the TimeStamp list
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
        distBetween = (RHX_Pos[index]**2 + RHY_Pos[index]**2) - (last_Beat_X**2 + last_Beat_Y**2)
        if (XBeat or YBeat):
            Beat[index] = 1
            print(index)
            last_Beat_X = RHX_Pos[index]
            last_Beat_Y = RHY_Pos[index]
    return Beat
list_nums = ['002','003','011','012','015','016','021','023','025','028']   # number in the title of the CSV file
for i in list_nums:                                 # for each number in list_nums
    CSV_Filename = 'StudentReport' + i + '.csv'     # generates the filename
    filedata = np.genfromtxt(CSV_Filename, delimiter=",")          # generates the array
    beats = analyzeBPM(filedata)                    # analyze the data
    beats_a = np.asarray(beats)
    np.savetxt(('BPMReport' + i + '.csv'), beats_a, delimiter=",")
