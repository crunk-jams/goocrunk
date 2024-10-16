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
       
char _upKey = KEY_UP_ARROW;                
char _downKey = KEY_DOWN_ARROW;              
char _leftKey = KEY_LEFT_ARROW;             
char _rightKey = KEY_RIGHT_ARROW;                             
char _startKey = KEY_RETURN; 
char _selectKey = KEY_BACKSPACE; 

int xCenter = 522;          // If second calibration seems more accurate you can replace these values with the altered camera center values from serial monitor
int yCenter = 138;

int finalX;                 // Values after tilt correction
int finalY;

int xLeft = 181;                  // Stored calibration points
int yTop = -40;
int xRight = 863;
int yBottom = 316;

int MoveXAxis;              // Unconstrained mouse postion
int MoveYAxis;               

int conMoveXAxis;           // Constrained mouse postion
int conMoveYAxis;           

int count = 0;                  // Set intial count

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

int res_x = 2560;// 1023;              // UPDATE: These values do not need to change
int res_y = 1600; //768;               // UPDATE: These values do not need to change


const int triPin = 12;
const int echoPin = 5;

const int killSwitch = 10;

bool canFire;
const int maxDistanceCheck = 100;
const float startFireDistance = 5;
const float endFireDistance = 4.2;
const float distanceLvl1 = 5.25;
const float distanceLvl2 = 5.45;

char _firStartedKey = 'f';                

char _distanceLvl1_Fire = 'a';                
char _distanceLvl2_Fire = 'b';                
char _distanceLvl3_Fire = 'd';                

char _distanceLvl1_Load = 'w';
bool lvl1_HasLoaded;                
char _distanceLvl2_Load = 'x';
bool lvl2_HasLoaded;                     
char _distanceLvl3_Load = 'z';
bool lvl3_HasLoaded;                     

unsigned long currentMillis = 0;    // stores the value of millis() in each iteration of loop()
const int onBoardDistanceInterval = 60;
unsigned long previousDistanceCheck = 0;


const int delayAfterShooting = 500;
unsigned long previousShootCheck = 0;


void setup() {

  myDFRobotIRPosition.begin(); 
   
  Serial.begin(9600);                     // For debugging (make sure your serial monitor has the same baud rate)

  AbsMouse.init(res_x, res_y);            

  pinMode(_tiggerPin, INPUT_PULLUP);         // Set pin modes
  pinMode(_upPin, INPUT_PULLUP);
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

  pinMode(triPin, OUTPUT);
  pinMode(echoPin, INPUT);
  Keyboard.begin();

  delay(500);
  
}


void loop() {
bool killSwitchState = digitalRead(killSwitch); 
if (killSwitchState)
  return;

  currentMillis = millis();
  captureDistance();


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

void captureDistance()
{
  if (currentMillis - previousDistanceCheck <= onBoardDistanceInterval)
    return;

  previousDistanceCheck += onBoardDistanceInterval;

      //if time between each shot is ok allow for another shoot.
  if (currentMillis <= previousShootCheck)
    return;



  float duration, cm, currentDistance;

  // The PING))) is triggered by a HIGH pulse of 2 or more microseconds.
  // Give a short LOW pulse beforehand to ensure a clean HIGH pulse:
  digitalWrite(triPin, LOW);
  delayMicroseconds(2);
  digitalWrite(triPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(triPin, LOW);
  duration = pulseIn(echoPin, HIGH);

  cm = microsecondsToCentimeters(duration);

  if (cm < maxDistanceCheck)
  {
    if (!canFire && cm >= startFireDistance)
    {
      Keyboard.write(_firStartedKey);
      canFire = true;
      currentDistance = 0;
      lvl1_HasLoaded = false;
      lvl2_HasLoaded = false;
      lvl3_HasLoaded = false;
    }
    if (canFire)
    {
      if (cm >= currentDistance)
      {
        currentDistance = cm;
        //Player is loading Goo
        if (!lvl3_HasLoaded && currentDistance >= distanceLvl2)
        {
          Keyboard.write(_distanceLvl3_Load);
          lvl3_HasLoaded = true;
          lvl2_HasLoaded = true;
          lvl1_HasLoaded = true;
        }
        else if (!lvl2_HasLoaded && currentDistance >= distanceLvl1)
        {
          Keyboard.write(_distanceLvl2_Load);
          lvl2_HasLoaded = true;
          lvl1_HasLoaded = true;
        }
        else if (!lvl1_HasLoaded && currentDistance >= startFireDistance)
        {
          Keyboard.write(_distanceLvl1_Load);
          lvl1_HasLoaded = true;
        }

        /*
        if (!lvl1_HasLoaded && currentDistance >= startFireDistance)
        {  
          Keyboard.write(_distanceLvl4_Load);
          lvl1_HasLoaded = true;
        }
        */
      }
      
        /*
        if (cm < endFireDistance)
        {
          //Player has fired
          Keyboard.write(_distanceLvl4_Fire);
        
          currentDistance = 0;
          canFire = false;
          delay(100);
        }
        */

      if (cm < endFireDistance)
      {
        //Player has fired
        if (currentDistance >= distanceLvl2)
        {
          Keyboard.write(_distanceLvl3_Fire);
        }
        else if (currentDistance >= distanceLvl1)
        {
          Keyboard.write(_distanceLvl2_Fire);
        }
        else
        {
          Keyboard.write(_distanceLvl1_Fire);
        }
        
        currentDistance = 0;
        canFire = false;
        previousShootCheck = delayAfterShooting + currentMillis;
        delay(100);
      }
    }
  }
/*
  Serial.print(cm);
  Serial.print("custom ");

  Serial.print(canFire);
  Serial.print(" canFire ");

  Serial.print(lvl1_HasLoaded);
  Serial.print(" currentDistance ");
  Serial.print(currentDistance);

  Serial.println();
  */
}

float microsecondsToCentimeters(long microseconds) {
  return microseconds / 29.0 / 2.0;
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
  buttonState3 = digitalRead(_upPin);  
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
/*
  if (buttonState3 != lastButtonState3) {
    if (buttonState3 == LOW) {
    Keyboard.press(_upKey);
    }
    else {
    Keyboard.release(_upKey);
    }
    delay(10);
  }

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
