# Discomatic 

A utility that takes advantage of the Kinect IR Cameraa to "blob" track the average center of multiple people, from 9 meters above the ground, as they move into different "zones", which then sends a different midi signal to "LoopMIDI" http://www.tobias-erichsen.de/software/loopmidi.html
<br>
<br>LoopMiDi is easily recognizable by Ableton Live.  DISCOMATIC, along with LoopMiDi and Ableton Live allows us to create interactive installations where users can control audio, lights, and even video.

## Code and Build Status

This is the version used for the 2014 New Year's installation as part of Boston First Night and Figment on the Common.

## Feature Requests

### High Priority
-Align IR Camera Grid with Actual Camera Grid ( or display IR image )
-Send a Single MIDI Signal upon trigger, and not send constant MIDI Signals
-Allow disabling of video to reduce resource consumption.

### Low Priority [Wish List]
- The Ability to reconfigure the Grid [More/Less and Larger/Smaller Squares and/or Rectangles.]<br>
- Control which MIDI CC or Note is being sent in each Zone
- Kinect Zoom/Pan/Tilt Access
- Change to MIDI CC?
- Client/server model so one machine can run server to watch kinect and process, and multiple machines can run the client to send midi thru ableton
-- web config/live edit?
-- light scripting directly( video->derez)

## Original Author
Munish Dhiman<br>
munish@gestureresearch.com<br>
He can be contacted with any questions.<br>

## Copyleft
Released under GPLv3 at the present time.
