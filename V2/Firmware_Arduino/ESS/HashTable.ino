
/* 
* dictionary.cpp
*
* Created: 4/7/2016 7:54:30 PM
* Author: uetian
*/

#include "HashTable.h"

void HashTable::ClearAll(void)
{  
  for (uint8_t i = 0; i < tableSize; i++) 
  {
    keys[i] = "";
    values[i] = "";
  }
}
uint8_t HashTable::isNull(uint8_t index)
{
  return keys[index].length() == 0;
}
int HashTable::Count()
{
  int c = 0;
  for (int i = 0; i < tableSize; i++)
    if (keys[i] != F(""))
      c++;
  return c;
}
void HashTable::Add(String key, String value)
{
  for (uint8_t i = 0; i < tableSize; i++)
  {
    if (keys[i].equals(""))
    {
      keys[i] = key;
      values[i] = value;
      return;
    }
  }
}


uint8_t HashTable::ContainsKey(String key)
{
  for (uint8_t i = 0; i < tableSize; i++)
  {
    if (keys[i].equals(key))
    {
      return 1;
    }
  }
  return 0;
}
String HashTable::Get(String key)
{
  for (uint8_t i = 0; i < tableSize; i++)
  {
    if (keys[i].equals(key))
    {
      return values[i];
    }
  }
}
// default constructor
HashTable::HashTable()
{
} //dictionary
