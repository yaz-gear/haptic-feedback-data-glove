#include <Arduino.h>
#include <ESP32Servo.h>
#include <Wire.h>
#include <I2Cdev.h>
#include <MPU6050_6Axis_MotionApps20.h>
// this is initializing and defining everything
Servo servo1, servo2, servo3, servo4, servo5;

const int potPin1 = 34, potPin2 = 35, potPin3 = 32, potPin4 = 33, potPin5 = 25;
const int servoPin1 = 15, servoPin2 = 16, servoPin3 = 17, servoPin4 = 18, servoPin5 = 19;

MPU6050 mpu;
bool dmpReady = false;
uint16_t packetSize;

void setup() {
  Serial.begin(115200);
  Wire.begin();

  pinMode(potPin1, INPUT);
  pinMode(potPin2, INPUT);
  pinMode(potPin3, INPUT);
  pinMode(potPin4, INPUT);
  pinMode(potPin5, INPUT);

  servo1.attach(servoPin1);
  servo2.attach(servoPin2);
  servo3.attach(servoPin3);
  servo4.attach(servoPin4);
  servo5.attach(servoPin5);

  // Initialize to the position that is optimal based on calibration
  servo1.write(0);
  servo2.write(0);
  servo3.write(0);
  servo4.write(0);
  servo5.write(0);

  mpu.initialize();
  if (mpu.dmpInitialize() == 0) {
    mpu.setDMPEnabled(true);
    mpu.setFullScaleAccelRange(MPU6050_ACCEL_FS_2);  // Set accelerometer sensitivity to ±2g as this has shown the best results
    dmpReady = true;
    packetSize = mpu.dmpGetFIFOPacketSize();
    Serial.println("DMP enabled");
  } else {
    Serial.println("DMP Initialization failed");
    while (1);  
  }
}

void loop() {
  if (dmpReady) {
    while (mpu.getFIFOCount() < packetSize);

    if (mpu.getFIFOCount() == 1024) {
      mpu.resetFIFO();
      Serial.println("FIFO overflow!");
    } else {
      uint8_t fifoBuffer[64];
      mpu.getFIFOBytes(fifoBuffer, packetSize);

      Quaternion q;
      VectorFloat gravity;
      float ypr[3];
      VectorInt16 accel;
      mpu.dmpGetQuaternion(&q, fifoBuffer);
      mpu.dmpGetGravity(&gravity, &q);
      mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
      mpu.dmpGetAccel(&accel, fifoBuffer);  // Get accelerometer data

      int val1 = analogRead(potPin1);
      int val2 = analogRead(potPin2);
      int val3 = analogRead(potPin3);
      int val4 = analogRead(potPin4);
      int val5 = analogRead(potPin5);

      // Scaling accelerometer data to g
      float ax = accel.x / 16384.0;  // Assuming ±2g range
      float ay = accel.y / 16384.0;
      float az = accel.z / 16384.0;

      // Send all the data to unity
      Serial.print(val1); Serial.print(",");
      Serial.print(val2); Serial.print(",");
      Serial.print(val3); Serial.print(",");
      Serial.print(val4); Serial.print(",");
      Serial.print(val5); Serial.print(",");
      Serial.print(ypr[0] * 180/M_PI); Serial.print(",");
      Serial.print(ypr[1] * 180/M_PI); Serial.print(",");
      Serial.print(ypr[2] * 180/M_PI); Serial.print(",");
      Serial.print(ax); Serial.print(",");
      Serial.print(ay); Serial.print(",");
      Serial.println(az);
    }
  }

  // Listen for incoming commands from Unity to control servos
  if (Serial.available() > 0) {
    String input = Serial.readStringUntil('\n');
    int servoID, angle;
    sscanf(input.c_str(), "%d,%d", &servoID, &angle); // expecting "servoID,angle"
    if (servoID >= 1 && servoID <= 5 && angle >= 0 && angle <= 180) {
      switch (servoID) {
        case 1: servo1.write(angle); break;
        case 2: servo2.write(angle); break;
        case 3: servo3.write(angle); break;
        case 4: servo4.write(angle); break;
        case 5: servo5.write(angle); break;
      }
    }
  }

  delay(10);
}
