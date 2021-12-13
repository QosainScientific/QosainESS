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
    //avgSetPoint = setpoint * (1 - setPointAvgFactor) + avgSetPoint * setPointAvgFactor;
    //setpoint = avgSetPoint;
    float _Kp = this->_Kp;// +this->_Kp * (1 - abs(lastSignal) / _max) * _Bp;
    // Calculate error
    float error = setpoint - pv;

    // Proportional term
    float Pout = _Kp * error;

    // Integral term
    _integral += error * _dt;
    if (_Ki == 0)
        _integral = 0;
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
    //float derivative = (error - _pre_error) / _dt;
    //float Dout = _Kd * derivative;

    // Calculate total output
    float output = Pout + Iout;// +Dout;

    // Restrict to max/min
    if (output > _max)
        output = _max;
    else if (output < _min)
        output = _min;

    // Save error to previous error
    //_pre_error = error;
    //lastSignal = output;
    return output;
}