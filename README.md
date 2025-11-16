# DODGE THE CREEPS - README

Installation (Windows):
------------------------------
1. Udpak ZIP-filen til en valgfri mappe.
2. Sørg for at filen "DodgeTheCreeps.exe" og mappen 
   "data_Dodge the Creeps_windows_x86_64" ligger i samme mappe.
   Eksempel:
      DodgeTheCreeps/
        ├─ DodgeTheCreeps.exe
        └─ data_Dodge the Creeps_windows_x86_64/

Sådan spiller du (Windows):
------------------------------
- Dobbeltklik på "DodgeTheCreeps.exe" for at starte spillet.
- Brug piletasterne eller WASD til at bevæge dig.
- Undgå fjenderne så længe du kan for at få en høj score.

Tips (Windows):
------------------------------
- Hvis spillet ikke starter, så prøv at højreklikke på
  "DodgeTheCreeps.exe" og vælg "Kør som administrator".
- Du kan også tjekke, at du har de nyeste .NET runtime-filer
  installeret (kræves kun i visse Windows-versioner).


Installation (macOS):
------------------------------
1. Download og unzip ZIP-filen.
2. Du bør ende med en mappe som indeholder:

   DodgeTheCreepsMac/Dodge the Creeps.app

3. Fjern macOS quarantine i Terminal:

   xattr -rd com.apple.quarantine "Dodge the Creeps.app"

4. Gør appens binære filer eksekverbare:

   chmod +x "Dodge the Creeps.app/Contents/MacOS"/*

5. Fix 1 (mest vigtig): Ad-hoc kodesigning  
   Dette step løser problemet hvor spillet bliver "killed" ved launch.

   codesign --force --deep --sign - "Dodge the Creeps.app"

6. Start spillet manuelt første gang:

   "./Dodge the Creeps.app/Contents/MacOS/Dodge the Creeps"

Efter dette burde spillet kunne åbnes normalt fremover, også via Finder.

Sådan spiller du (macOS):
------------------------------
- Start appen som enhver anden Mac-applikation.
- Brug piletasterne eller WASD til at bevæge dig.
- Undgå fjenderne så længe du kan for at få en høj score.


W.I.P:
------------------------------
- 

Kontakt:
------------------------------
Udviklet af: Gustav Færmann Lassen (Som følge af Godot's egen 2D tutorial)  
Version: 1.0  
Godot 4.5 (C# version)

Tak fordi du prøver mit spil.
