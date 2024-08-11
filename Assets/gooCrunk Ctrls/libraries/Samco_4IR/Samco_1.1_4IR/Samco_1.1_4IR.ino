/*!
 * @file Samco_1.1_4IR.ino
 * @brief 4 Button Light Gun sketch for 4 LED setup
 * @n INO file for Samco Light Gun 4 LED setup
 *
 * @copyright   Samco, https://github.com/samuelballantyne, April 2020
 * @copyright   GNU Lesser General Public License
 *
 * @author [Sam Ballantyne](samuelballantyne@hotmail.com)
 * @version  V1.0
 * @date  2020
 */

int xCenter = 512;              // If second calibration seems more accurate you can replace these values with the altered camera center values from serial monitor
int yCenter = 368;

int finalX = 0;                 // Values after tilt correction
int finalY = 0;

int xLeft = 0;                  // Stored calibration points
int yTop = 0;
int xRight = 0;
int yBottom = 0;

int MoveXAxis = 0;              // Unconstrained mouse postion
int MoveYAxis = 0;               

int conMoveXAxis = 0;           // Constrained mouse postion
int conMoveYAxis = 0;           

int count = 4;                  // Set intial count

int caliPin = 11;               // Set Calibration Pin (change to A4 to use ALT Pin)
int leftPin = 12;               // Set Left Mouse Pin
int rightPin = 7;              // Set Right Mouse Pin
int middlePin = 9;             // Set Middle Mouse Pin
int pedalPin = A1;              // Set Pedal Pin

int buttonState1 = 0;           // Set Button states
int lastButtonState1 = 0;
int buttonState2 = 0;
int lastButtonState2 = 0;
int buttonState3 = 0;
int lastButtonState3 = 0;
int buttonState4 = 0;         
int lastButtonState4 = 0;
int buttonState5 = 0;         
int lastButtonState5 = 0;      

#include <HID.h>                // Load libraries
#include <Wire.h>
#include <AbsMouse.h>
#include <DFRobotIRPosition.h>
#include <Samco.h>

DFRobotIRPosition myDFRobotIRPosition;
Samco mySamco;

int res_x = 1023;              // UPDATE: These values do not need to change
int res_y = 768;               // UPDATE: These values do not need to change


void setup() {

  pinMode(5, OUTPUT);                     // Needed for IstyBitsy 32u4 5V and other 32u4 boards    
  digitalWrite (5, HIGH);                 // This pin is used to power the IR Camera when using Samco PCB
  delay(500);
  
  Serial.begin(9600);                     // For debugging (make sure your serial monitor has the same baud rate)

  AbsMouse.init(res_x, res_y);            

  pinMode(caliPin, INPUT_PULLUP);         // Set pin modes
  pinMode(leftPin, INPUT_PULLUP);
  pinMode(rightPin, INPUT_PULLUP);
  pinMode(middlePin, INPUT_PULLUP); 
  pinMode(pedalPin, INPUT_PULLUP);      

  myDFRobotIRPosition.begin();            // Start IR Camera

  AbsMouse.move((res_x / 2), (res_y / 2));          // Set mouse position to centre of the screen
  
  delay(500);
  
}




void loop() {

  if (count > 3) {

    getPosition();
    mouseButtons();
    PrintResults();
    go();

  }

  /* ------------------ START/PAUSE MOUSE ---------------------- */


  else if (count > 2 ) {


    skip();
    mouseCount();


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
    footPedal();
    getPosition();

    MoveXAxis = map (finalX, xLeft, xRight, 300, (res_x - 300));
    MoveYAxis = map (finalY, yTop, yBottom, 200, (res_y - 200));
    conMoveXAxis = constrain (MoveXAxis, 0, res_x);
    conMoveYAxis = constrain (MoveYAxis, 0, res_y);
    
    PrintResults();
    reset();



  }

}


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

  buttonState1 = digitalRead(caliPin);

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

  buttonState2 = digitalRead(leftPin);
  buttonState3 = digitalRead(rightPin);
  buttonState4 = digitalRead(middlePin);     

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
      AbsMouse.press(MOUSE_RIGHT);
    }
    else {
      AbsMouse.release(MOUSE_RIGHT);
    }
    delay(10);
  }

  if (buttonState4 != lastButtonState4) {     
    if (buttonState4 == LOW) {
      AbsMouse.press(MOUSE_MIDDLE);
    }
    else {
      AbsMouse.release(MOUSE_MIDDLE);
    }
    delay(10);
  }

  lastButtonState2 = buttonState2;
  lastButtonState3 = buttonState3;
  lastButtonState4 = buttonState4;            
}

void footPedal() {    // Setup Left, Right & Middle Mouse buttons

  buttonState5 = digitalRead(pedalPin);     

  if (buttonState5 != lastButtonState5) {     
    if (buttonState5 == LOW) {
      AbsMouse.press(MOUSE_RIGHT);
    }
    else {
      AbsMouse.release(MOUSE_RIGHT);
    }
    delay(10);
  }

  lastButtonState5 = buttonState5;            
}

void mouseCount() {    // Set count down on trigger

  buttonState2 = digitalRead(leftPin);

  if (buttonState2 != lastButtonState2) {
    if (buttonState2 == LOW) {
      count--;
    }
    else { // do nothing
    }
    delay(10);
  }

  lastButtonState2 = buttonState2;
}



void reset() {    // Pause/Re-calibrate button

  buttonState1 = digitalRead(caliPin);

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

  buttonState1 = digitalRead(caliPin);

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
