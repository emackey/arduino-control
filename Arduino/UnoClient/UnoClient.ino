// First simple ArduinoControl client, by Ed Mackey, 30 Nov 2014.

int baudRate = 19200; // default is 9600.

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
  {  3, 255, 255, "PWM 3"},
  {  5, 255, 255, "PWM 5"},
  {  6, 255, 255, "PWM 6"},
  {  9, 255, 255, "PWM 9"},
  { 10, 255, 255, "PWM 10"},
  { 11, 255, 255, "PWM 11"}
};

// List of digital (boolean) outputs with names, escaped as above.
// Default values must be 0 or 1 only.
pin digitalOuts[] = {
  {  2, 1, 1, "Digital 2"},
  {  4, 1, 1, "Digital 4"},
  {  7, 1, 1, "Digital 7"},
  {  8, 1, 1, "Digital 8"},
  { 13, 1, 1, "Onboard LED"}
};

// List of analog inputs.  Default values are ignored.
pin analogIns[] = {
  { A0, 0, 0, "Input A0"},
  { A1, 0, 0, "Input A1"},
  { A2, 0, 0, "Input A2"},
  { A3, 0, 0, "Input A3"},
  { A4, 0, 0, "Input A4"},
  { A5, 0, 0, "Input A5"}
};

// List of digital (boolean) inputs with names, escaped as above.
// Default values are ignored.
pin digitalIns[] = {
  { 12, 1, 1, "Digital Input 12"}
};

// This calculates the number of pins.
int numAnalogOuts = sizeof(analogOuts) / sizeof(pin);
int numDigitalOuts = sizeof(digitalOuts) / sizeof(pin);
int numAnalogIns = sizeof(analogIns) / sizeof(pin);
int numDigitalIns = sizeof(digitalIns) / sizeof(pin);

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
  for (i = 0; i < numDigitalIns; ++i) {
    pin = digitalIns[i].number;
    pinMode(pin, INPUT_PULLUP);
  }
}

void setup() {
  resetPinsToDefaults();
  Serial.begin(baudRate);
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
    analogIns[i].value = analogRead(analogIns[i].number);
    Serial.print("|AI");
    Serial.print(analogIns[i].number, DEC);
    Serial.print(":");
    Serial.print(analogIns[i].value, DEC);
    Serial.print("\"");
    Serial.print(analogIns[i].name);
    Serial.print("\"");
  }
  for (i = 0; i < numDigitalIns; ++i) {
    digitalIns[i].value = digitalRead(digitalIns[i].number);
    Serial.print("|DI");
    Serial.print(digitalIns[i].number, DEC);
    Serial.print(":");
    Serial.print(digitalIns[i].value, DEC);
    Serial.print("\"");
    Serial.print(digitalIns[i].name);
    Serial.print("\"");
  }
  Serial.println(";");
}

void pollPins() {
  Serial.print("@POLL");
  int i;
  for (i = 0; i < numAnalogIns; ++i) {
    analogIns[i].value = analogRead(analogIns[i].number);
    Serial.print("|A");
    Serial.print(analogIns[i].number, DEC);
    Serial.print(":");
    Serial.print(analogIns[i].value, DEC);
  }
  for (i = 0; i < numDigitalIns; ++i) {
    digitalIns[i].value = digitalRead(digitalIns[i].number);
    Serial.print("|D");
    Serial.print(digitalIns[i].number, DEC);
    Serial.print(":");
    Serial.print(digitalIns[i].value, DEC);
  }
  Serial.println(";");
}

int millisBetweenPushes = 16;
int lastPushMillis = 0;

void pushUpdates() {
  int now = millis();
  if ((now >= lastPushMillis) && (now < (lastPushMillis + millisBetweenPushes))) {
    return;
  }
  lastPushMillis = now;
  
  int i;
  int currentValue;
  boolean isPushing = false;
  for (i = 0; i < numDigitalIns; ++i) {
    currentValue = digitalRead(digitalIns[i].number);
    if (currentValue != digitalIns[i].value) {
      digitalIns[i].value = currentValue;
      if (!isPushing) {
        isPushing = true;
        Serial.print("@PUSH");
      }
      Serial.print("|D");
      Serial.print(digitalIns[i].number, DEC);
      Serial.print(":");
      Serial.print(digitalIns[i].value, DEC);
    }
  }
  
  if (isPushing) {
    Serial.println(";");
  }
}

void requestReadFromPin(int pin) {
  int i;
  for (i = 0; i < numAnalogIns; ++i) {
    if (pin == analogIns[i].number) {
      analogIns[i].value = analogRead(pin);
      Serial.print("@A");
      Serial.print(pin, DEC);
      Serial.print(":");
      Serial.print(analogIns[i].value, DEC);
      Serial.println(";");
      return;
    }
  }
  for (i = 0; i < numDigitalIns; ++i) {
    if (pin == digitalIns[i].number) {
      digitalIns[i].value = digitalRead(pin);
      Serial.print("@D");
      Serial.print(pin, DEC);
      Serial.print(":");
      Serial.print(digitalIns[i].value, DEC);
      Serial.println(";");
      return;
    }
  }

  Serial.print("@ERROR: No such input pin ");
  Serial.print(pin, DEC);
  Serial.println(";");
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
  pushUpdates();
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

