/*
 Name:		SPRFirmWareNano.ino
 Created:	9/23/2019 2:17:30 PM
 Author:	techBOY
 Co-author:	Hassan Ahmed
*/

// Use our custom stepper library
//#include "Stepper2.h"
#include "HashTable.h"
#include "PID.h"
#define DriverIsTMC2208 1
#define MotorIsSmallFaulHarber 1

// Set to 1 to remove the coating commands to make space for M119, M301, G0 and G1
#define BuildForPIDCalibraiton 1
// hardware pin map
#define SyringePumpEnablePin	13
#define xLimitPin			A7
#define yLimitPin			A6
#define pStepPin			A3
#define pDirectionPin		A4
#define pLimitPin			A5
#define pNegatve			1
#define pPositive			(!pNegatve)
#define PIDErrorInMM		0.3

uint8_t readPosx();
uint8_t readPosy();

uint32_t lastVelocityControlMillis = 0;

// vMax must not exceed 7812 - 10% . Its the ISR frequency
//float aMax = 12000, vMax = 6000;
float aMax = 6000, vMax = 4000;
int32_t currentCountx = 0, lastControllerMicrosx = 0;
int32_t requiredCountx = 0;
float time0x = 0, s0x = 0;
float  time1x = 0, s1x = 0;
float  time2x = 0, s2x = 0;
float  time3x = 0, s3x = 0;
float vTravelx = 0;
bool reversex = false;
int8_t lastPosx = 0;
bool _lastDirx = 0;
#if MotorIsSmallFaulHarber
PIDClass PositionControllerx = PIDClass(2, 0.000005, 0);
#else 
PIDClass PositionControllerx = PIDClass(1, 0.00002, 0);
#endif

int32_t currentCounty = 0, lastControllerMicrosy = 0;
int32_t requiredCounty = 0;
float time0y = 0, s0y = 0;
float  time1y = 0, s1y = 0;
float  time2y = 0, s2y = 0;
float  time3y = 0, s3y = 0;
float vTravely = 0;
bool reversey = false;
int8_t lastPosy = 0;
bool _lastDiry = 0;
#if MotorIsSmallFaulHarber
PIDClass PositionControllery = PIDClass(1, 0.00001, 0);
#else 
PIDClass PositionControllery = PIDClass(1, 0.00002, 0);
#endif



int8_t pumpDirection = 0;
//float mmPerStep[3] = { 0.0075,0.0075, 0.01 };
float mmPerStep[3] = {
#if MotorIsSmallFaulHarber
	0.0080211472021715, 0.0080211472021715,
#else
	0.0021037695107656, 0.0021037695107656,
#endif
#if DriverIsTMC2208
	0.000198F
#else
	0.00317564
#endif
}; // recalculate!!!!!
float limits[3] = { 130,130,98 }; // in mm
float currentPositions[3] = { 0, 0, 0 };
float targetXPosition = 0;
float targetYPosition = 0;
//void gotoXY(float x, float y, float speed_mmps);
bool pumpStep(int8_t direction, bool dontDelay = false);

String currentXyStatus, currentPumpStatus;
float currentProgress = 0;
void SendStatus()
//void SendStatus(String xyStatus, String pumpStatus, float progress)
{
	//if (progress == -1000)
		/*progress = currentProgress;
	currentProgress = progress;
	if (xyStatus == F("{last}"))
		xyStatus = currentXyStatus;
	if (pumpStatus == F("{last}"))
		pumpStatus = currentPumpStatus;*/
		/*if (currentXyStatus != xyStatus)
			currentXyStatus = xyStatus;
		if (currentPumpStatus != pumpStatus)
			currentPumpStatus = pumpStatus;*/
	Serial.print(F("status:"));
	Serial.print(F("x="));
	Serial.print(currentPositions[0], 1);
	Serial.print(F(",y="));
	Serial.print(currentPositions[1], 1);
	Serial.print(F(",z="));
	Serial.print(currentPositions[2], 1);
	Serial.print(F(",progress="));
	Serial.print(min(100, currentProgress), 2);
	Serial.print(F(",xy stage="));
	Serial.print(currentXyStatus);
	Serial.print(F(",pump="));
	Serial.print(currentPumpStatus);

	Serial.println();
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
uint32_t total = 0;
ISR(TIMER2_OVF_vect) {
	total++;
	uint8_t pos = readPosx();
	if (lastPosx != pos)
	{
		if (pos != lastPosx)
		{
			if (pos == (lastPosx + 1) % 4) // fwd
			{
				currentCountx++;
				currentPositions[0] += mmPerStep[0];
				_lastDirx = 1;
			}
			else if (pos == (lastPosx + 3) % 4) // rev
			{
				currentCountx--;
				currentPositions[0] -= mmPerStep[0];
				_lastDirx = 0;
			}
			else
			{
				if (_lastDirx == 1)
				{
					currentCountx++;
					currentPositions[0] += mmPerStep[0];
				}
				else
				{
					currentCountx--;
					currentPositions[0] -= +mmPerStep[0];
				}
				// we skipped something
			}
			lastPosx = pos;
		}
	}
	pos = readPosy();
	if (lastPosy != pos)
	{
		if (pos != lastPosy)
		{
			if (pos == (lastPosy + 1) % 4) // fwd
			{
				currentCounty++;
				currentPositions[1] += mmPerStep[1];
				_lastDiry = 1;
			}
			else if (pos == (lastPosy + 3) % 4) // rev
			{
				currentCounty--;
				currentPositions[1] -= mmPerStep[1];
				_lastDiry = 0;
			}
			else
			{
				if (_lastDiry == 1)
				{
					currentCounty++;
					currentPositions[1] += mmPerStep[1];
				}
				else
				{
					currentCounty--;
					currentPositions[1] -= mmPerStep[1];
				}
				// we skipped something
			}
			lastPosy = pos;
		}
	}
}
void setup() {
	Serial.begin(115200);
	Serial.setTimeout(100);
	currentXyStatus = F("Idle");
	currentPumpStatus = F("Idle");
	/////////////////////////////////
	//         PIN  MAP            //
	/////////////////////////////////

	pinMode(SyringePumpEnablePin, 1);
	digitalWrite(SyringePumpEnablePin, 1);
	//	_____X_Motor_________ 
	//	|_PIN___|__Address__|
	//	|  EN	| PB2, OC1B |
	//	|  In1	|    PD2    |
	//	|  In2	|    PC2    |
	//	|  PGA	|    PB3    |
	//	|  PGB	|    PB4    |
	//	| Limit	| xLimitPin |

	//	_____Y_Motor_________ 
	//	|_PIN___|__Address__|
	//	|  EN	| PB1, OC1A |
	//	|  In1	|    PD3    |
	//	|  In2	|    PD4    |
	//	|  PGA	|    PD7    |
	//	|  PGB	|    PB0    |
	//	| Limit	| yLimitPin |

	//	_______P_Motor___________
	//	|_PIN___|____Address____|
	//	| Step	|   pStepPin	|
	//	| Dir	| pDirectionPin |
	//	| Limit	|   pLimitPin	|

	// Setup pins used by motor X
	DDRB |= (1 << PB2);	// XEN
	DDRD |= (1 << PD2); // XIn1
	DDRC |= (1 << PC2); // XIn2
	digitalWrite(xLimitPin, 1);

	// Setup pins used by motor Y
	DDRB |= (1 << PB1);	// YEN
	DDRD |= (1 << PD3); // YIn1
	DDRD |= (1 << PD4); // YIn2
	digitalWrite(yLimitPin, 1);

	// Setup PWM
	TCCR1A = (1 << COM1A1) | (0 << COM1A0) | (1 << COM1B1) | (0 << COM1B0) | (1 << WGM11) | (0 << WGM10);
	TCCR1B = (0 << ICNC1) | (0 << ICES1) | (1 << WGM13) | (1 << WGM12) | (0 << CS12) | (0 << CS11) | (1 << CS10);
	//TCCR3C = 0b00001001;
	ICR1 = 450;

	// photogate timer interrupt setup
	TCCR2A = (0 << COM2A1) | (0 << COM2A0) | (0 << COM2B1) | (0 << COM2B0) | (0 << WGM21) | (0 << WGM20);
	TCCR2B = (0 << WGM22) | (0 << CS22) | (0 << CS21) | (1 << CS20);
	TIMSK2 |= (1 << TOIE2);
	PositionControllerx._max = 100;
	PositionControllerx._min = -100;
	PositionControllery._max = 100;
	PositionControllery._min = -100;

#if MotorIsSmallFaulHarber
	//PositionControllerx._Bp = 5;
	//PositionControllery._Bp = 5;
#endif
	// Setup pins used by motor P
	pinMode(pStepPin, 1);
	pinMode(pDirectionPin, 1);
	digitalWrite(pLimitPin, 1);


	currentPositions[0] = 0;
	currentPositions[1] = 0;
	currentPositions[2] = limits[2];

	//Serial.println(F("begin"));
}
void setPumpDirection(int8_t direction)
{
	if (pumpDirection == direction)
		return;
	pumpDirection = direction;

	// we give stepper the direction at the time of the pumpStep
	if (direction == 1)
		digitalWrite(pDirectionPin, pPositive);
	else
		digitalWrite(pDirectionPin, pNegatve);
}
bool checkLimit(uint8_t channel)
{
	if (channel == 0)
		return analogRead(xLimitPin) < 500;
	else if (channel == 1)
		return analogRead(yLimitPin) < 500;
	else
		return analogRead(pLimitPin) < 500;
}

bool pumpStep(int8_t direction, bool dontDelay)
{
	setPumpDirection(direction);
	bool wasMoved = false;
	PORTC |= 0b1000;
	PORTC &= 0b11110111;
	/*digitalWrite(pStepPin, 1);
	digitalWrite(pStepPin, 0);*/
	if (!dontDelay)
		delayMicroseconds(400);
	wasMoved = true;
	currentPositions[2] += pumpDirection * mmPerStep[2];
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

	if (channel == 0 || channel == 1) // x/y motor
	{
		float speed_mmps = 8;
		float tAllowed = limits[channel] / speed_mmps;
		int32_t msAllowed = tAllowed * 1000;
		//Serial.println(F("Go home started"));
		SetCourseForXY(channel, -limits[channel] / mmPerStep[channel], tAllowed, false);
		int32_t tStartedAt = millis();
		while (millis() - tStartedAt < msAllowed)
		{
			MotorLoopXY(1, 1);
			if (checkLimit(channel))
				foundCount++;
			else if (foundCount > 0)
				foundCount--;
			if (foundCount > 5)
				break;
		}
		// back it up a bit
		float backupMM = 1.5F;
		//Serial.println(F("Found limit. backing up"));
		SetCourseForXY(channel, backupMM / mmPerStep[channel], backupMM / 2, false);
		tStartedAt = millis();
		while (millis() - tStartedAt < (backupMM / 2) * 1000)
			MotorLoopXY(1, 1);

		//Serial.println(F("Fine finding"));
		// go back again.
		speed_mmps = 0.5;
		tAllowed = backupMM / speed_mmps;
		msAllowed = tAllowed * 1000 * 1.5;
		SetCourseForXY(channel, -backupMM / mmPerStep[channel], tAllowed, false);
		tStartedAt = millis();
		while (millis() - tStartedAt < msAllowed)
		{
			MotorLoopXY(1, 1);
			if (checkLimit(channel))
				foundCount++;
			else if (foundCount > 0)
				foundCount--;
			if (foundCount > 5)
				break;
		}
		// hard stop
		//Serial.println(F("Hard Stop"));
		SetCourseForXY(channel, 0, 0, false);
		tStartedAt = millis();
		while (millis() - tStartedAt < 500)
			MotorLoopXY(1, 1); // both motors need to be in PID

		//Serial.println(F("Leaving some room"));
		SetCourseForXY(channel, 20, 0.5, false);
		tStartedAt = millis();
		while (millis() - tStartedAt < 1500)
			MotorLoopXY(1, 1); // both motors need to be in PID

		//Serial.println(F("Resetting Vars"));
		if (channel == 0)
		{
			currentCountx = 0;
			requiredCountx = 0;
			s3x = 0;
		}
		else
		{
			currentCounty = 0;
			requiredCounty = 0;
			s3y = 0;
		}
		currentPositions[channel] = 0;
	}
	else if (channel == 2)
	{
		// Home Pump motor in the usual way
		for (long i = 0; i < limits[channel] / mmPerStep[channel]; i++)
		{
			if (checkLimit(channel))
				foundCount++;
			else if (foundCount > 0)
				foundCount--;
			if (foundCount > 5)
			{
				break;
			}
			pumpStep(homeDir, true);

			if (i % 200 == 0)
			{
				SendStatus();
			}
			MotorLoopXY(1, 1); // both motors need to be in PID
		}
		for (int i = 0; i < 20; i++)
		{
			pumpStep(-homeDir, true);
			delay(1);
			MotorLoopXY(1, 1); // both motors need to be in PID
		}
	}
	/*if (foundCount > 5)
	{
		Info(F("homing"), String(F("Found the limit")) + String(channel));
	}
	else
		Error(F("homing"), String(F("Home not found for the motor ")) + String(channel));*/

}
void motorWriteXY(int8_t channel, int power)
{
	uint16_t OCR3Ch = 0;
	if (power > 0)
		OCR3Ch = power * 4.5;
	else if (power < 0)
		OCR3Ch = (-power) * 4.5;
	else
		OCR3Ch = 0;
	if (channel == 0) // x
		OCR1B = OCR3Ch;
	else
		OCR1A = OCR3Ch;

	if (channel == 0)
	{
		//	|  In1	|    PD1    |
		//	|  In2	|    PC2    |

		uint8_t current1 = PORTD;
		uint8_t current2 = PORTC;
		current1 &= ~(0b1 << 2);
		current2 &= ~(0b1 << 2);

		if (power < 0) current1 |= 1 << 2;
		else if (power > 0) current2 |= 1 << 2;
		PORTD = current1;
		PORTC = current2;
	}
	else if (channel == 1)
	{

		//	|  In1	|    PD3    |
		//	|  In2	|    PD4    |
		uint8_t current = PORTD;
		current &= ~(0b11 << 3);

		if (power > 0) current |= 1 << 3;
		else if (power < 0) current |= 1 << 4;
		PORTD = current;
	}
}
uint8_t PG1_CWMap[] = { 0,0,1,1 };
uint8_t PG2_CWMap[] = { 0,1,1,0 };
uint8_t readPosx()
{
	//	|  PGA	|    PB3    |
	//	|  PGB	|    PB4    |
	uint8_t pg1 = (PINB >> 3) % 2;
	uint8_t pg2 = (PINB >> 4) % 2;
	uint8_t pos = 0;
	for (int i = 0; i < 4; i++)
	{
		if (PG1_CWMap[i] == pg1 && PG2_CWMap[i] == pg2)
		{
			pos = i;
			break;
		}
	}
	return pos;
}
uint8_t readPosy()
{
	//	|  PGA	|    PD7    |
	//	|  PGB	|    PB0    |
	uint8_t pg2 = (PIND >> 7) % 2;
	uint8_t pg1 = (PINB >> 0) % 2;
	uint8_t pos = 0;
	for (int i = 0; i < 4; i++)
	{
		if (PG1_CWMap[i] == pg1 && PG2_CWMap[i] == pg2)
		{
			pos = i;
			break;
		}
	}
	return pos;
}

void MotorLoopXY(bool doX, bool doY)
{
	if (doX)
	{
		int32_t dtI = micros() - lastControllerMicrosx;
		float dt = dtI / 1000000.0F;
		float S = PositionControllerx.Signal(requiredCountx, currentCountx, dt);
		// Velocity controller doens't help. Alsmost same response

		/*float reqV = ((float)requiredCountx - lastPidCountX);
		float curV = ((float)currentCountx - lastPidCountX);
		lastPidCountX = currentCountx;
		float S = PositionControllerx.Signal(reqV, curV, dt);*/

		motorWriteXY(0, S);

	}
	if (doY)
	{
		int32_t dtI = micros() - lastControllerMicrosy;
		float dt = dtI / 1000000.0F;
		float S = PositionControllery.Signal(requiredCounty, currentCounty, dt);
		motorWriteXY(1, S);
	}
	if (millis() > lastVelocityControlMillis)
	{
		lastVelocityControlMillis = millis();
		if (doX)
		{
			if (millis() < time1x * 1000) // rising
			{
				float t = (millis() / 1000.0F - time0x);
				if (!reversex)
					requiredCountx = s0x + 0 + 0.5F * aMax * t * t;
				else
					requiredCountx = s0x + 0 - 0.5F * aMax * t * t;
			}
			else if (millis() < time2x * 1000) // drag
			{
				if (!reversex)
					requiredCountx = s1x + vTravelx * (millis() / 1000.0F - time1x);
				else
					requiredCountx = s1x - vTravelx * (millis() / 1000.0F - time1x);
			}
			else if (millis() < time3x * 1000) // falling
			{
				float t = (millis() / 1000.0F - time2x);
				if (!reversex)
					requiredCountx = s2x + vTravelx * t - 0.5F * aMax * t * t;
				else
					requiredCountx = s2x - (vTravelx * t - 0.5F * aMax * t * t);
			}
			else // overdue
			{
				requiredCountx = s3x;
			}
		}
		if (doY)
		{
			if (millis() < time1y * 1000) // rising
			{
				float t = (millis() / 1000.0F - time0y);
				if (!reversey)
					requiredCounty = s0y + 0 + 0.5F * aMax * t * t;
				else
					requiredCounty = s0y + 0 - 0.5F * aMax * t * t;
			}
			else if (millis() < time2y * 1000) // drag
			{
				if (!reversey)
					requiredCounty = s1y + vTravely * (millis() / 1000.0F - time1y);
				else
					requiredCounty = s1y - vTravely * (millis() / 1000.0F - time1y);
			}
			else if (millis() < time3y * 1000) // falling
			{
				float t = (millis() / 1000.0F - time2y);
				if (!reversey)
					requiredCounty = s2y + vTravely * t - 0.5F * aMax * t * t;
				else
					requiredCounty = s2y - (vTravely * t - 0.5F * aMax * t * t);
			}
			else // overdue
			{
				requiredCounty = s3y;
			}
		}
	}
}
bool MoveXYMotorBlocking(int8_t channel, float sToTravel, float tAllowed)
{
	SetCourseForXY(channel, sToTravel, tAllowed, false);
	int32_t sMillis = millis();
	while (millis() - sMillis < tAllowed * 1000)
		MotorLoopXY(channel == 0, channel == 1);
}

bool SetCourseForXY(int8_t channel, float sToTravel, float tAllowed, bool dontApply)
{
	//float finalRequiredCount = Serial.readStringUntil(',').toInt();
	//float tAllowed = Serial.readStringUntil('\n').toFloat();
	//float sToTravel = finalRequiredCount - (channel ? currentCountx : currentCounty);
	bool isNeg = sToTravel < 0;
	if (isNeg)
		sToTravel = -sToTravel;
	// Fig 1
	float vStart = 0;
	float vEnd = 0;

	// syms('P2x', 'P2y', 'a_m', 'c1', 'c2', 'v_s', 'v_e', 't_m', 'v_m');
	// L1 = P2y == a_m * P2x + v_s;
	// L2 = P2y == -a_m * P2x + v_e + a_m * t_m;
	// P2 = solve([L1, L2], [P2x, P2y]);

	// x2 = P2.P2x;
	// y2 = P2.P2y;

	float x2 = (vEnd - vStart + aMax * tAllowed) / (2 * aMax);
	float y2 = vEnd / 2 + vStart / 2 + (aMax * tAllowed) / 2;

	float t_1b = 0, t_2b = 0, t_3b = 0;

	if (sToTravel == 0) // hardStop
	{
	}
	else if (y2 > vMax) // trapezium
	{
		//Serial.println(F("Its a trapezium"));
		// Fig 2
		float x3 = (vMax - vStart) / aMax;
		float y3 = vMax;
		float x4 = tAllowed - (vEnd - vMax) / (-aMax);
		float y4 = vMax;
		float t_1 = x3;
		float t_2 = x4 - x3;
		float t_3 = tAllowed - x4;
		float a1 = 0.5 * t_1 * (vMax - vStart);
		float a2 = t_1 * vStart;
		float a3 = t_2 * vMax;
		float a4 = 0.5 * t_3 * (vMax - vEnd);
		float a5 = t_3 * vEnd;
		float area = a1 + a2 + a3 + a4 + a5;
		if (area < sToTravel)
		{
			//Serial.println(F("Travel not possible 1"));
			if (!dontApply)
			{
				float additionalTimeNeeded = (sToTravel - area) / vMax;
				if (isNeg)
					sToTravel = -sToTravel;
				return SetCourseForXY(channel, sToTravel, tAllowed + additionalTimeNeeded, dontApply);
			}
			else
				return false;
		}
		// find out the maxV we are going to use
		// syms('a_1', 'a_2', 'a_3', 'a_4', 'a_5', '_vTravel', 'sTravel', 't_1', 't_2', 't_3')
		// ee6 = 0.5 * t_1 * (_vTravel - v_s) + t_1 * v_s + t_2 * _vTravel + 0.5 * t_3 * (_vTravel - v_e) + t_3 * v_e == sTravel;
	// solve(ee6, _vTravel)
		float _vTravel = vEnd / 2 + vStart / 2 + (aMax * tAllowed) / 2 - sqrt(aMax * aMax * tAllowed * tAllowed + 2 * aMax * tAllowed * vEnd + 2 * aMax * tAllowed * vStart - 4 * sToTravel * aMax - vEnd * vEnd + 2 * vEnd * vStart - vStart * vStart) / 2;

		// recalculate the trapezium
		float x3b = (_vTravel - vStart) / aMax;
		float y3b = _vTravel;
		float x4b = tAllowed - (vEnd - _vTravel) / (-aMax);
		float y4b = _vTravel;
		t_1b = x3b;
		t_2b = x4b - x3b;
		t_3b = tAllowed - x4b;
		if (!dontApply)
		{
			if (channel == 0)
				vTravelx = _vTravel;
			else if (channel == 1)
				vTravely = _vTravel;
		}
	}
	else// its a triangle
	{
		//Serial.println(F("Its a triangle"));
		// lets see if acc or dec is possible
		if (vEnd > vStart)
		{
			if ((vEnd - vStart) / tAllowed > aMax)
			{
				//Serial.println(F("Travel not possible 2"));
				float timeNeeded = (vEnd - vStart) / aMax;
				if (!dontApply)
				{
					if (isNeg)
						sToTravel = -sToTravel;
					return SetCourseForXY(channel, sToTravel, timeNeeded, dontApply);
				}
				return false;
			}
		}
		else
		{
			if ((vEnd - vStart) / tAllowed < -aMax)
			{
				//Serial.println(F("Travel not possible 3"));
				float timeNeeded = (vEnd - vStart) / -aMax;
				if (!dontApply)
				{
					if (isNeg)
						sToTravel = -sToTravel;
					return SetCourseForXY(channel, sToTravel, timeNeeded, dontApply);
				}
				return false;
				return false;
			}
		}
		// figure 3
		float a1 = 0.5 * x2 * (y2 - vStart);
		float a2 = x2 * vStart;
		float a3 = 0.5 * (tAllowed - x2) * (y2 - vEnd);
		float a4 = (tAllowed - x2) * vEnd;
		float area = a1 + a2 + a3 + a4;
		if (area < sToTravel)
		{
			//Serial.println(F("Travel not possible 4"));
			if (dontApply)
				return false;
			float areaMax = (vMax * vMax - vStart * vStart) / 2 / aMax + (vEnd * vEnd - vMax * vMax) / 2 / -aMax;
			if (areaMax >= sToTravel)
			{
				float travelLeft = sToTravel - area;
				float additionalTime1 = -(y2 - sqrt(y2 * y2 + 2 * aMax * travelLeft)) / aMax;
				if (isNeg)
					sToTravel = -sToTravel;
				return SetCourseForXY(channel, sToTravel, tAllowed + additionalTime1 * 1.05, dontApply);
			}
			else
			{
				float tInTriangle = (vMax - vStart) / aMax + (vEnd - vMax) / -aMax;
				float tAtvMax = (sToTravel - areaMax) / vMax;
				if (isNeg)
					sToTravel = -sToTravel;
				return SetCourseForXY(channel, sToTravel, tAtvMax + tInTriangle, dontApply);
			}
		}
		float _vTravel = vEnd / 2 + vStart / 2 + (aMax * tAllowed) / 2 - sqrt(aMax * aMax * tAllowed * tAllowed + 2 * aMax * tAllowed * vEnd + 2 * aMax * tAllowed * vStart - 4 * sToTravel * aMax - vEnd * vEnd + 2 * vEnd * vStart - vStart * vStart) / 2;
		// vTravel2 = vEnd / 2 + vStart / 2 + (aMax * tAllowed) / 2 + (aMax ^ 2 * tAllowed ^ 2 + 2 * aMax * tAllowed * vEnd + 2 * aMax * tAllowed * vStart - 4 * sToTravel * aMax - vEnd ^ 2 + 2 * vEnd * vStart - vStart ^ 2) ^ (1 / 2) / 2
		// calculate the trapezium
		float 	x3b = (_vTravel - vStart) / aMax;
		float y3b = _vTravel;
		float x4b = tAllowed - (vEnd - _vTravel) / (-aMax);
		float y4b = _vTravel;
		t_1b = x3b;
		t_2b = x4b - x3b;
		t_3b = tAllowed - x4b;
		if (!dontApply)
		{
			if (channel == 0)
				vTravelx = _vTravel;
			else if (channel == 1)
				vTravely = _vTravel;
		}
	}
	if (!dontApply)
	{
		if (channel == 0)
		{
			if (isNeg)
				targetXPosition = currentPositions[0] - sToTravel * mmPerStep[0];
			else
				targetXPosition = currentPositions[0] + sToTravel * mmPerStep[0];
		}
		else if (channel == 0)
		{
			if (isNeg)
				targetYPosition = currentPositions[1] - sToTravel * mmPerStep[1];
			else
				targetYPosition = currentPositions[1] + sToTravel * mmPerStep[1];
		}
		if (sToTravel == 0) // hardStop
		{
			if (channel == 0)
			{
				requiredCountx = currentCountx;
				// We can now force an overdue.
				time0x = (millis() - 1) / 1000.0F;
				time1x = time0x;
				time2x = time0x;
				time3x = time0x;
				s3x = currentCountx;
			}
			else
			{
				requiredCounty = currentCounty;
				// We can now force an overdue.
				time0y = (millis() - 1) / 1000.0F;
				time1y = time0y;
				time2y = time0y;
				time3y = time0y;
				s3y = currentCounty;
			}
			//Serial.println(F("Hard Stop"));
		}
		else
		{
			if (channel == 0)
			{
				time0x = millis() / 1000.0F;
				time1x = time0x + t_1b;
				time2x = time1x + t_2b;
				time3x = time2x + t_3b;
				if (!isNeg)
				{
					s0x = currentCountx;
					s1x = s0x + 0 + 0.5F * aMax * t_1b * t_1b;
					s2x = s1x + vTravelx * t_2b;
					s3x = s0x + sToTravel;
				}
				else
				{
					s0x = currentCountx;
					s1x = s0x - (0 + 0.5F * aMax * t_1b * t_1b);
					s2x = s1x - vTravelx * t_2b;
					s3x = s0x - sToTravel;
				}
				reversex = isNeg;
				lastControllerMicrosx = micros();
			}
			else if (channel == 1)
			{
				time0y = millis() / 1000.0F;
				time1y = time0y + t_1b;
				time2y = time1y + t_2b;
				time3y = time2y + t_3b;
				if (!isNeg)
				{
					s0y = currentCounty;
					s1y = s0y + 0 + 0.5F * aMax * t_1b * t_1b;
					s2y = s1y + vTravely * t_2b;
					s3y = s0y + sToTravel;
				}
				else
				{
					s0y = currentCounty;
					s1y = s0y - (0 + 0.5F * aMax * t_1b * t_1b);
					s2y = s1y - vTravely * t_2b;
					s3y = s0y - sToTravel;
				}
				reversey = isNeg;
				lastControllerMicrosy = micros();
			}
		}
	}
	/*if (channel == 0)
	{
		Serial.print(F("applicaitonChannel ")); Serial.println(channel);
		Serial.print(F("reverse ")); Serial.println(reversex);
		Serial.print(F("vTravel ")); Serial.println(vTravelx);
		Serial.print(F("time0 ")); Serial.println(time0x);
		Serial.print(F("time1 ")); Serial.println(time1x);
		Serial.print(F("time2 ")); Serial.println(time2x);
		Serial.print(F("time3 ")); Serial.println(time3x);
		Serial.print(F("s0 ")); Serial.println(s0x);
		Serial.print(F("s1 ")); Serial.println(s1x);
		Serial.print(F("s2 ")); Serial.println(s2x);
		Serial.print(F("s3 ")); Serial.println(s3x);
	}
	else if (channel == 1)
	{
		Serial.print(F("applicaitonChannel ")); Serial.println(channel);
		Serial.print(F("reverse ")); Serial.println(reversey);
		Serial.print(F("vTravel ")); Serial.println(vTravely);
		Serial.print(F("time0 ")); Serial.println(time0y);
		Serial.print(F("time1 ")); Serial.println(time1y);
		Serial.print(F("time2 ")); Serial.println(time2y);
		Serial.print(F("time3 ")); Serial.println(time3y);
		Serial.print(F("s0 ")); Serial.println(s0y);
		Serial.print(F("s1 ")); Serial.println(s1y);
		Serial.print(F("s2 ")); Serial.println(s2y);
		Serial.print(F("s3 ")); Serial.println(s3y);
	}*/
	lastVelocityControlMillis = millis() - 1;
	lastVelocityControlMillis = millis() - 1;
	return true;
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
int8_t coatStatus = 0;
float coatX = 0, coatY = 0, coatWidth = 0, coatHeight = 0, pumpMax = 0, Q = 0, coatSpeed = 0, coatStepY = 0;
float definitiveLengthTravelled = 0;
float lengthTravelled = 0;
float totalLength = 0;
// Need to design a new scheme
float pumpStart = 0;
int coatMove = 0;
float targetXAtPause = 0;
float targetYAtPause = 0;
int coatYStepsTaken = 0;
long millisToCoatTo = 0;
long timeSinceLastZStep = 0;
int coatsCompleted = 0, timesToCoat = 0;
bool stopCoatFlag = 0;
bool abortBegun = false;
long lastStatusSend = 0;
uint32_t lastSerialMillis = 0;
uint32_t ourCounter = 0;
uint32_t autoStatusSentAt = 0;

void loop()
{
	//ourCounter++;
	//if (millis() - lastSerialMillis > 1000)
	//{
	//	//Serial.print(total);
	//	//total = 0;
	//	//Serial.print(F(", "));
	//	//Serial.println(ourCounter);
	//	//ourCounter = 0;
	//	//return;
	//	lastSerialMillis = millis();
	//	Serial.print(F("ccx: "));
	//	Serial.print(currentCountx);
	//	Serial.print(F(", rcx: "));
	//	Serial.print(requiredCountx);
	//	Serial.print(F(", cpx: "));
	//	Serial.print(currentPositions[0]);
	//	/*Serial.print(F(", OCRB: "));
	//	Serial.print(OCR1B);
	//	Serial.print(F(", tx = "));
	//	Serial.print(millis() / 1000.0F - time0x);*/
	//	Serial.print(F("\t|\tccy: "));
	//	Serial.print(currentCounty);
	//	Serial.print(F(", rcy: "));
	//	Serial.print(requiredCounty);
	//	Serial.print(F(", cpy: "));
	//	Serial.print(currentPositions[1]);
	//	/*Serial.print(F(", OCRA: "));
	//	Serial.print(OCR1A);
	//	Serial.print(F(", ty = "));
	//	Serial.print(millis() / 1000.0F - time0y);*/

	//	Serial.println();
	//}
#if BuildForPIDCalibraiton
	if (millis() - autoStatusSentAt > 1000)
#else
	if (millis() - autoStatusSentAt > 50)
#endif
	{
		SendStatus();
		autoStatusSentAt = millis();
	}
	MotorLoopXY(1, 1);
	if (stopCoatFlag)
	{
		if (!abortBegun)
		{
			abortBegun = true;
			if (coatMove == 6) // pump only
			{
				stopCoatFlag = false;
				abortBegun = false;
				coatStatus = 0;
				coatMove = 0;
				currentProgress = -1000;
				digitalWrite(SyringePumpEnablePin, 0);
				currentXyStatus = F("Idle");
				currentPumpStatus = F("Idle");
				digitalWrite(SyringePumpEnablePin, 1);
				Serial.println("coat end");
			}
			else
			{
				if (coatStatus != 0) // running or paused
				{
					coatStatus = 1;
					currentXyStatus = F("Moving");
					currentPumpStatus = F("Idle");
					digitalWrite(SyringePumpEnablePin, 1);
					SendStatus();
					if (coatMove < 4)
					{
						//Serial.println(F("coat move 4"));
						coatMove = 4; // go to x zero
						SetCourseForXY(0, -(currentPositions[0] - coatX) / mmPerStep[0], abs((currentPositions[0] - coatX)) / 3, false);
					}
				}
				else
				{
					stopCoatFlag = false;
					abortBegun = false;
					coatStatus = 0;
					coatMove = 0;
					currentProgress = -1000;
					digitalWrite(SyringePumpEnablePin, 1);
					currentXyStatus = F("Idle");
					currentPumpStatus = F("Idle");
					digitalWrite(SyringePumpEnablePin, 1);
					Serial.println("coat end");
				}
			}
		}
	}

	//	_____X_Motor_________ 
	//	|__MOVE_|_Movement__|
	//	|  0	|  in x  >  |
	//	|  1	|  in y  ^  |
	//	|  2	|  in x  <  |
	//	|  3	|  in y  v  |
	//	|  4	| to x home |
	//	|  5	| to y home |
	if (coatStatus == 1)
	{
		// Done
		if (coatMove == 4 || coatMove == 5)	  // go home
		{
			if (coatMove == 4) // go back to x = 0
			{
				if (currentPositions[0] - PIDErrorInMM <= coatX)
				{
					//Serial.println(F("coat move 5"));
					coatMove = 5;
					SetCourseForXY(1, -(currentPositions[1] - coatY) / mmPerStep[1], abs(currentPositions[1] - coatY) / 3, false);
				}
				else
				{
#if BuildForPIDCalibraiton
					if (millis() - lastStatusSend > 400)
#else
					if (millis() - lastStatusSend > 30)
#endif
					{
						currentXyStatus = F("Moving");
						currentPumpStatus = F("Paused");
						SendStatus();
						lastStatusSend = millis();
					}
				}
			}
			else if (coatMove == 5) // go back to y = 0
			{
				if (currentPositions[1] - PIDErrorInMM <= coatY)
				{
					coatYStepsTaken = 0;
					coatMove = 0;
					//Serial.println(F("coat move 0"));
					if (stopCoatFlag)
					{
						//SendStatus(F("Moving"), F("Idle"), lengthTravelled / totalLength * 100.0F);
						currentXyStatus = F("Idle");
						currentPumpStatus = F("Idle");
						digitalWrite(SyringePumpEnablePin, 1);
						currentProgress = -1000;
						abortBegun = false;
						stopCoatFlag = 0;
						coatStatus = 0;
						SendStatus();
						Serial.println("coat end");
					}
					else
					{
						//SetCourseForXY(0, coatWidth / mmPerStep[0], coatWidth / coatSpeed, false);
						SetCourseForXY(0, (coatX + coatWidth - currentPositions[0]) / mmPerStep[0], coatWidth / coatSpeed, false);
						currentXyStatus = F("Moving");
						currentPumpStatus = F("Paused");
						SendStatus();
					}
				}
				else
				{
#if BuildForPIDCalibraiton
					if (millis() - lastStatusSend > 400)
#else
					if (millis() - lastStatusSend > 30)
#endif
					{
						currentXyStatus = F("Moving");
						currentPumpStatus = F("Paused");
						SendStatus();
						lastStatusSend = millis();
					}
				}
			}
			if (stopCoatFlag)
				return;
		}
		else // coating or pumping
		{
			// we only control motor final positions.
			// xy loop
			// already running
			// same as before
			// pump loop
			bool hadAStep = false;
			if (millis() - timeSinceLastZStep > mmPerStep[2] / Q)
			{
				timeSinceLastZStep = millis();
				pumpStep(-1, true);
				hadAStep = true;
			}
			if (hadAStep)
			{
#if BuildForPIDCalibraiton
				if (millis() - lastStatusSend > 400)
#else
				if (millis() - lastStatusSend > 50)
#endif
				{
					if (coatMove < 6)
						currentXyStatus = F("Coating");
					else
						currentXyStatus = F("Idle");
					currentPumpStatus = F("Pumping");
					digitalWrite(SyringePumpEnablePin, 0);
					if (coatMove == 6)
						/*currentProgress = abs(currentPositions[2] - pumpStart) / abs(pumpMax - pumpStart) * 100;
					else*/
					{
						currentProgress = abs(currentPositions[2] - pumpStart) / abs(pumpMax - pumpStart) * 100;
						SendStatus();
					}
					lastStatusSend = millis();
				}
				//delay(1);
			}

			// check state exit conditions
			// Done, going X+
			if (coatMove == 0)
			{
				if (currentPositions[0] + PIDErrorInMM > coatX + coatWidth) // outside x bounds, lets go up
				{
					if (currentPositions[1] + PIDErrorInMM + coatStepY > coatY + coatHeight + 0.1) 
						// can't go up. go to x home.
					{
						//Serial.println(F("coat move 4"));
						coatMove = 4; // go to x zero
						coatsCompleted++;
						//SetCourseForXY(0, -coatWidth / mmPerStep[0], abs(currentPositions[0] - coatX) / 3, false);
						SetCourseForXY(0, (coatX - currentPositions[0]) / mmPerStep[0], abs(coatX - currentPositions[0]) / 3, false);
						//Serial.println(F("BP1"));
					}
					else
					{
						definitiveLengthTravelled += coatWidth;
						coatYStepsTaken++;
						//Serial.println(F("coat move 1"));
						coatMove = 1; // go up as usual in steps
						SetCourseForXY(1, coatStepY / mmPerStep[1], coatStepY / coatSpeed, false);
						//Serial.println(F("BP2"));
					}
				}
				else // moving normally horizontally towards X++
					lengthTravelled = definitiveLengthTravelled + (currentPositions[0] - coatX);

			}
			// Done, going Y+	(1)
			else if (coatMove == 1)
			{
				if (currentPositions[1] + PIDErrorInMM > coatY + coatStepY * coatYStepsTaken) // moved a step, lets go back to x = 0
				{
					definitiveLengthTravelled += coatStepY;
					//Serial.println(F("coat move 2"));
					coatMove = 2; // go left
					//SetCourseForXY(0, -coatWidth / mmPerStep[0], coatWidth / coatSpeed, false);
					SetCourseForXY(0, (coatX - currentPositions[0]) / mmPerStep[0], coatWidth / coatSpeed, false);
				}
				else // moving normally vertically towards Y++
					lengthTravelled = definitiveLengthTravelled + (currentPositions[1] - coatY - coatStepY * (coatYStepsTaken - 1));

			}
			// Done, going x-
			else if (coatMove == 2)
			{
				if (currentPositions[0] - PIDErrorInMM < coatX) // too left, lets go up
				{
					if (currentPositions[1] + coatStepY + PIDErrorInMM > coatY + coatHeight + 0.1) 
						// can't go up. already at x home, go to y home.
					{
						//Serial.println(F("coat move 5"));
						coatMove = 5;
						coatsCompleted++;
						SetCourseForXY(1, (coatY - currentPositions[1]) / mmPerStep[1], abs(coatY - currentPositions[1]) / 3, false);
						//Serial.println(F("BP3"));
					}
					else
					{
						definitiveLengthTravelled += coatWidth;
						coatYStepsTaken++;
						//Serial.println(F("coat move 3"));
						coatMove = 3; // go up as usual in steps
						SetCourseForXY(1, coatStepY / mmPerStep[1], coatStepY / coatSpeed, false);

						//Serial.println(F("BP4"));
					}
				}
				else // moving normally horizontally towards X--
					lengthTravelled = definitiveLengthTravelled + (coatWidth - (currentPositions[0] - coatX));
			}
			// Done, going Y+ (2)
			else if (coatMove == 3)
			{
				if (currentPositions[1] + PIDErrorInMM > coatY + coatStepY * coatYStepsTaken) 
					// step complete, lets go right to x max
				{
					definitiveLengthTravelled += coatStepY;
					//Serial.println(F("coat move 0"));
					coatMove = 0; // go right
					//SetCourseForXY(0, coatWidth / mmPerStep[0], coatWidth / coatSpeed, false);
					SetCourseForXY(0, (coatX + coatWidth - currentPositions[0]) / mmPerStep[0], coatWidth / coatSpeed, false);
				}
				else // moving normally vertically towards Y++
					lengthTravelled = definitiveLengthTravelled + (currentPositions[1] - coatY - coatStepY * (coatYStepsTaken - 1));
			}
			if (coatMove <= 3)
				currentProgress = lengthTravelled / totalLength * 100.0F;
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
		int comStart = Serial.read();
		if (comStart == 253) // for instrument Name
		{
			delay(1);
			if (Serial.read() == ':')
			{
				Serial.write(0xAA);
				Serial.write(0xFF);
				Serial.write(253);
				Serial.write(':');
				String name = F("Qosain ESS");
				Serial.write(name.length());
				Serial.print(name);
			}
			return;
		}
		String command = String((char)comStart) + Serial.readStringUntil('\n');
		getArgs(command, Args);
		command = getCommand(command);

		if (command == F("abort"))
		{
			stopCoatFlag = true;
			Serial.println(F("abort resp: answer = stopped"));
		}
#if !BuildForPIDCalibraiton
		else if (command == F("pause coat"))
		{
			targetXAtPause = targetXPosition;
			targetYAtPause = targetYPosition;
			SetCourseForXY(0, 0, 0, 0);
			SetCourseForXY(1, 0, 0, 0);
			currentPumpStatus = F("paused");
			currentXyStatus = F("paused");
			coatStatus = -1;
		}
		else if (command == F("resume coat"))
		{
			timeSinceLastZStep = millis();
			coatStatus = 1;
			// coat move 6 will automatically be skipped
			if (coatMove == 0 || coatMove == 2 || coatMove == 4) // X
				SetCourseForXY(0, (targetXAtPause - currentPositions[0]) / mmPerStep[0], abs(targetXAtPause - currentPositions[0]) / coatSpeed, false);
			else if (coatMove == 1 || coatMove == 3 || coatMove == 5) // Y
				SetCourseForXY(1, (targetYAtPause - currentPositions[1]) / mmPerStep[1], abs(targetYAtPause - currentPositions[1]) / coatSpeed, false);
		}
		else if (command == F("set coat"))
		{
			abortBegun = false;
			Serial.println(F("Set Coat")); delay(1);
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

			if (Args.Get(F("rstr")).toInt() == 1)
			{
				if (SetCourseForXY(0, lenX / mmPerStep[0], lenX / coatSpeed, true) == false)
				{
					Serial.println(F("coat resp: answer = no, message = The set coat speed is too much for the linear stage."));
					return;
				}

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
				if (coatSpeed <= 0 || coatSpeed > 25)
				{
					Serial.println(F("coat resp: answer = no, message = The travel speed you entered is not within the possible hardware range, (0, 25] mm/s"));
					return;
				}
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
			pumpStart = currentPositions[2];

			float timeToPump = Args.Get(F("mxt")).toFloat(); // seconds
			if (timeToPump < 0)
			{
				timeToPump = limits[2] / Q;
				millisToCoatTo = -1;
			}
			else
			{
				// to implement a more accurate time limit, lets use millis instead of setting a maxDist
				millisToCoatTo = millis() + timeToPump * 1000;
			}
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
			stopCoatFlag = 0;
			definitiveLengthTravelled = 0;
			lengthTravelled = 0;
			coatYStepsTaken = 0;
			coatStatus = 1;
			coatMove = 0;

			if (Args.Get(F("rstr")).toInt() == 0)
			{
				coatWidth = 0;
				coatHeight = 0;
				coatMove = 6;
			}
			timeSinceLastZStep = millis();


			Serial.println(String(F("coat resp: answer = yes, total length = ")) + String(totalLength));
			if (Args.Get(F("rstr")).toInt() == 1)
			{
				SetCourseForXY(0, lenX / mmPerStep[0], lenX / coatSpeed, false);
				digitalWrite(SyringePumpEnablePin, 0);
			}
		}
#endif
#if BuildForPIDCalibraiton
		else if (command.startsWith(F("M301")))
		{
			if (coatStatus != 0)
				return;

			command.toLowerCase();
			float p = PositionControllerx._Kp, i = PositionControllerx._Ki, d = PositionControllerx._Kd;
			if (command.indexOf(F("p")) > 0)
			{
				String vStr = command.substring(command.indexOf(F("p")) + 1);
				if (vStr.indexOf(F(" ") > 0))
					vStr = vStr.substring(0, vStr.indexOf(F(" ")));
				p = vStr.toFloat();
			}
			if (command.indexOf(F("i")) > 0)
			{
				String vStr = command.substring(command.indexOf(F("i")) + 1);
				if (vStr.indexOf(F(" ") > 0))
					vStr = vStr.substring(0, vStr.indexOf(F(" ")));
				i = vStr.toFloat();
			}
			if (command.indexOf(F("d")) > 0)
			{
				String vStr = command.substring(command.indexOf(F("d")) + 1);
				if (vStr.indexOf(F(" ") > 0))
					vStr = vStr.substring(0, vStr.indexOf(F(" ")));
				d = vStr.toFloat();
			}
			PositionControllerx._Kp = p;
			PositionControllerx._Ki = i;
			PositionControllerx._Kd = d;
			PositionControllery._Kp = p;
			PositionControllery._Ki = i;
			PositionControllery._Kd = d;
			Serial.println(F("Set PID:"));
			Serial.print(F("P:"));
			Serial.println(p, 8);
			Serial.print(F("I:"));
			Serial.println(i, 8);
			Serial.print(F("D:"));
			Serial.print(d, 8);
			Serial.println();
		}
		//else if (command.startsWith(F("M302")))
		//{
		//	if (coatStatus != 0)
		//		return;

		//	command.toLowerCase();
		//	float p = PositionControllerx._Kp, i = PositionControllerx._Ki, d = PositionControllerx._Kd;
		//	if (command.indexOf(F("p")) > 0)
		//	{
		//		String vStr = command.substring(command.indexOf(F("p")) + 1);
		//		if (vStr.indexOf(F(" ") > 0))
		//			vStr = vStr.substring(0, vStr.indexOf(F(" ")));
		//		p = vStr.toFloat();
		//	}
		//	if (command.indexOf(F("i")) > 0)
		//	{
		//		String vStr = command.substring(command.indexOf(F("i")) + 1);
		//		if (vStr.indexOf(F(" ") > 0))
		//			vStr = vStr.substring(0, vStr.indexOf(F(" ")));
		//		i = vStr.toFloat();
		//	}
		//	if (command.indexOf(F("d")) > 0)
		//	{
		//		String vStr = command.substring(command.indexOf(F("d")) + 1);
		//		if (vStr.indexOf(F(" ") > 0))
		//			vStr = vStr.substring(0, vStr.indexOf(F(" ")));
		//		d = vStr.toFloat();
		//	}
		//	PositionControllerx._Bp = p;
		//	PositionControllerx._Bi = i;
		//	PositionControllerx._Bd = d;
		//	PositionControllery._Bp = p;
		//	PositionControllery._Bi = i;
		//	PositionControllery._Bd = d;
		//	Serial.println(F("Set PID Bias:"));
		//	Serial.print(F("P:"));
		//	Serial.println(p, 8);
		//	Serial.print(F("I:"));
		//	Serial.println(i, 8);
		//	Serial.print(F("D:"));
		//	Serial.print(d, 8);
		//	Serial.println();
		//}
		//else if (command.startsWith(F("M303")))
		//{
		//	if (coatStatus != 0)
		//		return;

		//	command.toLowerCase();
		//	float f = PositionControllerx.setPointAvgFactor;
		//	if (command.indexOf(F("f")) > 0)
		//	{
		//		String vStr = command.substring(command.indexOf(F("f")) + 1);
		//		if (vStr.indexOf(F(" ") > 0))
		//			vStr = vStr.substring(0, vStr.indexOf(F(" ")));
		//		f = vStr.toFloat();
		//	}
		//	PositionControllerx.setPointAvgFactor = f;
		//	PositionControllery.setPointAvgFactor = f;

		//	Serial.println(F("Set averaging factor:"));
		//	Serial.print(F("f: "));
		//	Serial.println(f, 8);
		//	Serial.println();
		//}
		else if (command == F("M119"))
		{
			Serial.print(F("X Limit: "));
			Serial.println(checkLimit(0) ? F("Touching") : F("Open"));
			Serial.print(F("Y Limit: "));
			Serial.println(checkLimit(1) ? F("Touching") : F("Open"));
			Serial.print(F("Syringe Limit: "));
			Serial.println(checkLimit(2) ? F("Touching") : F("Open"));
		}
		else if (command.startsWith(F("G0")) || command.startsWith(F("G1")))
		{
			/*if (coatStatus != 0)
				return;*/

			command.toLowerCase();
			float v = 1, x = targetXPosition, y = targetYPosition;

			Serial.print(F("x0 "));
			Serial.print(x);
			Serial.print(F(", y0 "));
			Serial.print(y);
			if (command.indexOf(F("f")) > 0)
			{
				String vStr = command.substring(command.indexOf(F("f")) + 1);
				if (vStr.indexOf(F(" ") > 0))
					vStr = vStr.substring(0, vStr.indexOf(F(" ")));
				v = vStr.toFloat();
			}
			if (command.indexOf(F("x")) > 0)
			{
				String vStr = command.substring(command.indexOf(F("x")) + 1);
				if (vStr.indexOf(F(" ") > 0))
					vStr = vStr.substring(0, vStr.indexOf(F(" ")));
				if (command.startsWith(F("G1")))
					x = vStr.toFloat();
				else
					x += vStr.toFloat();
			}
			if (command.indexOf(F("y")) > 0)
			{
				String vStr = command.substring(command.indexOf(F("y")) + 1);
				if (vStr.indexOf(F(" ") > 0))
					vStr = vStr.substring(0, vStr.indexOf(F(" ")));

				if (command.startsWith(F("G1")))
					y = vStr.toFloat();
				else
					y += vStr.toFloat();
			}
			Serial.print(F(" > x1 "));
			Serial.print(x);
			Serial.print(F(", y1 "));
			Serial.println(y);
			float th = atan2(y - currentPositions[1], x - currentPositions[0]);
			float vx = abs(v * cos(th));
			float vy = abs(v * sin(th));

			float vxyA[] = { vx,vy };
			float xyA[] = { x, y };
			for (int channel = 0; channel < 2; channel++)
			{
				Serial.print(F("Set Course: C"));
				Serial.print(channel);
				if (command.indexOf((char)('x' + channel)) < 0)
				{
					Serial.println(" skip 1");
					continue;
				}
				float xy = xyA[channel];
				float vxy = vxyA[channel];
				if (xy < 0)
					xy = 0;
				if (xy > limits[channel])
					xy = limits[channel];
				float dxy = xy - currentPositions[channel];
				int32_t toMovePulses = dxy / mmPerStep[channel];
				float tAllowed = abs(dxy) / vxy;
				if (abs(vxy) < .00001)
				{
					Serial.println(" skip 2");
					continue;
				}
				if (toMovePulses == 0)
				{
					Serial.println(" skip 3");
					continue;
				}
				Serial.print(F(", xy"));
				Serial.print(xy);
				Serial.print(F(", vxy"));
				Serial.print(vxy);
				Serial.print(F(", t"));
				Serial.print(tAllowed);
				Serial.println();
				SetCourseForXY(channel, toMovePulses, tAllowed, false);
			}
			for (int channel = 0; channel < 2; channel++)
			{
				if (command.indexOf((char)('x' + channel)) < 0)
					continue;
				float xy = xyA[channel];
				float vxy = vxyA[channel];
				if (xy < 0)
					xy = 0;
				if (xy > limits[channel])
					xy = limits[channel];
				float dxy = xy - currentPositions[channel];
				int32_t toMovePulses = dxy / mmPerStep[channel];
				float tAllowed = abs(dxy) / vxy;
				if (abs(vxy) < .00001)
					continue;
				if (toMovePulses == 0)
					continue;
				int32_t start = millis();
				while (abs(currentPositions[channel] - xy) > PIDErrorInMM)
				{
					MotorLoopXY(true, true);
#if BuildForPIDCalibraiton
					if (millis() - start > 1000)
#else
					if (millis() - start > 50)
#endif
					{
						currentXyStatus = F("Moving");
						currentPumpStatus = F("Idle");
						digitalWrite(SyringePumpEnablePin, 1);
						currentProgress = -1;
						SendStatus();
						start = millis();
					}
				}
				Serial.println("Move done");
			}

			currentXyStatus = F("Idle");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
		}
#endif
		else if ((command.startsWith(F("x")) || command.startsWith(F("y"))) && (command.charAt(1) == '+' || command.charAt(1) == '-'))
		{
			if (coatStatus != 0)
				return;

			float v = 1;
			float toMove = 0;
			if (command.length() == 2)
			{
				toMove = 1;
				v = 1;
			}
			else
			{
				toMove = 10;
				v = 4;
			}
			int8_t channel = command.startsWith(F("x")) ? 0 : 1;
			if (command.charAt(1) == '-')
				toMove *= -1;

			Serial.println(command);
			float finalPos = currentPositions[channel] + toMove;
			if (finalPos < 0)
				finalPos = 0;
			if (finalPos > limits[channel])
				finalPos = limits[channel];
			toMove = finalPos - currentPositions[channel];
			float tAllowed = abs(toMove) / v;
			int32_t toMovePulses = toMove / mmPerStep[channel];
			if (toMovePulses != 0)
				SetCourseForXY(channel, toMovePulses, tAllowed, false);
			int32_t start = millis();
			while (abs(currentPositions[channel] - finalPos) > PIDErrorInMM)
			{
				MotorLoopXY(true, true);
				if (millis() - start > 50)
				{
					currentXyStatus = F("Moving");
					currentPumpStatus = F("Idle");
					digitalWrite(SyringePumpEnablePin, 1);
					currentProgress = -1;
					SendStatus();
					start = millis();
				}
			}

			currentXyStatus = F("Idle");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
		}
		else if (command == F("z+") || command == F("z++"))
		{
			if (coatStatus != 0)
				return;
			digitalWrite(SyringePumpEnablePin, 0);
			Serial.print(command);
			for (long steps = 0; steps < (command == F("z+") ? 1 : 10) / mmPerStep[2]; steps++)
			{
				if (currentPositions[2] + mmPerStep[2] > limits[2])
					break;
				pumpStep(1);
				if (steps % 200 == 0)
				{
					currentXyStatus = F("Idle");
					currentPumpStatus = F("Moving");
					currentProgress = -1;
					SendStatus();
				}
			}
			currentXyStatus = F("Idle");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
		}
		else if (command == F("z-") || command == F("z--"))
		{
			if (coatStatus != 0)
				return;
			Serial.println("z-");
			digitalWrite(SyringePumpEnablePin, 0);
			for (long steps = 0; steps < (command == F("z-") ? 1 : 10) / mmPerStep[2]; steps++)
			{
				if (currentPositions[2] + mmPerStep[2] < 0)
					break;
				pumpStep(-1, true);
				//delayMicroseconds(50);
				if (steps % 200 == 0)
				{
					currentXyStatus = F("Idle");
					currentPumpStatus = F("Moving");
					currentProgress = -1;
					SendStatus();
				}
			}
			currentXyStatus = F("Idle");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
		}
		else if (command == F("sw resume"))
		{
			currentPositions[0] = Args.Get(F("x")).toFloat();
			currentPositions[1] = Args.Get(F("y")).toFloat();
			currentPositions[2] = Args.Get(F("z")).toFloat();
			currentCountx = requiredCountx;
			currentCounty = requiredCounty;
			coatStatus = 0;
			currentProgress = -1000;
			Serial.println("sw resume");
		}
		else if (command == F("home xy"))
		{
			currentXyStatus = F("Homing X");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
			MoveToHome(0);
			currentXyStatus = F("Homing Y");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
			MoveToHome(1);
			//MoveToHome(2);


			currentPositions[0] = 0;
			currentPositions[1] = 0;
			//currentPositions[2] = limits[2];
			currentXyStatus = F("Idle");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
		}
		else if (command == F("ztop"))
		{
			if (coatStatus != 0)
				return;
			currentXyStatus = F("Idle");
			currentPumpStatus = F("Homing Z");
			digitalWrite(SyringePumpEnablePin, 0);
			currentProgress = -1;
			SendStatus();
			MoveToHome(2);
			currentPositions[2] = limits[2];

			currentXyStatus = F("Idle");
			currentPumpStatus = F("Idle");
			digitalWrite(SyringePumpEnablePin, 1);
			currentProgress = -1;
			SendStatus();
		}
		else if (command == F("mx") || command == F("my"))
		{
			SetCourseForXY(command.charAt(1) == 'x' ? 0 : 1, Args.Get(F("d")).toFloat(), Args.Get(F("t")).toFloat(), false);
		}
#if BuildForPIDCalibraiton
		else
		{
			Serial.print(F("unknown command: "));
			Serial.println(command);
		}
#endif
	}
}
