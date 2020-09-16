/*
 Name:		SPRFirmWareNano.ino
 Created:	9/23/2019 2:17:30 PM
 Author:	techBOY
*/

#include "Stepper2.h"
#include "HashTable.h"
// hardware pin map
#define xStepPin		2
#define xDirectionPin	13
#define xLimitPin		8
#define xNegatve		0
#define xPositive		(!xNegatve)
#define yStepPin		5
#define yDirectionPin	4
#define yLimitPin		12		   
#define yNegatve		0
#define yPositive		(!yNegatve)

#define pLimitPin		A5
#define HE				11
#define H1A				9
#define H1B				10
#define H2A				A4
#define H2B				6 
Stepper2 stepper = Stepper2(8, H1B, H1A, H2A, H2B);


int8_t directions[3] = { 0,0,0 };
//float mmPerStep[3] = { 0.0075,0.0075, 0.01 };
float mmPerStep[3] = { 0.0150,0.0150, 0.01 };
float limits[3] = { 55,55,30 };
float currentPositions[3] = { 0,0,0 };
//void gotoXY(float x, float y, float speed);
bool step(uint8_t channel, int8_t direction, bool dontDelay = false);


void SendStatus(String xyStatus, String pumpStatus, float progress)
{
	Serial.print(F("status: "));
	Serial.print(F("x = "));
	Serial.print(currentPositions[0], 3);
	Serial.print(F(", y = "));
	Serial.print(currentPositions[1], 3);	
	Serial.print(F(", progress = "));
	Serial.print(min(100, progress), 0);
	Serial.print(F(", xy stage = "));
	Serial.print(xyStatus);
	Serial.print(F(", pump = "));
	Serial.println(pumpStatus);
}
void Info(String tag, String message)
{
	Serial.print(F("info: tag = "));
	Serial.print(tag);
	Serial.print(F(", message = "));
	Serial.println(message);
}
void Error(String tag, String message)
{}
void setup() {
	Serial.begin(115200);
	Serial.setTimeout(100);
	pinMode(xStepPin, 1);
	pinMode(xDirectionPin, 1);
	pinMode(xLimitPin, 0);
	digitalWrite(xLimitPin, 1);
	pinMode(yStepPin, 1);
	pinMode(yDirectionPin, 1);
	pinMode(yLimitPin, 0);
	digitalWrite(yLimitPin, 1);
	pinMode(pLimitPin, 0);
	digitalWrite(pLimitPin, 1);
	pinMode(H1A, 1);
	pinMode(H1B, 1);
	pinMode(H2A, 1);
	pinMode(H2B, 1);

	// enable pin PWM for h bridge
	pinMode(11, OUTPUT);  // PB3, OC2A
	pinMode(3, OUTPUT);	  // PD3, OC2B
	TCCR2A = _BV(COM2B1) | _BV(COM2A1) | _BV(WGM21) | _BV(WGM20);
	TCCR2B = _BV(CS21);

	OCR2A = 180;
	OCR2B = 255 - (140); // XY motors. Smaller means more power full. [0, 255]

	//gotoXY(45, 45, 6);
	Serial.println(F("begin"));
}
//bool pumpLoop()
//{
//	if (pumpRate <= 0)
//	{
//		stepper.switchOff();
//		pumpProg = -1;
//		return false;
//	}
//	// check physical limit
//	if (currentPositions[2] <= 0) // stop
//	{
//		if (pumpRate != 0)
//		{
//			pumpRate = 0;
//			Serial.println("pump error: message = The pump has reached its physical limit.");
//			return false;
//		}
//	}
//	if (currentPositions[2] <= pointToPumpTo) // volume limit reached
//	{
//		pumpRate = 0;
//		Serial.println("pump end: message = The required volume has been pumped.");
//		return false;
//	}
//	if (millis() - pumpStartedAtT > timeToPump && timeToPump > 0)
//	{
//		pumpRate = 0;
//		Serial.println("pump end: message = The pump has stopped.");
//		return false;
//	}
//
//	float t = (float)(millis() - pumpStartedAtT)/ 1000.0F; // time from pump start
//	float requiredPoint = pointPumpStartedFrom - pumpRate * t;
//
//	if (currentPositions[2] > requiredPoint)
//	{
//		step(2, -1, true);
//		delay(1); // the other 1 by the other loop
//		return true;
//	}
//	return false;
//}
void setDirection(uint8_t channel, int8_t direction)
{
	if (directions[channel] == direction)
		return;
	directions[channel] = direction;
	if (channel == 0)
	{
		if (direction == 1)
			digitalWrite(xDirectionPin, xPositive);
		else
			digitalWrite(xDirectionPin, xNegatve);
	}
	else if (channel == 1)
	{
		if (direction == 1)
			digitalWrite(yDirectionPin, yPositive);
		else
			digitalWrite(yDirectionPin, yNegatve);
	}
	else if (channel == 2)
	{
		// we give stepper the direction at the time of the step
	}
}
bool checkLimit(uint8_t channel)
{
	if (channel == 0)
		return !digitalRead(xLimitPin);
	else if (channel == 1)
		return !digitalRead(yLimitPin);
	else
		return digitalRead(pLimitPin);
}

bool step(uint8_t channel, int8_t direction, bool dontDelay)
{
	setDirection(channel, direction);
	bool wasMoved = false;
	if (channel == 0)
	{
		digitalWrite(xStepPin, 1);
		digitalWrite(xStepPin, 0);
		if (!dontDelay)
			delay(1);
		wasMoved = true;
	}
	else if (channel == 1)
	{
		digitalWrite(yStepPin, 1);
		digitalWrite(yStepPin, 0);
		if (!dontDelay)
			delay(1);
		wasMoved = true;
	}
	else
	{
		stepper.step(directions[channel]);
		if (!dontDelay)
			delay(3);
		else
			delay(2);
		wasMoved = true;
	}
	currentPositions[channel] += directions[channel] * mmPerStep[channel];
	return wasMoved;
}
void MoveToHome(int channel)
{
	int8_t homeDir = 0;
	if (channel == 0)
		homeDir = -1;
	else if (channel == 1)
		homeDir = -1;
	else
		homeDir = 1;
	int foundCount = 0;
	for (int i = 0; i < limits[channel] / mmPerStep[channel]; i++)
	{
		if (checkLimit(channel))
			foundCount++;
		else if (foundCount > 0)
			foundCount--;
		if (foundCount > 5)
		{
			break;
		}
		step(channel, homeDir);
		delay(1);
	}
	delay(500);
	for (int i = 0; i < 20; i++)
	{
		step(channel, -homeDir);
		delay(1);
	}			   
	if (foundCount > 5)
	{
		Info(F("homing"), String(F("Found the limit")) + String(channel));
	}
	else													   
		Error(F("homing"), String(F("Home not found for the motor ")) + String(channel));

}

String trimString(String str) {
	while (str.startsWith(F(" ")))
		str = str.substring(1);
	while (str.endsWith(F(" ")))
		str = str.substring(0, str.length() - 1);
	return str;
}
String getCommand(String CommandRaw)
{
	if (CommandRaw.indexOf(F(":")) >= 0)
		return trimString(CommandRaw.substring(0, CommandRaw.indexOf(F(":"))));
	return CommandRaw;
}
void getArgs(String CommandRaw, HashTable& Args)
{
	String argsPart;
	String comPart;
	if (CommandRaw.indexOf(F(":")) < 0) // no args
	{
		comPart = CommandRaw;
		argsPart = F("");
	}
	else
	{
		comPart = getCommand(CommandRaw);
		argsPart = CommandRaw.substring(comPart.length() + 1);
	}

	while (trimString(argsPart).length() > 0)
	{
		String pair = argsPart;
		if (argsPart.indexOf(F(",")) >= 0)
		{
			pair = argsPart.substring(0, argsPart.indexOf(F(",")));
			argsPart = trimString(argsPart.substring(argsPart.indexOf(F(",")) + 1));
		}
		else
		{
			pair = argsPart;
			argsPart = F("");
		}
		if (pair.indexOf(F("=")) < 0)
			continue;
		String key = trimString(pair.substring(0, pair.indexOf("=")));
		String value = trimString(pair.substring(pair.indexOf("=") + 1));
		Args.Add(key, value);
	}
}
//void gotoXY(float x, float y, float speed)
//{
//	Serial.print(F("GotoXY: x = "));
//	Serial.print(x);
//	Serial.print(F(", y = "));
//	Serial.print(y);
//	Serial.print(F(", speed = "));
//	Serial.print(speed);
//	float totalMoveX = x - currentPositions[0];
//	float totalMoveY = y - currentPositions[1];
//
//	Serial.print(F(", TotalMoveX = "));
//	Serial.print(totalMoveX);
//	Serial.print(F(", TotalMoveY = "));
//	Serial.print(totalMoveY);
//
//	float distance = sqrt(pow(y - currentPositions[1], 2) + pow(x - currentPositions[0], 2));
//
//	Serial.print(F(", Distance = "));
//	Serial.print(distance);
//	float angle = atan2(y - currentPositions[1], x - currentPositions[0]);
//	Serial.print(F(", angle = "));
//	Serial.print(angle);
//	float xSpeed = speed * cos(angle);
//	float ySpeed = speed * sin(angle);
//	float startX = currentPositions[0];
//	float startY = currentPositions[1];
//	Serial.print(F(", X speed = "));
//	Serial.print(xSpeed);
//	Serial.print(F(", Y speed = "));
//	Serial.println(ySpeed);
//
//	float startT = (float)millis() / 1000.0F;
//	while (true)
//	{
//		if (rasterAbort)
//			return;
//		if (EmergencyComs())
//		{
//			Error(F("main"), F("The move operation was aborted"));
//			return;
//		}
//		float t = (float)millis() / 1000.0F - startT;
//		/*Serial.print(F("t = "));
//		Serial.println(t);*/
//		float expectedX = startX + xSpeed * t;
//		float expectedY = startY + ySpeed * t;
//
//		if (x == startX)
//		{
//			if (y - startY > 0) // positive
//			{
//				if (currentPositions[1] >= y)
//					return;
//			}
//			else
//			{
//				if (currentPositions[1] <= y)
//					return;
//			}
//		}
//		else
//		{
//			if (x - startX > 0) // positive
//			{
//				if (currentPositions[0] >= x)
//					return;
//			}
//			else
//			{
//				if (currentPositions[0] <= x)
//					return;
//			}
//		}
//		float toMoveXY[2] = { 
//			expectedX - currentPositions[0],
//			expectedY - currentPositions[1] };
//		/*Serial.print(F("cX = "));
//		Serial.println(currentPositions[0], 4);
//		Serial.print(F("cY = "));
//		Serial.println(currentPositions[1], 4);
//		Serial.print(F("mX = "));
//		Serial.println(toMoveXY[0], 4);
//		Serial.print(F("mY = "));
//		Serial.println(toMoveXY[1], 4);*/
//		bool wasMoved = false;
//
//		for (int channel = 0; channel < 2; channel++)
//		{
//			//int channel = 0;
//			float toMove = toMoveXY[channel];
//			if (toMove != 0)
//			{
//				if (toMove + currentPositions[channel] > limits[channel])
//					toMove = limits[channel] - currentPositions[channel];
//				if (toMove + currentPositions[channel] < 0)
//					toMove = -currentPositions[channel];
//
//				if (toMove > 0) // move forward
//				{
//					for (float s = 0; s <= toMove; s += mmPerStep[channel])
//					{
//						if (step(channel, 1, true))
//						{
//							wasMoved = true;
//						}
//						mmPerStep[channel];
//					}
//				}
//				else	 // reverse
//				{
//					for (float s = toMove; s < 0; s += mmPerStep[channel])
//					{
//						if (step(channel, -1, true))
//						{
//							wasMoved = true;
//						}
//						mmPerStep[channel];
//					}
//				}
//			}
//			if (wasMoved)
//				delay(1);
//		}
//
//	}
//}
int8_t coatStatus = 0;
float coatX = 0, coatY = 0, coatWidth = 0, coatHeight = 0, pumpMax = 0, Q = 0, coatSpeed = 0, coatStepY = 0;
float lengthTravelled = 0;
float totalLength = 0;
int coatMove = 0;
int coatYStepsTaken = 0;
long millisToCoatTo = 0;
long timeSinceLastXYStep = 0;	  
long timeSinceLastZStep = 0;
int coatsCompleted = 0, timesToCoat = 0;
bool stopCoatFlag = 0;
long lastStatusSend = 0;
void loop() 
{
	if (stopCoatFlag)
	{
		if (coatStatus != 0)
		{
			coatStatus = 1;
			if (coatMove < 4)
				coatMove = 4;
		}
	}

	if (coatStatus == 1)
	{
		if (coatMove == 4 || coatMove == 5)	  // go home
		{
			if (coatMove == 4) // go back to x = 0
			{
				if (currentPositions[0] <= coatX)
					coatMove = 5;
				else
				{
					step(0, -1);
					delay(1);
					if (millis() - lastStatusSend > 30)
					{
						String s1 = F("Moving");
						String s2 = F("Paused");
						SendStatus(s1, s2, 0);
						lastStatusSend = millis();
					}
				}
			}
			else if (coatMove == 5) // go back to y = 0
			{
				if (currentPositions[1] <= coatY)
				{
					coatYStepsTaken = 0;
					coatMove = 0;
					if (stopCoatFlag)
					{
						stopCoatFlag = 0;
						coatStatus = 0;
						SendStatus(F("Moving"), F("Idle"), lengthTravelled / totalLength * 100.0F);
						SendStatus("Idle", "Idle", 100);
						Serial.println("coat end");
					}
					else
						SendStatus(F("Moving"), F("Paused"), lengthTravelled / totalLength * 100.0F);
				}
				else
				{
					step(1, -1);
					delay(1);
					if (millis() - lastStatusSend > 30)
					{
						SendStatus(F("Moving"), F("Paused"), lengthTravelled / totalLength * 100.0F);
						lastStatusSend = millis();
					}
				}
			}
			if (stopCoatFlag)
				return;
		}
		else
		{
			int channel = 0, dir = 0;
			if (coatMove == 0)
			{
				channel = 0;
				dir = 1;
			}
			else if (coatMove == 1)
			{
				channel = 1;
				dir = 1;
			}
			else if (coatMove == 2)
			{
				channel = 0;
				dir = -1;
			}
			else if (coatMove == 3)
			{
				channel = 1;
				dir = 1;
			}
			bool hadAStep = false;
			if (micros() - timeSinceLastXYStep > (mmPerStep[channel] / coatSpeed) * 1000000.0F)
			{
				timeSinceLastXYStep = micros();
				step(channel, dir, true);
				lengthTravelled += mmPerStep[channel];
				hadAStep = true;
			}
			if (millis() - timeSinceLastZStep > mmPerStep[2] / Q)
			{
				timeSinceLastZStep = millis();
				step(2, -1, true);
				hadAStep = true;
			}
			if (hadAStep)
			{
				if (millis() - lastStatusSend > 50)
				{
					SendStatus(F("Coating"), F("Pumping"), lengthTravelled / totalLength * 100.0F);
					lastStatusSend = millis();
				}
				delay(1);
			}
			// check state exit conditions
			if (coatMove == 0) // going X+
			{
				if (currentPositions[0] > coatX + coatWidth) // outside bounds, lets go up
				{
					if (currentPositions[1] + coatStepY > coatY + coatHeight + 0.1) // can't go up. go to y home.
					{
						coatMove = 4;
						coatsCompleted++;
					}
					else
					{
						coatYStepsTaken++;
						coatMove = 1; // go up as usual
					}
				}
			}	
			else if (coatMove == 1) // going Y+	(1)
			{
				if (currentPositions[1] > coatY + coatStepY * coatYStepsTaken) // outside bounds, lets go back to x = 0
					coatMove = 2; // go left
			}  
			else if (coatMove == 2) // going x-
			{
				if (currentPositions[0] < coatX) // outside bounds, lets go up
				{
					if (currentPositions[1] + coatStepY > coatY + coatHeight + 0.1) // can't go up. go to y home.
					{
						coatMove = 5;
						coatsCompleted++;
					}
					else
					{
						coatYStepsTaken++;
						coatMove = 3; // go up as usual
					}
				}
			}
			else if (coatMove == 3) // going Y+ (2)
			{
				if (currentPositions[1] > coatY + coatStepY * coatYStepsTaken) // outside bounds, lets go back to x = max
					coatMove = 0; // go right
			}
		}
		if (coatsCompleted >= timesToCoat && !stopCoatFlag)
		{
			stopCoatFlag = 1;
		}
		if (millis() > millisToCoatTo && !stopCoatFlag)
			stopCoatFlag = 1;

		if (currentPositions[2] < pumpMax && !stopCoatFlag)
			stopCoatFlag = 1;
	}

	if (Serial.available())
	{					     
		HashTable Args;
		String command = Serial.readStringUntil('\n'); 	
		getArgs(command, Args);
		command = getCommand(command);
		if (command == F("abort"))
		{
			stopCoatFlag = true;
			Serial.println(F("abort resp: answer = stopped"));
		}
		else if (command == F("pause coat"))
		{					 
			coatStatus = -1;
		}
		else if (command == F("resume coat"))
		{
			timeSinceLastXYStep = micros();
			timeSinceLastZStep = millis();
			coatStatus = 1;
		}
		else if (command == F("set coat"))
		{
			// this command must be used safely							 
			//int mode = Args.Get(F("mode")).toInt();
			float startX = currentPositions[0];
			float startY = currentPositions[1];
			float x = startX, y = startY;
			coatStatus = 0;
			float lenX = Args.Get(F("lenX")).toFloat();	  // mm
			float lenY = Args.Get(F("lenY")).toFloat();	  // mm
			coatStepY = Args.Get(F("stepY")).toFloat(); // ~ 
			timesToCoat = Args.Get(F("coats")).toInt();	  // ~
			coatSpeed = Args.Get(F("speed")).toFloat(); // mm/sec
			Q = Args.Get(F("Q")).toFloat() / 1000;	      // mm/ms
			if (currentPositions[0] + lenX > limits[0])
			{
				Serial.println(F("coat resp: answer = no, message = The coat area width is outside the limits of the linear stage."));
				return;
			} 
			if (currentPositions[1] + lenY > limits[1])
			{
				Serial.println(F("coat resp: answer = no, message = The coat area height is outside the limits of the linear stage."));
				return;
			}
			if (coatSpeed <= 0 || coatSpeed > 5)
			{
				Serial.println(F("coat resp: answer = no, message = The travel speed you entered is not within the possible hardware range, (0, 5] mm/s"));
				return;
			}

			float maxDist = Args.Get(F("mxd")).toFloat();
			if (maxDist > 0)
			{
				if (currentPositions[2] - maxDist < 0)
				{
					Serial.println(F("coat resp: answer = no, message = The pump won't be able to push this amount of volume."));
					return;
				}
			}
			if (maxDist < 0) // not needed
				pumpMax = 0;
			else
				pumpMax = currentPositions[2] - maxDist;
				
			float timeToPump = Args.Get(F("mxt")).toFloat(); // seconds
			if (timeToPump < 0)
				timeToPump = limits[2] / Q;

			millisToCoatTo = millis() + timeToPump * 1000;

			totalLength = 0;
			for (float y = 0; y <= lenY + 0.1; y += coatStepY)
			{
				totalLength += lenX; 
				if (y + coatStepY + 0.1 <= lenY)
					totalLength += coatStepY;
			}
			totalLength *= timesToCoat;



			coatX = currentPositions[0];
			coatY = currentPositions[1];
			coatWidth = lenX;
			coatHeight = lenY;
			coatsCompleted = 0;
			timeSinceLastXYStep = micros();
			timeSinceLastZStep = millis();
			stopCoatFlag = 0;
			lengthTravelled = 0;
			coatYStepsTaken = 0;
									  
			coatStatus = 1;

			Serial.println(String(F("coat resp: answer = yes, total length = ")) + String(totalLength));
		}
		else if (command == F("x+") || command == F("x++"))
		{
			if (coatStatus != 0)
				return;
			Serial.println("x+");
			for (int steps = 0; steps < (command == F("x+") ? 1 : 10) / mmPerStep[0]; steps++)
			{
				if (currentPositions[0] + mmPerStep[0] > limits[0])
					break;
				step(0, 1);
				if (steps % 20 == 0)
					SendStatus(F("Moving"), F("Idle"), -1);
			}
			SendStatus(F("Idle"), F("Idle"), -1);
		}
		else if (command == F("x-") || command == F("x--"))
		{
			if (coatStatus != 0)
				return;
			Serial.println("x-");
			for (int steps = 0; steps < (command == F("x-") ? 1 : 10) / mmPerStep[0]; steps++)
			{
				if (currentPositions[0] + mmPerStep[0] < 0)
					break;
				step(0, -1);
				if (steps % 20 == 0)
					SendStatus(F("Moving"), F("Idle"), -1);
			}
			SendStatus(F("Idle"), F("Idle"), -1);
		}
		else if (command == F("y+") || command == F("y++"))
		{
			if (coatStatus != 0)
				return;
			Serial.println("y-");
			for (int steps = 0; steps < (command == F("y-") ? 1 : 10) / mmPerStep[0]; steps++)
			{
				if (currentPositions[1] + mmPerStep[1] > limits[1])
					break;
				step(1, 1);
				if (steps % 20 == 0)
					SendStatus(F("Moving"), F("Idle"), -1);
			}
			SendStatus(F("Idle"), F("Idle"), -1);
		}
		else if (command == F("y-") || command == F("y--"))
		{
			if (coatStatus != 0)
				return;
			Serial.println("y-");
			for (int steps = 0; steps < (command == F("y-") ? 1 : 10) / mmPerStep[0]; steps++)
			{
				if (currentPositions[1] + mmPerStep[1] < 0)
					break;
				step(1, -1);
				if (steps % 20 == 0)
					SendStatus(F("Moving"), F("Idle"), -1);
			}
			SendStatus(F("Idle"), F("Idle"), -1);
		}
		else if (command == F("home all"))
		{
			SendStatus(F("Homing"), F("Idle"), -1);
			MoveToHome(0);
			MoveToHome(1);				 
			SendStatus(F("Idle"), F("Homing"), -1);
			MoveToHome(2);


			currentPositions[0] = 0;
			currentPositions[1] = 0;
			currentPositions[2] = limits[2];
			stepper.switchOff();
			SendStatus(F("Idle"), F("Idle"), -1);
		}
		else if (command == "ztop")
		{
			if (coatStatus != 0)
				return;
			MoveToHome(2);
			currentPositions[2] = limits[2];
			stepper.switchOff();
		}
		else if (command == F("z+") || command == F("z++"))
		{
			if (coatStatus != 0)
				return;
			for (long i = 0; i < (command == F("z+") ? 1 : 10) * 1.0F / mmPerStep[2]; i++)
			{
				if (currentPositions[2] >= limits[2])
					return;
				stepper.step(1);
				currentPositions[2] += mmPerStep[2];
				delay(2);
			}
			while (Serial.available()) Serial.read();
		}
		else if (command == F("z-") || command == F("z--"))
		{
			if (coatStatus != 0)
				return;
			for (long i = 0; i < (command == F("z-") ? 1 : 10) * 1.0F / mmPerStep[2]; i++)
			{
				if (currentPositions[2] <= 0)
					return;
				stepper.step(-1);
				currentPositions[2] -= mmPerStep[2];
				delay(2);
			}
			while (Serial.available()) Serial.read();

		}
	}
}
