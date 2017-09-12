FROM duplocloud/msbuild:v1
ADD Git-2.14.1-64-bit.exe C:\\Git-2.14.1-64-bit.exe
RUN powershell.exe -Command Start-Process c:\Git-2.14.1-64-bit.exe -ArgumentList '/VERYSILENT' -Wait
ADD zbuilder c:\\zbuilder
CMD ["c:\\zbuilder\\ZbuildMonitor.exe"]