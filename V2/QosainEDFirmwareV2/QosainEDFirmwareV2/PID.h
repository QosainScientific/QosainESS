// PID.h

#ifndef _PID_h
#define _PID_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class PIDClass
{
private:
	//float lastSignal = 0;
	//float avgSetPoint = 0;
 public:
	 PIDClass(float Kp_, float Ki_, float Kd_);
	 float Signal(float ref, float position, float dt);
	 float I = 0;// , setPointAvgFactor = 0;
	float _Kp = 0, _Ki = 0, _Kd = 0, _min = 0, _max = 255;
	//float _Bp = 1, _Bi = 1, _Bd = 1;
	//float _pre_error = 0;
	float _integral = 0;
};

#endif

