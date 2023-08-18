# Chart Sanitizer

This is a simple tool I made for myself to take `song.ini` files (used by FoFiX, Phase Shift, Clone Hero, etc.) and output them in a standardized manner. It helps clean up a bunch of fields that contain default values and can even do things like make albums into playlists internally.

This tool may or may not get more functionality in the future.

## Use

```text
Chart Sanitizer
--------------------
Reads in one or more song.ini files and normalizes them.

Usage: ChartSanitizer.exe [options] <path> ...

Options:
    -pfa       Set playlist values from album info
    -pl=<pl>   Set playlist name
    -spl=<spl> Set sub-playlist name
```

## External Libraries

This code does not employ any external libraries, but the INI reader/writer code is borrowed from my other project, [SabreTools](https://github.com/SabreTools/SabreTools).
