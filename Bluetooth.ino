#include <SoftwareSerial.h>
SoftwareSerial BT(2, 3);
String cmd;
byte temperature = 0;
int last_temp;

void SendSensor(){
   
  //블루투스 출력 
  String message = "Sensor";

  if((int)temperature<100) message+="0";
  if((int)temperature<10) message+="0";
  message+=((int)temperature);
  BT.println(message);
  last_temp = (int)temperature;
}
void setup() {
  Serial.begin(9600);
  BT.begin(9600);
}

void loop(){
   if ((int)temperature!=last_temp ){
    SendSensor();
  }
}
