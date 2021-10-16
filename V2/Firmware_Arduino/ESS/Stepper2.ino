/*
 * Stepper.cpp - Stepper library for Wiring/Arduino - Version 1.1.0
 *
 * Original library        (0.1)   by Tom Igoe.
 * Two-wire modifications  (0.2)   by Sebastian Gassner
 * Combination version     (0.3)   by Tom Igoe and David Mellis
 * Bug fix for four-wire   (0.4)   by Tom Igoe, bug fix from Noah Shibley
 * High-speed stepping mod         by Eugene Kozlenko
 * Timer rollover fix              by Eugene Kozlenko
 * Five phase five wire    (1.1.0) by Ryan Orendorff
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 *
 *
 * Drives a unipolar, bipolar, or five phase stepper motor.
 *
 * When wiring multiple stepper motors to a microcontroller, you quickly run
 * out of output pins, with each motor requiring 4 connections.
 *
 * By making use of the fact that at any time two of the four motor coils are
 * the inverse of the other two, the number of control connections can be
 * reduced from 4 to 2 for the unipolar and bipolar motors.
 *
 * A slightly modified circuit around a Darlington transistor array or an
 * L293 H-bridge connects to only 2 microcontroler pins, inverts the signals
 * received, and delivers the 4 (2 plus 2 inverted ones) output signals
 * required for driving a stepper motor. Similarly the Arduino motor shields
 * 2 direction pins may be used.
 *
 * The sequence of control signals for 5 phase, 5 control wires is as follows:
 *
 * Step C0 C1 C2 C3 C4
 *    1  0  1  1  0  1
 *    2  0  1  0  0  1
 *    3  0  1  0  1  1
 *    4  0  1  0  1  0
 *    5  1  1  0  1  0
 *    6  1  0  0  1  0
 *    7  1  0  1  1  0
 *    8  1  0  1  0  0
 *    9  1  0  1  0  1
 *   10  0  0  1  0  1
 *
 * The sequence of control signals for 4 control wires is as follows:
 *
 * Step C0 C1 C2 C3
 *    1  1  0  1  0
 *    2  0  1  1  0
 *    3  0  1  0  1
 *    4  1  0  0  1
 *
 * The sequence of controls signals for 2 control wires is as follows
 * (columns C1 and C2 from above):
 *
 * Step C0 C1
 *    1  0  1
 *    2  1  1
 *    3  1  0
 *    4  0  0
 *
 * The circuits can be found at
 *
 * http://www.arduino.cc/en/Tutorial/Stepper
 */

#include "Arduino.h"
#include "Stepper2.h"



/*
 *   constructor for four-pin version
 *   Sets which wires should control the motor.
 */
Stepper2::Stepper2(int number_of_steps, int motor_pin_1, int motor_pin_2,
                                      int motor_pin_3, int motor_pin_4)
{
  this->step_number = 0;    // which pumpStep the motor is on

  // Arduino pins for the motor control connection:
  this->motor_pin_1 = motor_pin_1;
  this->motor_pin_2 = motor_pin_2;
  this->motor_pin_3 = motor_pin_3;
  this->motor_pin_4 = motor_pin_4;

  // setup the pins on the microcontroller:
  pinMode(this->motor_pin_1, OUTPUT);
  pinMode(this->motor_pin_2, OUTPUT);
  pinMode(this->motor_pin_3, OUTPUT);
  pinMode(this->motor_pin_4, OUTPUT);
}


/*
 * Moves the motor steps_to_move steps.  If the number is negative,
 * the motor moves in the reverse direction.
 */
void Stepper2::pumpStep(int8_t direction)
{
  // increment or decrement the pumpStep number,
  // depending on direction:
  if (direction == -1)
    this->step_number++;
  else
    this->step_number--;
  stepMotor(this->step_number % 8);
}

/*
 * Moves the motor forward or backwards.
 */               
void Stepper2::switchOff()
{
  digitalWrite(motor_pin_1, LOW);
  digitalWrite(motor_pin_2, LOW);
  digitalWrite(motor_pin_3, LOW);
  digitalWrite(motor_pin_4, LOW);
}
//uint8_t stepsMatrix[8][4] = {
//  { LOW, LOW, LOW, HIGH },
//  { LOW, LOW, HIGH, HIGH },
//  { LOW, LOW, HIGH, LOW },
//  { LOW, HIGH, HIGH, LOW },
//  { LOW, HIGH, LOW, LOW },
//  { HIGH, HIGH, LOW, LOW },
//  { HIGH, LOW, LOW, LOW },
//  { HIGH, LOW, LOW, HIGH },
//};
uint8_t stepsMatrix[8][4] = {
  { HIGH, LOW, HIGH, LOW }, // 10 10  D
  { LOW, LOW, HIGH, LOW },  // 00 10  
  { LOW, HIGH, HIGH, LOW},  // 01 10  D
  { LOW, HIGH, LOW, LOW },  // 01 00  
  { LOW, HIGH, LOW, HIGH }, // 01 01  D
  { LOW, LOW, LOW, HIGH },  // 00 01  
  { HIGH, LOW, LOW, HIGH},  // 10 01  D
  { HIGH, LOW, LOW, LOW },  // 10 00  
};
void Stepper2::writeStep(uint8_t thisStep[])
{
  digitalWrite(motor_pin_1, thisStep[0]);
  digitalWrite(motor_pin_2, thisStep[1]);
  digitalWrite(motor_pin_3, thisStep[2]);
  digitalWrite(motor_pin_4, thisStep[3]);
}
void Stepper2::stepMotor(int thisStep)
{
  writeStep(stepsMatrix[thisStep]);
}
