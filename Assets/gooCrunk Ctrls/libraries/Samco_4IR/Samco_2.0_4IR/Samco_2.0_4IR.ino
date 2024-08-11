/*!
 * @file Samco_2.0_4IR.ino
 * @brief 10 Button Light Gun sketch for 4 LED setup
 * @n INO file for Samco Light Gun 4 LED setup
 *
 * @copyright   Samco, https://github.com/samuelballantyne, June 2020
 * @copyright   GNU Lesser General Public License
 *
 * @author [Sam Ballantyne](samuelballantyne@hotmail.com)
 * @version  V1.0
 * @date  2020
 */

#include <HID.h>                // Load libraries
#include <Wire.h>
#include <Keyboard.h>
#include <AbsMouse.h>
#include <DFRobotIRPosition.h>
#include <Samco.h>

// Tigger is set to LEFT_MOUSE, A Button is set to RIGHT_MOUSE & B Button is set to MIDDLE_MOUSE 
       
char _upKey = "KEY_UP_ARROW";                
char _downKey = KEY_DOWN_ARROW;              
char _leftKey = KEY_LEFT_ARROW;             
char _rightKey = KEY_RIGHT_ARROW;                             
char _startKey = KEY_RETURN; 
char _selectKey = KEY_BACKSPACE; 

int xCenter = 512;          // If second calibration seems more accurate you can replace these values with the altered camera center values from serial monitor
int yCenter = 368;

int finalX;                 // Values after tilt correction
int finalY;

int xLeft;                  // Stored calibration points
int yTop;
int xRight;
int yBottom;

int MoveXAxis;              // Unconstrained mouse postion
int MoveYAxis;               

int conMoveXAxis;           // Constrained mouse postion
int conMoveYAxis;           

int count = 4;                  // Set intial count

int _tiggerPin = 7;               // Label Pin to buttons
int _upPin = 11;                
int _downPin = 9;              
int _leftPin = 100;             
int _rightPin = 99;               
int _APin = A1;                
int _BPin = A0;              
int _startPin = A2; 
int _selectPin = A3;               
int _reloadPin = 13;
int _pedalPin = 90;                       

int buttonState1 = 0;           
int lastButtonState1 = 0;
int buttonState2 = 0;
int lastButtonState2 = 0;
int buttonState3 = 0;
int lastButtonState3 = 0;
int buttonState4 = 0;         
int lastButtonState4 = 0; 
int buttonState5 = 0;           
int lastButtonState5 = 0;
int buttonState6 = 0;
int lastButtonState6 = 0;
int buttonState7 = 0;
int lastButtonState7 = 0;
int buttonState8 = 0;         
int lastButtonState8 = 0;          
int buttonState9 = 0;           
int lastButtonState9 = 0;
int buttonState10 = 0;
int lastButtonState10 = 0; 
int buttonState11 = 0;
int lastButtonState11 = 0; 

DFRobotIRPosition myDFRobotIRPosition;
Samco mySamco;

int res_x = 1023;              // UPDATE: These values do not need to change
int res_y = 768;               // UPDATE: These values do not need to change


const int triPin = 12;
const int echoPin = 5;


const int killSwitch = 10;
const int pauseButtonPin = 11;

char _pauseKey = "P";                
char _distanceLvl1_Fire = "A";                
char _distanceLvl2_Fire = "B";                
char _distanceLvl3_Fire = "C";                
char _distanceLvl4_Fire = "D";                

char _distanceLvl1_Load = "W";
bool lvl1_HasLoaded;                
char _distanceLvl2_Load = "X";
bool lvl2_HasLoaded;                     
char _distanceLvl3_Load = "Y";
bool lvl3_HasLoaded;                     
char _distanceLvl4_Load = "Z";
bool lvl4_HasLoaded;                     

char _firStartedKey = "F";                

const float fireDistance = 1;
const float distanceLvl1 = 3;
const float distanceLvl2 = 5;
const float distanceLvl3 = 7;
const float distanceLvl4 = 9;



int currentDistance;
bool canFire;



void setup() {

  myDFRobotIRPosition.begin(); 
   
  Serial.begin(9600);                     // For debugging (make sure your serial monitor has the same baud rate)

  AbsMouse.init(res_x, res_y);            

  pinMode(_tiggerPin, INPUT_PULLUP);         // Set pin modes
//  pinMode(_upPin, INPUT_PULLUP);
  pinMode(_downPin, INPUT_PULLUP);
  pinMode(_leftPin, INPUT_PULLUP);
  pinMode(_rightPin, INPUT_PULLUP);          // Set pin modes
  pinMode(_APin, INPUT_PULLUP);
  pinMode(_BPin, INPUT_PULLUP);
  pinMode(_startPin, INPUT_PULLUP);  
  pinMode(_selectPin, INPUT_PULLUP);
  pinMode(_reloadPin, INPUT_PULLUP);       
  pinMode(_pedalPin, INPUT_PULLUP);

  AbsMouse.move((res_x / 2), (res_y / 2));          // Set mouse position to centre of the screen
  

  pinMode(killSwitch, INPUT_PULLUP);
  pinMode(pauseButtonPin, INPUT_PULLUP);

  pinMode(triPin, OUTPUT);
  pinMode(echoPin, INPUT);
  Keyboard.begin();

  delay(500);
  
}


void loop() {
bool killSwitchState = digitalRead(killSwitch); 
if (killSwitchState)
  return;


//Distance Controls
  long duration, inches, cm;

  // The PING))) is triggered by a HIGH pulse of 2 or more microseconds.
  // Give a short LOW pulse beforehand to ensure a clean HIGH pulse:
  digitalWrite(triPin, LOW);
  delayMicroseconds(2);
  digitalWrite(triPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(triPin, LOW);

  // The same pin is used to read the signal from the PING))): a HIGH pulse
  // whose duration is the time (in microseconds) from the sending of the ping
  // to the reception of its echo off of an object.
  duration = pulseIn(echoPin, HIGH);

  // convert the time into a distance
  distance = microsecondsToInches(duration);
 // distance = microsecondsToCentimeters(duration);

//Fire with Distance Sensor
  if (distance < distanceLvl4)
  {
    if (canFire)
    {
      if (distance < fireDistance)
      {
        //Player has fired
        if (currentDistance <= distanceLvl1)
        {
          Keyboard.write(_distanceLvl1_Fire);
        }
        else if (currentDistance <= distanceLvl2)
        {
          Keyboard.write(_distanceLvl2_Fire);
        }
        else if (currentDistance <= distanceLvl3)
        {
          Keyboard.write(_distanceLvl3_Fire);
        }
        else if (currentDistance <= distanceLvl4)
        {
          Keyboard.write(_distanceLvl4_Fire);
        }
        
        canFire = false;
      }

      if (distance > currentDistance)
      {
        currentDistance = distance;
        //Player is loading Goo
        if (!lvl1_HasLoaded && currentDistance <= distanceLvl1)
        {
          Keyboard.write(_distanceLvl1_Load);
          lvl1_HasLoaded = true;
        }
        else if (!lvl2_HasLoaded && currentDistance <= distanceLvl2)
        {
          Keyboard.write(_distanceLvl2_Load);
          lvl2_HasLoaded = true;
        }
        else if (!lvl3_HasLoaded && currentDistance <= distanceLvl3)
        {
          Keyboard.write(_distanceLvl3_Load);
          lvl3_HasLoaded = true;
        }
        else if (!lvl4_HasLoaded && currentDistance <= distanceLvl4)
        {
          Keyboard.write(_distanceLvl4_Load);
          lvl4_HasLoaded = true;
        }
      }
    }
    else if (!canFire && distance > fireDistance)
    {
      Keyboard.write(_firStartedKey);
      canFire = true;
      currentDistance = distance;
      lvl1_HasLoaded = false;
      lvl2_HasLoaded = false;
      lvl3_HasLoaded = false;
      lvl4_HasLoaded = false;
    }
  }


  //Serial.print(distance);
  //Serial.print("inORcm, ");
  //Serial.println();
  

//Mouse Controls
  if (count > 3) {

    getPosition();
    mouseButtons();
    PrintResults();
    go();

  }

  /* ------------------ START/PAUSE MOUSE ---------------------- */


  else if (count > 2 ) {

    skip();
    getPosition();
    mouseCount();
    mouseButtons();
    PrintResults();

  }


  /* ---------------------- TOP LEFT --------------------------- */


  else if (count > 1 ) {

    AbsMouse.move(300, 200);

    mouseCount();
    getPosition();

    xLeft = finalX;
    yTop = finalY;

    PrintResults();

  }


  /* -------------------- BOTTOM RIGHT ------------------------- */


  else if (count > 0 ) {
    
    AbsMouse.move((res_x - 300), (res_y - 200));
    
    mouseCount();
    getPosition();

    xRight = finalX;
    yBottom = finalY;
    
    xCenter = ((xRight - xLeft) / 2) + min(xRight, xLeft);
    yCenter = ((yBottom - yTop) / 2) + min(yBottom, yTop);

    PrintResults();

  }


  /* ---------------------- LET'S GO --------------------------- */


  else {
    
    AbsMouse.move(conMoveXAxis, conMoveYAxis);

    mouseButtons();
    getPosition();

    MoveXAxis = map (finalX, xLeft, xRight, 300, (res_x - 300));
    MoveYAxis = map (finalY, yTop, yBottom, 200, (res_y - 200));
    conMoveXAxis = constrain (MoveXAxis, 0, res_x);
    conMoveYAxis = constrain (MoveYAxis, 0, res_y);

    PrintResults();
    reset();

  }

}


//Distance Controls
long microsecondsToInches(long microseconds) {
  // According to Parallax's datasheet for the PING))), there are 73.746
  // microseconds per inch (i.e. sound travels at 1130 feet per second).
  // This gives the distance travelled by the ping, outbound and return,
  // so we divide by 2 to get the distance of the obstacle.
  // See: https://www.parallax.com/package/ping-ultrasonic-distance-sensor-downloads/
  return microseconds / 74 / 2;
}

long microsecondsToCentimeters(long microseconds) {
  // The speed of sound is 340 m/s or 29 microseconds per centimeter.
  // The ping travels out and back, so to find the distance of the object we
  // take half of the distance travelled.
  return microseconds / 29 / 2;
}

//MouseControls

/*        -----------------------------------------------        */
/* --------------------------- METHODS ------------------------- */
/*        -----------------------------------------------        */


void getPosition() {    // Get tilt adjusted position from IR postioning camera

myDFRobotIRPosition.requestPosition();
    if (myDFRobotIRPosition.available()) {
    mySamco.begin(myDFRobotIRPosition.readX(0), myDFRobotIRPosition.readY(0), myDFRobotIRPosition.readX(1), myDFRobotIRPosition.readY(1),myDFRobotIRPosition.readX(2), myDFRobotIRPosition.readY(2),myDFRobotIRPosition.readX(3), myDFRobotIRPosition.readY(3), xCenter, yCenter);
    finalX = mySamco.X();
    finalY = mySamco.Y();
    }
    else {
    Serial.println("Device not available!");
    }
}



void go() {    // Setup Start Calibration Button

  buttonState1 = digitalRead(_reloadPin);

  if (buttonState1 != lastButtonState1) {
    if (buttonState1 == LOW) {
      count--;
    }
    else { // do nothing
    }
    delay(50);
  }
  lastButtonState1 = buttonState1;
}



void mouseButtons() {    // Setup Left, Right & Middle Mouse buttons

  buttonState2 = digitalRead(_tiggerPin);
  buttonState3 = digitalRead(pauseButtonPin);  
  buttonState4 = digitalRead(_downPin);
  buttonState5 = digitalRead(_leftPin);
  buttonState6 = digitalRead(_rightPin);   
  buttonState7 = digitalRead(_APin);
  buttonState8 = digitalRead(_BPin);
  buttonState9 = digitalRead(_startPin);      
  buttonState10 = digitalRead(_selectPin); 
  buttonState11 = digitalRead(_pedalPin); 
  
  if (buttonState2 != lastButtonState2) {
    if (buttonState2 == LOW) {
      AbsMouse.press(MOUSE_LEFT);
    }
    else {
      AbsMouse.release(MOUSE_LEFT);
    }
    delay(10);
  }

  if (buttonState3 != lastButtonState3) {
    if (buttonState3 == LOW) {
    Keyboard.press(_pauseKey);
    }
    else {
    Keyboard.release(_pauseKey);
    }
    delay(10);
  }
/*
  if (buttonState4 != lastButtonState4) {     
    if (buttonState4 == LOW) {
    Keyboard.press(_downKey);
    }
    else {
    Keyboard.release(_downKey);
    }
    delay(10);
  }

  if (buttonState5 != lastButtonState5) {
    if (buttonState5 == LOW) {
    Keyboard.press(_leftKey);
    }
    else {
    Keyboard.release(_leftKey);
    }
    delay(10);
  }
  
  if (buttonState6 != lastButtonState6) {
    if (buttonState6 == LOW) {
    Keyboard.press(_rightKey);
    }
    else {
    Keyboard.release(_rightKey);
    }
    delay(10);
  }
*/
  if (buttonState7 != lastButtonState7) {
    if (buttonState7 == LOW) {
      AbsMouse.press(MOUSE_RIGHT);
    }
    else {
      AbsMouse.release(MOUSE_RIGHT);
    }
    delay(10);
  }

  if (buttonState8 != lastButtonState8) {     
    if (buttonState8 == LOW) {
      AbsMouse.press(MOUSE_MIDDLE);
    }
    else {
      AbsMouse.release(MOUSE_MIDDLE);
    }
    delay(10);
  }
  if (buttonState9 != lastButtonState9) {
    if (buttonState9 == LOW) {
    Keyboard.press(_selectKey);
    }
    else {
    Keyboard.release(_selectKey);
    }
    delay(10);
  }
  
  if (buttonState10 != lastButtonState10) {
    if (buttonState10 == LOW) {
    Keyboard.press(_startKey);
    }
    else {
    Keyboard.release(_startKey);
    }
    delay(10);
  }

  if (buttonState11 != lastButtonState11) {
    if (buttonState11 == LOW) {
      AbsMouse.press(MOUSE_RIGHT);
    }
    else {
      AbsMouse.release(MOUSE_RIGHT);
    }
    delay(10);
  }

  lastButtonState2 = buttonState2;
  lastButtonState3 = buttonState3;
  lastButtonState4 = buttonState4;      
  lastButtonState5 = buttonState5;
  lastButtonState6 = buttonState6;
  lastButtonState7 = buttonState7;
  lastButtonState8 = buttonState8;
  lastButtonState9 = buttonState9;
  lastButtonState10 = buttonState10; 
  lastButtonState11 = buttonState11;     
}


void mouseCount() {    // Set count down on trigger

  buttonState2 = digitalRead(_tiggerPin);

  if (buttonState2 != lastButtonState2) {
    if (buttonState2 == LOW) {
      Serial.print(" _tiggerPin_tiggerPin_tiggerPin");

      count--;
    }
    else { // do nothing
    }
    delay(10);
  }

  lastButtonState2 = buttonState2;
}


void reset() {    // Pause/Re-calibrate button

  buttonState1 = digitalRead(_reloadPin);

  if (buttonState1 != lastButtonState1) {
    if (buttonState1 == LOW) {
      count = 3;
      delay(50);
    }
    else { // do nothing
    }
    delay(50);
  }
  lastButtonState1 = buttonState1;
}


void skip() {    // Unpause button

  buttonState1 = digitalRead(_reloadPin);

  if (buttonState1 != lastButtonState1) {
    if (buttonState1 == LOW) {
      count = 0;
      delay(50);
    }
    else { // do nothing
    }
    delay(50);
  }
  lastButtonState1 = buttonState1;
}


void PrintResults() {    // Print results for debugging

  Serial.print("RAW: ");
  Serial.print(finalX);
  Serial.print(", ");
  Serial.print(finalY);
  Serial.print("     Count: ");
  Serial.print(count);
  Serial.print("     Calibration: ");
  Serial.print(xLeft);
  Serial.print(", ");
  Serial.print(yTop);
  Serial.print(", ");
  Serial.print(xRight);
  Serial.print(", ");
  Serial.print(yBottom);
  Serial.print("     Cam Center: x ");
  Serial.print(xCenter);
  Serial.print(", y ");
  Serial.println(yCenter);

}
