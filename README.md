# FlexiWallUI

## Versions In This Repository

Abgabeversion ist der letzte commit in branch Master bzw Tag v1.

## Voraussetzungen

Die volgende Soft- und Hardware sollte vorhanden sein um die Anwendung zu verwenden und zu compilieren.

- Git installieren falls noch nicht getan
- Visual Studio 2017
- Microsoft .NET Framework 4.7
- Flexiwall

## Building

Komponentendiagram/ vereinfachtes Klassendiagram siehe FlexiWallUICodeMap
Quellcodedokumentation siehe Help
Projektstruktur??? Wtf? Eventuel C# WPF MVVM-Struktur

Zu nächst sollte die Anwendung von Git geklont werden:

`git clone https://gitlab.mg.inf.tu-dresden.de/komplexpraktika/vmi5_2017_veronese/flexiplosion.git`

Das Projekt ist eine Visual Studio Solution und sollte sich daher einfach Compilieren lassen.\
Die Projekteinrichtung wurde mit VS 2017 vorgenommen und compiliert. Zur Weiterführung sollte es daher ausreichen die Solution mit der gleichen oder neueren VS Version zu öffnen. Für ältere Versionen von Visual Studio muss eventuell die Solution angepasst werden. Dies passiert in der Regel automatisch und sollte kein weiteres Eingreifen erfordern. Codeänderungen sollten durch eine eventuelle Anpassung der Solution nicht notwendig werden.\
Jetzt kann die Anwendung gebaut werden. Das fertige Projekt liegt je nach build-Option hier:

`$(ProjectFolder)\Flexiwall\FlexiwallUI\bin\{Debug|Release}`

Zur Ausführung wird ein aktuelles Windows mit mindestens .Net Framework 4.5 sowie die Flexiwall benötigt. Es gibt eine Einstellung mit der der Emulator für die Flexiwall aktiviert werden kann. So ist es möglich ohne die entsprechende Hardware den Code zu testen.

Es ist keine Installation notwendig.
Es muss die FlexiWallUI.exe im Projectordner "Flexiwall\FlexiwallUI\bin\{Debug|Release}" gestartet werden.


## Bedienung

Eine Installation ist nicht notwendig.\
Um das Programm zu starten, muss es vorher compiliert werden. Siehe Schritt weiter oben. Danach muss nur die FlexiWallUI.exe im entsprechenden Ordner geöffnet werden:

`$(ProjectFolder)\Flexiwall\FlexiwallUI\bin\{Debug|Release}`

Zu erst sieht man einen Auswahlbildschirm. Aufder rechten Seite ist die Interaktionsfläche um die Schollenansicht auszuwählen. Auf der linken Seite hingegen kann die Magische Linse ausgewählt werden.\
Die Magische Linse zeigt verschiedene Aufnahmen des Bildes im Infrarotbereich und mit Röntgenstrahlung. Mit der Tiefe der Interaktion in die Flexiwall wird gesteuert welches Bild in der Linse angezeigt wird.\
Die Schollenansicht hat eine Standartanimation auf dem Bruder Antonio. Eine weitere Animation wird auf der Mutter Zuanna ausgelöst. Diese zwei Animationen, die über zwei Storyboards gesteuert werden sollen nur die Idee aufzeigen. Weitere Storyboards könne für weitere Personen auf dem Bild hinzugefügt werden. Die Tiefe mit der in die Flexiwall hineingedrückt wird steuert die Position der Animation. Das heißt, dass die Animation mittels Rein- und Rausdrücken vorwärts und rückwärts abgespielt wird. Wenn circa die maximale Tiefe erreicht ist, wird die Animation eingefroren. Dadurch können Informationen in den Sprechblasen gelesen werden, ohne dass stark in die Flexiwall gedrückt werden muss. Durch Loslassen bzw. Rausziehen der Flexiwall resettet das Verhalten.

### Hotkeys

* F10: Toggle für Opütionsmenü

### Navigation mit Emulator

Es ist möglich das Programm mit der Maus zu steuern. Dafür simuliert die Maus die Position einer Hand während man mit dem Mausrad die Tiefe der Hand veränern kann.
Mausrad nach oben simuliert ein Hineindrücken und vice versa.

## Erweiterte Hilfe

Um den Einstieg in das Projekt zu erleichtern gibt es eine CodeMap als Komponentendiagram bzw. vereinfachtes Klassendiagram.\
Siehe dazu FlexiWallUICodeMap in:

`$(ProjectFolder)\Flexiwall`

Zusätzlich gibt es eine Quellcodedokumentation. Siehe Help:

`$(ProjectFolder)\Flexiwall\Help`
