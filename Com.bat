@echo off
REM "C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\tlbexp.exe" Robotics\bin\Release\Robotics.dll /out:Robotics\bin\Release\Robotics.tlb > Robotics\bin\Release\tlbexp.log.txt 2>&1
REM start notepad.exe Robotics\bin\Release\tlbexp.log.txt

"C:\Windows\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe" Robotics\bin\Release\Robotics.dll /nologo /unregister /verbose > Robotics\bin\Release\regasm.log.txt 2>&1
"C:\Windows\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe" Robotics\bin\Release\Robotics.dll /nologo /verbose /tlb:Robotics\bin\Release\Robotics.tlb >> Robotics\bin\Release\regasm.log.txt 2>&1
"C:\Windows\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe" Robotics\bin\Release\Robotics.dll /silent /nologo /regfile:Robotics\bin\Release\Robotics.reg
start notepad.exe Robotics\bin\Release\regasm.log.txt
REM pause