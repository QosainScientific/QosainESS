#include <Arduino.h>

#pragma once
#define tableSize 10
class HashTable
{
//variables
public:
protected:
private:

//functions
public:
	HashTable();
	void Add(String key, String value);
	int Count();
	String Get(String key);
	void ClearAll(void);
	uint8_t isNull(uint8_t index);
	uint8_t ContainsKey(String key);
	String keys[tableSize];
	String values[tableSize];
protected:
private:

}; //dictionary
