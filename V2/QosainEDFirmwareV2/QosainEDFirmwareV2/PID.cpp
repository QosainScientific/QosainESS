// 
// 
// 

#include "PID.h"
PIDClass::PIDClass(float Kp_, float Ki_, float Kd_)
{
	_Kp = Kp_; _Ki = Ki_; _Kd = Kd_;
	I = 0;
}

float PIDClass::Signal(float setpoint, float pv, float _dt)
{
    // Calculate error
    float error = setpoint - pv;

    // Proportional term
    float Pout = _Kp * error;

    // Integral term
    _integral += error * _dt;
    // Restrict to max/min
    float Iout = _Ki * _integral;

    if (_Ki != 0)
    {
        if (_integral * _Ki > _max)
            _integral = _max / _Ki;
        else if (_integral * _Ki < _min)
            _integral = _min / _Ki;
    }
    // Derivative term
    float derivative = (error - _pre_error) / _dt;
    float Dout = _Kd * derivative;

    // Calculate total output
    float output = Pout + Iout + Dout;

    // Restrict to max/min
    if (output > _max)
        output = _max;
    else if (output < _min)
        output = _min;

    // Save error to previous error
    _pre_error = error;

    return output;
}