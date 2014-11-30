// First simple ArduinoControl client

int analogOuts[] = { 5, 10, 9, 11, 13, 6 };
int digitalOuts[] = { 7, 2, 1, 3, 12, 4 };

int numAnalogOuts = sizeof(analogOuts) / sizeof(int);
int numDigitalOuts = sizeof(digitalOuts) / sizeof(int);

void resetPinsToDefaults() {
  for (int i = 0; i < numDigitalOuts; ++i) {
    pinMode(digitalOuts[i], OUTPUT);
    digitalWrite(digitalOuts[i], HIGH);
  }
}

void setup() {
  resetPinsToDefaults();
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
    // Note this really does hang until you open a serial monitor or other connection.
  }
  Serial.println("@RESET;");
}

void listPins() {
  Serial.println("TODO: list of pins");
}

void requestReadFromPin(int pin) {
  Serial.print("TODO: Read from pin ");
  Serial.println(pin, DEC);
}

void requestWriteToAnalogPin(int pin, int value) {
  boolean allowed = false;
  for (int i = 0; i < numAnalogOuts; ++i) {
    if (pin == analogOuts[i]) {
      allowed = true;
      break;
    }
  }

  if (!allowed) {
    Serial.print("@ERROR: No such analog pin ");
    Serial.print(pin, DEC);
    Serial.println(";");
  } else if ((value < 0) || (value > 255)) {
    Serial.println("@ERROR: Analog value must be between 0 and 255, inclusive;");
  } else {
    analogWrite(pin, value);
    Serial.println("@OK;");
  }
}

void requestWriteToDigitalPin(int pin, int value) {
  boolean allowed = false;
  for (int i = 0; i < numDigitalOuts; ++i) {
    if (pin == digitalOuts[i]) {
      allowed = true;
      break;
    }
  }

  if (!allowed) {
    Serial.print("@ERROR: No such digital pin ");
    Serial.print(pin, DEC);
    Serial.println(";");
  } else if ((value < 0) || (value > 1)) {
    Serial.println("@ERROR: Digital value must be 0 or 1;");
  } else {
    digitalWrite(pin, (value > 0) ? HIGH : LOW);
    Serial.println("@OK;");
  }
}

// Expectations:
// 0 - expect next command to start with '@'
// 1 - expect command char, such as 'A' or 'D'
// 2 - expect pin number followed by ':'
// 3 - expect value to write followed by ';'
int expect = 0;
int incomingByte;
int incomingPin;
int incomingValue;
boolean incomingModeIsAnalog;
boolean incomingOperationIsOutput;

void loop() {
 // send data only when you receive data:
 if (Serial.available() > 0) {
   // read the incoming byte:
   incomingByte = Serial.read();

   switch (expect) {
     case 1: // expect command char, such as 'A' or 'D'
       expect = 0;
       if ((incomingByte == 'A') || (incomingByte == 'D') || (incomingByte == 'P')) {
         incomingModeIsAnalog = (incomingByte == 'A');
         incomingOperationIsOutput = (incomingByte != 'P');
         expect = 2;
       } else if (incomingByte == 'R') {
         resetPinsToDefaults();
       } else if (incomingByte == 'L') {
         listPins();
       }
       break;
     case 2: // expect pin number
       if ((incomingByte >= '0') && (incomingByte <= '9')) {
         incomingPin = incomingPin * 10 + (incomingByte - '0');
       } else if (incomingOperationIsOutput && (incomingByte == ':')) {
         expect = 3;
       } else if ((!incomingOperationIsOutput) && (incomingByte == ';')) {
         expect = 0;
         requestReadFromPin(incomingPin);
       } else {
         expect = 0;
       }
       break;
     case 3: // expect value to write
       if ((incomingByte >= '0') && (incomingByte <= '9')) {
         incomingValue = incomingValue * 10 + (incomingByte - '0');
       } else if (incomingByte == ';') {
         expect = 0;
         if (incomingModeIsAnalog) {
           requestWriteToAnalogPin(incomingPin, incomingValue);
         } else {
           requestWriteToDigitalPin(incomingPin, incomingValue);
         }
       } else {
         expect = 0;
       }
       break;
     default:
       if (incomingByte == '@') {
         expect = 1;
         incomingPin = 0;
         incomingValue = 0;
       }
   }
 }
}

