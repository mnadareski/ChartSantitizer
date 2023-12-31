﻿using System;
using System.IO;

namespace ChartSanitizer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // If we have no arguments, we can't do anything
            if (args == null || args.Length == 0)
            {
                DisplayHelp("At least one path must be provided!");
                return;
            }

            // Loop through the inputs and perform the updates
            var setter = new Setter();
            bool options = true;
            foreach (string arg in args)
            {
                #region Clone Hero

                if (options && arg == "-pfa")
                {
                    setter.PlaylistFromAlbum = true;
                    continue;
                }
                else if (options && arg.StartsWith("-pl="))
                {
                    setter.Playlist = arg.Split('=')[1];
                    continue;
                }
                else if (options && arg.StartsWith("-spl="))
                {
                    setter.SubPlaylist = arg.Split('=')[1];
                    continue;
                }

                #endregion

                #region FoFiX

                else if (options && arg.StartsWith("-uid="))
                {
                    setter.UnlockId = arg.Split('=')[1];
                    continue;
                }
                else if (options && arg.StartsWith("-ur="))
                {
                    setter.UnlockRequire = arg.Split('=')[1];
                    continue;
                }
                else if (options && arg.StartsWith("-ut="))
                {
                    setter.UnlockText = arg.Split('=')[1];
                    continue;
                }

                #endregion

                else
                {
                    options = false;
                }

                // Process the argument as a path
                ProcessPath(arg, setter);
            }
        }

        /// <summary>
        /// Display the help text
        /// </summary>
        /// <param name="error">Optional error string to display before the help text</param>
        private static void DisplayHelp(string? error = null)
        {
            // If we have an error, print it
            if (!string.IsNullOrWhiteSpace(error))
                Console.WriteLine(error);

            Console.WriteLine("Chart Sanitizer");
            Console.WriteLine("--------------------");
            Console.WriteLine("Reads in one or more song.ini files and normalizes them.");
            Console.WriteLine();
            Console.WriteLine("Usage: ChartSanitizer.exe [options] <path> ...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  -pfa       Set playlist values from album info (CH)");
            Console.WriteLine("  -pl=<pl>   Set playlist name (CH)");
            Console.WriteLine("  -spl=<spl> Set sub-playlist name (CH)");
            Console.WriteLine("  -uid=<uid> Set unlock ID (FoFiX)");
            Console.WriteLine("  -ur=<ur>   Set unlock requirement (FoFiX)");
            Console.WriteLine("  -ut=<ut>   Set unlock text (FoFiX)");
        }

        /// <summary>
        /// Process a single file or directory path
        /// </summary>
        /// <param name="path">Path to process</param>
        /// <param name="setter">Setter containing santization information</param>
        private static void ProcessPath(string? path, Setter setter)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (File.Exists(path))
            {
                ProcessFile(path, setter);
            }
            else if (Directory.Exists(path))
            {
                foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    ProcessFile(file, setter);
                }
            }
            else
            {
                Console.WriteLine($"'{path}' is not a valid file or directory, skipping...");
            }
        }

        /// <summary>
        /// Process a single file
        /// </summary>
        /// <param name="file">File path to process</param>
        /// <param name="setter">Setter containing santization information</param>
        private static void ProcessFile(string? file, Setter setter)
        {
            if (string.IsNullOrWhiteSpace(file))
                return;

            // If the filename isn't `song.ini`
            if (Path.GetFileName(file).ToLowerInvariant() != "song.ini")
            {
                Console.WriteLine($"'{file}' is not named `song.ini`, skipping...");
                return;
            }

            Console.WriteLine($"Sanitizing '{file}'...");
            bool result = Sanitize(file, setter);
            if (result)
                Console.WriteLine("Sanitizing succeeded!");
            else
                Console.WriteLine("Sanitizing failed!");
        }

        /// <summary>
        /// Sanitize a single song.ini path, if possible
        /// </summary>
        /// <param name="path">Path to the song.ini file</param>
        /// <param name="setter">Setter object representing changes</param>
        /// <returns>Indicates if the sanitization was a success or not</returns>
        private static bool Sanitize(string? path, Setter setter)
        {
            // Create the file we're going to be using
            var songIni = SongIniWrapper.ReadFromFile(path);
            if (songIni == null)
                return false;

            // Normalize the internal values
            SongIniWrapper.Normalize(songIni, setter);

            // Write the file back out
            return SongIniWrapper.WriteToFile(songIni, path);
        }
    }
}