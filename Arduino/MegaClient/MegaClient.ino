// First simple ArduinoControl client, by Ed Mackey, 30 Nov 2014.

struct pin {
  int number;        // The Arduino pin number.  You can use "A0" etc for analog inputs.
  int defaultValue;  // For output pins, the initial value at startup and after the @RESET; command.
  int value;         // The current value (will be overwritten at startup).
  String name;       // The human-readable name of what hardware this pin controls.
};

// List of "analog" PWM output pins, with names.  Names should be
// escaped such that an extra backslash '\' is placed in front of
// any '\', '@', or double-quotes in the string.  This escaping
// must be in addition to the compiler's own escaping, so a single
// backslash becomes four here.  Better to avoid special chars.
pin analogOuts[] = {
  {  2, 0, 0, "Strip 1 Red"},
  {  3, 0, 0, "Strip 1 Green"},
  {  4, 128, 128, "Strip 1 Blue"},
  {  5, 0, 0, "Strip 2 Red"},
  {  6, 0, 0, "Strip 2 Green"},
  {  7, 128, 128, "Strip 2 Blue"},
  {  8, 0, 0, "Strip 3 Red"},
  {  9, 0, 0, "Strip 3 Green"},
  {  10, 128, 128, "Strip 3 Blue"},
  {  11, 0, 0, "Strip 4 Red"},
  {  12, 0, 0, "Strip 4 Green"},
  {  13, 128, 128, "Strip 4 Blue"},
  {  43, 0, 0, "Strip 5 Red"},
  {  44, 0, 0, "Strip 5 Green"},
  {  45, 128, 128, "Strip 5 Blue"}
  //{ 13, 255, 255, "Onboard LED"}
};

// List of digital (boolean) outputs with names, escaped as above.
// Default values must be 0 or 1 only.
pin digitalOuts[] = {
//  {  2, 1, 1, "Digital 2"},
//  {  4, 1, 1, "Digital 4"},
//  {  7, 1, 1, "Digital 7"},
//  {  8, 1, 1, "Digital 8"},
//  { 12, 1, 1, "Digital 12"},
//  { 13, 1, 1, "Digital 13"}
};

// List of analog inputs.  Default values are ignored.
pin analogIns[] = {
  { A0, 0, 0, "Input A0"},
//  { A1, 0, 0, "Input A1"},
//  { A2, 0, 0, "Input A2"},
//  { A3, 0, 0, "Input A3"},
//  { A4, 0, 0, "Input A4"},
//  { A5, 0, 0, "Input A5"}
};

// This calculates the number of pins.
int numAnalogOuts = sizeof(analogOuts) / sizeof(pin);
int numDigitalOuts = sizeof(digitalOuts) / sizeof(pin);
int numAnalogIns = sizeof(analogIns) / sizeof(pin);

void resetPinsToDefaults() {
  int i, pin;
  for (i = 0; i < numAnalogOuts; ++i) {
    pin = analogOuts[i].number;
    pinMode(pin, OUTPUT);
    analogOuts[i].value = analogOuts[i].defaultValue;
    analogWrite(pin, analogOuts[i].value);
  }
  for (i = 0; i < numDigitalOuts; ++i) {
    pin = digitalOuts[i].number;
    pinMode(pin, OUTPUT);
    digitalOuts[i].value = digitalOuts[i].defaultValue;
    digitalWrite(pin, digitalOuts[i].value);
  }
  for (i = 0; i < numAnalogIns; ++i) {
    pin = analogIns[i].number;
    pinMode(pin, INPUT);
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
  Serial.print("@LIST");
  int i;
  for (i = 0; i < numAnalogOuts; ++i) {
    Serial.print("|AO");
    Serial.print(analogOuts[i].number, DEC);
    Serial.print(":");
    Serial.print(analogOuts[i].value, DEC);
    Serial.print("\"");
    Serial.print(analogOuts[i].name);
    Serial.print("\"");
  }
  for (i = 0; i < numDigitalOuts; ++i) {
    Serial.print("|DO");
    Serial.print(digitalOuts[i].number, DEC);
    Serial.print(":");
    Serial.print(digitalOuts[i].value, DEC);
    Serial.print("\"");
    Serial.print(digitalOuts[i].name);
    Serial.print("\"");
  }
  for (i = 0; i < numAnalogIns; ++i) {
    Serial.print("|AI");
    Serial.print(analogIns[i].number, DEC);
    Serial.print(":");
    Serial.print(analogRead(analogIns[i].number), DEC);
    Serial.print("\"");
    Serial.print(analogIns[i].name);
    Serial.print("\"");
  }
  Serial.println(";");
}

void pollPins() {
  Serial.print("@POLL");
  int i;
  for (i = 0; i < numAnalogIns; ++i) {
    Serial.print("|A");
    Serial.print(analogIns[i].number, DEC);
    Serial.print(":");
    Serial.print(analogRead(analogIns[i].number), DEC);
  }
  Serial.println(";");
}

void requestReadFromPin(int pin) {
  boolean allowed = false;
  int i;
  for (i = 0; i < numAnalogIns; ++i) {
    if (pin == analogIns[i].number) {
      allowed = true;
      break;
    }
  }

  if (!allowed) {
    Serial.print("@ERROR: No such analog input pin ");
    Serial.print(pin, DEC);
    Serial.println(";");
  } else {
    Serial.print("@A");
    Serial.print(pin, DEC);
    Serial.print(":");
    Serial.print(analogRead(pin), DEC);
    Serial.println(";");
  }
}

void requestWriteToAnalogPin(int pin, int value) {
  boolean allowed = false;
  int i;
  for (i = 0; i < numAnalogOuts; ++i) {
    if (pin == analogOuts[i].number) {
      allowed = true;
      break;
    }
  }

  if (!allowed) {
    Serial.print("@ERROR: No such analog output pin ");
    Serial.print(pin, DEC);
    Serial.println(";");
  } else if ((value < 0) || (value > 255)) {
    Serial.println("@ERROR: Analog value must be between 0 and 255, inclusive;");
  } else {
    analogWrite(pin, value);
    analogOuts[i].value = value;
    Serial.println("@OK;");
  }
}

void requestWriteToDigitalPin(int pin, int value) {
  boolean allowed = false;
  int i;
  for (i = 0; i < numDigitalOuts; ++i) {
    if (pin == digitalOuts[i].number) {
      allowed = true;
      break;
    }
  }

  if (!allowed) {
    Serial.print("@ERROR: No such digital output pin ");
    Serial.print(pin, DEC);
    Serial.println(";");
  } else if ((value < 0) || (value > 1)) {
    Serial.println("@ERROR: Digital value must be 0 or 1;");
  } else {
    digitalWrite(pin, (value > 0) ? HIGH : LOW);
    digitalOuts[i].value = value;
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
  if (Serial.available() > 0) {
    incomingByte = Serial.read();

    if (incomingByte == '@') {
      expect = 1;
      incomingPin = 0;
      incomingValue = 0;
    } else switch (expect) {
      case 1: // expect command char, such as 'A' or 'D'
        expect = 0;
        if ((incomingByte == 'A') || (incomingByte == 'D') || (incomingByte == 'I')) {
          incomingModeIsAnalog = (incomingByte == 'A');
          incomingOperationIsOutput = (incomingByte != 'I');
          expect = 2;
        } else if (incomingByte == 'R') {
          resetPinsToDefaults();
          Serial.println("@RESET;");
        } else if (incomingByte == 'L') {
          listPins();
        } else if (incomingByte == 'P') {
          pollPins();
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
        break;
    }
  }
}

