# Haptic Feedback Data Glove

## Overview
This project presents a low-cost haptic feedback data glove designed to enhance realism in VR-based training simulations. The system provides real-time finger tracking and force feedback using a combination of mechanical actuation and sensor data, offering an affordable alternative to existing commercial solutions.

## Problem Statement
Most current training simulations lack realistic haptic feedback, which is essential for tasks requiring precise hand movement. Existing solutions are either prohibitively expensive or fail to deliver meaningful force feedback. This project aims to develop an affordable data glove that delivers realistic force-based haptic feedback while remaining accessible and open for further development.

## System Description
The data glove integrates:
- Potentiometers for finger position tracking
- Servo motors and a pulley-based mechanism for force feedback
- An MPU (accelerometer) for hand motion tracking
- An ESP32 microcontroller for sensor acquisition and actuator control
- A custom DLL to enable communication between Unity and the Arduino
- Unity integration for VR interaction and visualization

The system continuously reads sensor data, processes finger and hand movement, and applies controlled resistive forces to simulate interaction with virtual objects.

## My Contribution
This project was fully designed and implemented by me as part of my MSc in Advanced Robotics. My contributions include:
- Mechanical design of the glove and pulley-based force feedback system
- Selection and integration of sensors and actuators
- Development of Arduino firmware for real-time control
- Creation of a custom DLL to translate SteamVR data and communicate with the Arduino
- Unity integration for VR interaction
- System testing and performance evaluation

## Technologies Used
- ESP32
- Servo motors, potentiometers, MPU (accelerometer)
- C / C++ (embedded and DLL development)
- Unity 
- CAD (3D modeling and mechanical design)

## Results
- Achieved real-time finger tracking with responsive force feedback
- Successfully integrated the glove into a VR environment
- Demonstrated a functional and affordable alternative to commercial haptic gloves

## Repository Structure

## Future Improvements
- Improved force calibration and realism
- Wireless communication
- Enhanced ergonomics and durability
- Expanded VR interaction scenarios

## Disclaimer
This project is inspired by prior academic work in the field of haptic interfaces and other similar works in the field, which is cited in the project report. All code, mechanical designs, and system integration presented here were independently developed for this project.
