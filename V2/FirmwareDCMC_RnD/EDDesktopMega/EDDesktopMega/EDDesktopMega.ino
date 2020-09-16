// Include the correct display library
// For a connection via I2C using Wire include
#include "PID.h"
#define PinUp 51
#define PinDown 53

uint32_t lastVelocityControlMillis = 0;
//int32_t finalRequiredCount = 0;
//int32_t startingCount = 0;
//uint32_t completeUntil = 0;
//uint32_t startingMillis = 0;
//int32_t requiredCount = 0;
volatile int32_t currentCount = 0;
uint32_t lastMillis = 0;
uint32_t lastSerialMillis = 0;
PIDClass PositionController = PIDClass(5, 0.0001, 0);
int8_t dir = 0;

uint8_t PG1_CWMap[] = { 0,0,1,1 };
uint8_t PG2_CWMap[] = { 0,1,1,0 };
int8_t lastPos = 0;
uint8_t readPos();
void handleInterrupt();
void setup() {
	Serial.begin(115200);
	Serial.println(F("Begin"));
	delay(500);
	//pinMode(PinEnable, OUTPUT);
	//pinMode(PinA, OUTPUT);
	//pinMode(PinB, OUTPUT);
	pinMode(LED_BUILTIN, 1);
	//pinMode(PinUp, INPUT_PULLUP);
	//pinMode(PinDown, INPUT_PULLUP);	
	//analogWrite(PinEnable, 100);
	//digitalWrite(PinA, 0);
	//digitalWrite(PinB, 1);
	DDRH |= 1 << 3; // PG1
	DDRH |= 1 << 4;	// PG2
	DDRE |= (1 << 3); // EN

	TCCR3A = (1 << COM3A1) | (0 << COM3A0) | (0 << COM3B1) | (0 << COM3B0) | (0 << COM3C1) | (0 << COM3C0) | (1 << WGM31) | (0 << WGM10);
	TCCR3B = (0 << ICNC3) | (0 << ICES3) | (1 << WGM33) | (1 << WGM32) | (0 << CS32) | (0 << CS31) | (1 << CS30);
	//TCCR3C = 0b00001001;
	ICR3 = 450;
	/*attachInterrupt(digitalPinToInterrupt(PinPG1), handleInterrupt, CHANGE);
	attachInterrupt(digitalPinToInterrupt(PinPG2), handleInterrupt, CHANGE);*/
	PositionController._max = 100;
	PositionController._min = -100;
	lastPos = readPos();
	PORTH = 0;
}
bool _lastDir = 0;
void handleInterrupt()
{
	uint8_t pos = readPos();
	if (pos != lastPos)
	{
		if (pos == (lastPos + 1) % 4) // fwd
		{
			currentCount++;
			_lastDir = 1;
		}
		if (pos == (lastPos + 3) % 4) // rev
		{
			currentCount--; _lastDir = 0;
		}
		else
		{
			if (_lastDir == 1)
				currentCount++;
			else
				currentCount--;
			// we skipped something
		}
		lastPos = pos;
	}
}
String PadLeft(String str, String pad, int length)
{
	while (str.length() < length)
		str = pad + str;
	return str;
}
uint8_t readPos()
{
	uint8_t pg1 = (PINJ >> 0) % 2;
	uint8_t pg2 = (PINJ >> 1) % 2;
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
uint8_t lastPrintedPos = 0;
void motorWrite(int power)
{
	if (power > 0)
		OCR3A = power * 2 + 250;
	else if (power < 0)
		OCR3A = (-power) * 2 + 250;
	else
		OCR3A = 0;

	uint8_t current = PORTH;
	current &= ~(0b11 << 3);
	if (power < 0)
	{
		current |= 1 << 3;
	}
	else if (power > 0)
	{
		current |= 1 << 4;
	}
	PORTH = current;
}
int32_t requiredCount = 0;
float aMax = 6000, vMax = 4000;
float time0 = 0, s0 = 0;
float time1 = 0, s1 = 0;
float time2 = 0, s2 = 0;
float time3 = 0, s3 = 0;
float vTravel = 0;
bool reverse = false;

void loop()
{
	if (millis() > lastVelocityControlMillis)
	{
		lastVelocityControlMillis = millis();
		if (millis() < time1 * 1000) // rising
		{
			float t = (millis() / 1000.0F - time0);
			if (!reverse)
				requiredCount = s0 + 0 + 0.5F * aMax * t * t;
			else
				requiredCount = s0 + 0 - 0.5F * aMax * t * t;
		}
		else if (millis() < time2 * 1000) // drag
		{
			if (!reverse)
				requiredCount = s1 + vTravel * (millis() / 1000.0F - time1);
			else
				requiredCount = s1 - vTravel * (millis() / 1000.0F - time1);
		}
		else if (millis() < time3 * 1000) // falling
		{
			float t = (millis() / 1000.0F - time2);
			if (!reverse)
				requiredCount = s2 + vTravel * t - 0.5F * aMax * t * t;
			else
				requiredCount = s2 - (vTravel * t - 0.5F * aMax * t * t);
		}
		else // overdue
		{
			//if (!reverse)
				requiredCount = s3;
			//else
			//requiredCount = s3;
		}
	}

	//delay(1);
	int32_t dtI = micros() - lastMillis;
	float dt = dtI / 1000000.0F;
	float S = PositionController.Signal(requiredCount, currentCount, dt);
	motorWrite(S);
	uint8_t pos = readPos();
	if (lastPos != pos)
	{
		lastPrintedPos = pos;
		if (pos != lastPos)
		{
			if (pos == (lastPos + 1) % 4) // fwd
			{
				currentCount++;
				_lastDir = 1;
			}
			else if (pos == (lastPos + 3) % 4) // rev
			{
				currentCount--;
				_lastDir = 0;
			}
			else
			{
				if (_lastDir == 1)
					currentCount++;
				else
					currentCount--;
				// we skipped something
			}
			lastPos = pos;
		}
	}
	if (millis() - lastSerialMillis > 100)
	{
		lastSerialMillis = millis();
		Serial.print(F("cc: "));
		Serial.print(currentCount);
		Serial.print(F(", rc: "));
		Serial.print(requiredCount);
		Serial.print(F(", OCR: "));
		Serial.print(OCR3A);
		Serial.print(F(", t = "));
		Serial.println(millis() / 1000.0F - time0);

	}
	if (Serial.available())
	{
		float finalRequiredCount = Serial.readStringUntil(',').toInt();
		float tAllowed = Serial.readStringUntil('\n').toFloat();
		float sToTravel = finalRequiredCount - currentCount;
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

		if (y2 > vMax) // trapezium
		{
			Serial.println(F("Its a trapezium"));
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
				Serial.println(F("Travel not possible 1"));
				return;
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
			vTravel = _vTravel;
		}
		else// its a triangle
		{
			Serial.println(F("Its a triangle"));
			// lets see if acc or dec is possible
			if (vEnd > vStart)
			{
				if ((vEnd - vStart) / tAllowed > aMax)
				{
					Serial.println(F("Travel not possible 2"));
					return;
				}
			}
			else
			{
				if ((vEnd - vStart) / tAllowed < -aMax)
				{
					Serial.println(F("Travel not possible 3"));
					return;
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
				Serial.println(F("Travel not possible 4"));
				Serial.println(area);
				Serial.println(sToTravel);
				return;
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
			vTravel = _vTravel;
		}
		time0 = millis() / 1000.0F;
		time1 = time0 + t_1b;
		time2 = time1 + t_2b;
		time3 = time2 + t_3b;
		if (!isNeg)
		{
			s0 = currentCount;
			s1 = s0 + 0 + 0.5F * aMax * t_1b * t_1b;
			s2 = s1 + vTravel * t_2b;
			s3 = s0 + sToTravel;
		}
		else
		{
			s0 = currentCount;
			s1 = s0 - (0 + 0.5F * aMax * t_1b * t_1b);
			s2 = s1 - vTravel * t_2b;
			s3 = s0 - sToTravel;
		}
		reverse = isNeg;
		Serial.print(F("reverse = ")); Serial.println(reverse);
		Serial.print(F("vTravel = ")); Serial.println(vTravel);
		Serial.print(F("time0 = ")); Serial.println(time0);
		Serial.print(F("time1 = ")); Serial.println(time1);
		Serial.print(F("time2 = ")); Serial.println(time2);
		Serial.print(F("time3 = ")); Serial.println(time3);
		Serial.print(F("s0 = ")); Serial.println(s0);
		Serial.print(F("s1 = ")); Serial.println(s1);
		Serial.print(F("s2 = ")); Serial.println(s2);
		Serial.print(F("s3 = ")); Serial.println(s3);
		lastVelocityControlMillis = millis() - 1;
	}
}