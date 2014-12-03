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
  {  5, 255, 255, "Output A"},
  { 10, 255, 255, "Output B"},
  {  9, 255, 255, "Output C"},
  { 11, 255, 255, "Output D"},
  { 13, 255, 255, "Output E"},
  {  6, 255, 255, "Output F"}
};

// List of digital (boolean) outputs with names, escaped as above.
// Default values must be 0 or 1 only.
pin digitalOuts[] = {
  {  7, 1, 1, "Green 1"},
  {  2, 1, 1, "Green 2"},
  {  1, 1, 1, "Green 3"},
  {  3, 1, 1, "Green 4"},
  { 12, 1, 1, "Green 5"},
  {  4, 1, 1, "Green 6"}
};

// This calculates the number of pins.
int numAnalogOuts = sizeof(analogOuts) / sizeof(pin);
int numDigitalOuts = sizeof(digitalOuts) / sizeof(pin);

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
  for (int i = 0; i < numAnalogOuts; ++i) {
    Serial.print("|AO");
    Serial.print(analogOuts[i].number, DEC);
    Serial.print(":");
    Serial.print(analogOuts[i].value, DEC);
    Serial.print("\"");
    Serial.print(analogOuts[i].name);
    Serial.print("\"");
  }
  for (int i = 0; i < numDigitalOuts; ++i) {
    Serial.print("|DO");
    Serial.print(digitalOuts[i].number, DEC);
    Serial.print(":");
    Serial.print(digitalOuts[i].value, DEC);
    Serial.print("\"");
    Serial.print(digitalOuts[i].name);
    Serial.print("\"");
  }
  Serial.println(";");
}

void requestReadFromPin(int pin) {
  Serial.println("@ERROR: Not implemented yet;");
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
    Serial.print("@ERROR: No such analog pin ");
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
    Serial.print("@ERROR: No such digital pin ");
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
        if ((incomingByte == 'A') || (incomingByte == 'D') || (incomingByte == 'P')) {
          incomingModeIsAnalog = (incomingByte == 'A');
          incomingOperationIsOutput = (incomingByte != 'P');
          expect = 2;
        } else if (incomingByte == 'R') {
          resetPinsToDefaults();
          Serial.println("@RESET;");
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
        break;
    }
  }
}

