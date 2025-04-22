#include "Display_ST7789.h"
#include "LVGL_Driver.h"
#include "ui.h"

// Переменные для хранения данных
float cpuUsage = 0.0;
float freeMemory = 0.0;
float gpuUsage = 0.0;
long freeMemoryGpu = 0;
float cpuTemp = 0.0;
float gpuTemp = 0.0;
float cpuFan = 0.0;
float cpuCores = 0.0;
float gpuClock = 0.0;
float cpuClock = 0.0;
float gpuFan = 0.0; // Добавляем переменную для GPU Fan

void setup() {
  Serial.begin(115200);
  while (!Serial) {
    delay(10);
  }
  delay(3000);

  Serial.println("ESP32-C6-LCD-1.47 Started");

  LCD_Init();
  Lvgl_Init();

  Serial.println("Initializing UI...");
  ui_init();
  Serial.println("UI initialized");

  if (ui_arc_cpu == NULL || ui_arc_gpu == NULL || 
      ui_label_cpuclock == NULL || ui_label_cputemp == NULL || ui_label_mem == NULL || 
      ui_label_cpucores == NULL || ui_label_cpufan == NULL || ui_label_gputemp == NULL || 
      ui_label_gpumem == NULL || ui_label_gpuclock == NULL || ui_label_gpufan == NULL) { // Добавляем проверку ui_label_gpufan
    Serial.println("Error: One or more UI objects are NULL");
    while (1);
  }

  Serial.println("Connected");
}

void loop() {
  if (Serial.available() > 0) {
    String data = Serial.readStringUntil('\n');
    data.trim();
    Serial.println("Received: " + data);
    parseData(data);
    updateUI();
  }
  Timer_Loop();
}

void parseData(String data) {
  Serial.println("Parsing: " + data);

  if (!data.startsWith("CPU:")) {
    Serial.println("Invalid data format: " + data);
    return;
  }

  int startIndex = 0;
  int endIndex;

  startIndex = data.indexOf("CPU:") + 4;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: CPU");
    return;
  }
  cpuUsage = data.substring(startIndex, endIndex).toFloat();

  startIndex = data.indexOf("MEM:") + 4;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: MEM");
    return;
  }
  freeMemory = data.substring(startIndex, endIndex).toFloat();

  startIndex = data.indexOf("GPU:") + 4;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: GPU");
    return;
  }
  gpuUsage = data.substring(startIndex, endIndex).toFloat();

  startIndex = data.indexOf("GPUMEM:") + 7;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: GPUMEM");
    return;
  }
  freeMemoryGpu = data.substring(startIndex, endIndex).toInt();

  startIndex = data.indexOf("CPUTEMP:") + 8;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: CPUTEMP");
    return;
  }
  String cpuTempStr = data.substring(startIndex, endIndex);
  Serial.println("Raw CPUTEMP string: " + cpuTempStr);
  cpuTemp = cpuTempStr.toFloat();
  Serial.printf("Parsed CPUTEMP: %.1f\n", cpuTemp);

  startIndex = data.indexOf("GPUTEMP:") + 8;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: GPUTEMP");
    return;
  }
  String gpuTempStr = data.substring(startIndex, endIndex);
  Serial.println("Raw GPUTEMP string: " + gpuTempStr);
  gpuTemp = gpuTempStr.toFloat();
  Serial.printf("Parsed GPUTEMP: %.1f\n", gpuTemp);

  startIndex = data.indexOf("CPUCLOCK:") + 9;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: CPUCLOCK");
    return;
  }
  String cpuClockStr = data.substring(startIndex, endIndex);
  Serial.println("Raw CPUCLOCK string: " + cpuClockStr);
  cpuClock = cpuClockStr.toFloat();
  Serial.printf("Parsed CPUCLOCK: %.2f\n", cpuClock);

  startIndex = data.indexOf("CPUFAN:") + 7;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: CPUFAN");
    return;
  }
  cpuFan = data.substring(startIndex, endIndex).toFloat();

  startIndex = data.indexOf("CPUCORES:") + 9;
  endIndex = data.indexOf(";", startIndex);
  if (endIndex == -1) {
    Serial.println("Parse error: CPUCORES");
    return;
  }
  cpuCores = data.substring(startIndex, endIndex).toFloat();

  startIndex = data.indexOf("GPUCLOCK:") + 9;
  endIndex = data.indexOf(";", startIndex); // Исправляем, добавляем поиск ";"
  if (endIndex == -1) {
    Serial.println("Parse error: GPUCLOCK");
    return;
  }
  String gpuClockStr = data.substring(startIndex, endIndex);
  Serial.println("Raw GPUCLOCK string: " + gpuClockStr);
  gpuClock = gpuClockStr.toFloat();
  Serial.printf("Parsed GPUCLOCK: %.2f\n", gpuClock);

  startIndex = data.indexOf("GPUFAN:") + 7;
  endIndex = data.length();
  String gpuFanStr = data.substring(startIndex, endIndex);
  Serial.println("Raw GPUFAN string: " + gpuFanStr);
  gpuFan = gpuFanStr.toFloat();
  Serial.printf("Parsed GPUFAN: %.0f\n", gpuFan);

  Serial.printf("Parsed: CPU=%.1f, MEM=%.0f, GPU=%.1f, GPUMEM=%ld, CPUTEMP=%.1f, GPUTEMP=%.1f, CPUCLOCK=%.2f, CPUFAN=%.0f, CPUCORES=%.1f, GPUCLOCK=%.2f, GPUFAN=%.0f\n",
                cpuUsage, freeMemory, gpuUsage, freeMemoryGpu, cpuTemp, gpuTemp, cpuClock, cpuFan, cpuCores, gpuClock, gpuFan);
}

void updateUI() {
  char buffer[64];

  Serial.printf("Updating UI: CPU=%.1f, GPU=%.1f\n", cpuUsage, gpuUsage);

  if (cpuUsage < 0 || cpuUsage > 100) cpuUsage = 0;
  if (gpuUsage < 0 || gpuUsage > 100) gpuUsage = 0;
  if (cpuTemp < -40 || cpuTemp > 150) cpuTemp = 0;
  if (gpuTemp < -40 || gpuTemp > 150) gpuTemp = 0;
  if (cpuClock < 0 || cpuClock > 5) cpuClock = 0;
  if (gpuClock < 0 || gpuClock > 5) gpuClock = 0;
  if (cpuFan < 0 || cpuFan > 10000) cpuFan = 0;
  if (cpuCores < 0 || cpuCores > 100) cpuCores = 0;
  if (freeMemory < 0) freeMemory = 0;
  if (freeMemoryGpu < 0) freeMemoryGpu = 0;
  if (gpuFan < 0 || gpuFan > 100) gpuFan = 0; // Проверка для GPU Fan (в процентах)

  Serial.println("Setting CPU Arc...");
  lv_arc_set_value(ui_arc_cpu, (int)cpuUsage);
  Serial.println("Setting GPU Arc...");
  lv_arc_set_value(ui_arc_gpu, (int)gpuUsage);

  Serial.println("Setting Label1...");
  Serial.printf("Before snprintf CPUCLOCK: %.2f\n", cpuClock);
  snprintf(buffer, sizeof(buffer), "%.2f GHz", cpuClock);
  Serial.printf("After snprintf: %s\n", buffer);
  lv_label_set_text(ui_label_cpuclock, buffer);

  Serial.println("Setting Label2...");
  Serial.printf("Before snprintf CPUTEMP: %.1f\n", cpuTemp);
  snprintf(buffer, sizeof(buffer), "t: %.1f°C", cpuTemp);
  Serial.printf("After snprintf: %s\n", buffer);
  lv_label_set_text(ui_label_cputemp, buffer);

  Serial.println("Setting Label3...");
  snprintf(buffer, sizeof(buffer), "m: %.0f MB", freeMemory);
  lv_label_set_text(ui_label_mem, buffer);

  Serial.println("Setting Label4...");
  snprintf(buffer, sizeof(buffer), "Cores: %.1f%%", cpuCores);
  lv_label_set_text(ui_label_cpucores, buffer);

  Serial.println("Setting Label5...");
  snprintf(buffer, sizeof(buffer), "F: %.0f RPM", cpuFan);
  lv_label_set_text(ui_label_cpufan, buffer);

  Serial.println("Setting Label6...");
  Serial.printf("Before snprintf GPUCLOCK: %.2f\n", gpuClock);
  snprintf(buffer, sizeof(buffer), "%.2f GHz", gpuClock);
  Serial.printf("After snprintf: %s\n", buffer);
  lv_label_set_text(ui_label_gpuclock, buffer);

  Serial.println("Setting Label7...");
  Serial.printf("Before snprintf GPUTEMP: %.1f\n", gpuTemp);
  snprintf(buffer, sizeof(buffer), "t: %.1f°C", gpuTemp);
  Serial.printf("After snprintf: %s\n", buffer);
  lv_label_set_text(ui_label_gputemp, buffer);

  Serial.println("Setting Label8...");
  snprintf(buffer, sizeof(buffer), "m: %ld MB", freeMemoryGpu);
  lv_label_set_text(ui_label_gpumem, buffer);

  Serial.println("Setting Label9...");
  snprintf(buffer, sizeof(buffer), "F: %.0f%%", gpuFan); // Отображаем в процентах
  lv_label_set_text(ui_label_gpufan, buffer);

  Serial.println("UI update completed");
}