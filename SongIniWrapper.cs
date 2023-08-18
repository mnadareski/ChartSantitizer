using System;
using System.IO;
using System.Linq;

namespace ChartSanitizer
{
    internal static class SongIniWrapper
    {
        /// <summary>
        /// Read a SongIni from an input file, if possible
        /// </summary>
        /// <param name="path">Path to the song.ini file</param>
        /// <returns>Nullable SongIni representing the parsed data</returns>
        public static SongIni? ReadFromFile(string? path)
        {
            // If we don't have a valid path, we can't do anything
            if (string.IsNullOrWhiteSpace(path))
                return null;
            else if (!File.Exists(path))
                return null;

            // Setup the file we'll be using
            var songIni = new SongIni();

            try
            {
                using IniReader reader = new IniReader(path) { ValidateRows = false };

                while (!reader.EndOfStream)
                {
                    // If we have no next line
                    if (!reader.ReadNextLine())
                        break;

                    // Process each row type
                    switch (reader.RowType)
                    {
                        // We ignore empty, comment, and invalid rows
                        case IniRowType.None:
                        case IniRowType.Comment:
                        case IniRowType.Invalid:
                            continue;

                        // Only [song] or [Song] are allowed section names
                        case IniRowType.SectionHeader:
                            string? sectionName = reader.Section?.ToLowerInvariant();
                            switch (sectionName)
                            {
                                case "song":
                                    // No-op
                                    break;
                                default:
                                    return null;
                            }
                            break;

                        // Validate and parse all fields
                        case IniRowType.KeyValue:
                            string? key = reader.KeyValuePair?.Key?.ToLowerInvariant();
                            string? value = reader.KeyValuePair?.Value;
                            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                                continue;

                            switch (key)
                            {
                                #region Song/Chart Metadata

                                case "name": songIni.Name ??= value; break;
                                case "artist": songIni.Artist ??= value; break;
                                case "album": songIni.Album ??= value; break;
                                case "genre": songIni.Genre ??= value; break;
                                case "sub_genre": songIni.SubGenre ??= value; break;
                                case "year": songIni.Year ??= value; break;
                                case "charter": songIni.Charter ??= value; break;
                                case "frets": songIni.Charter ??= value; break;
                                case "version": songIni.Version ??= ReadLong(value); break;
                                case "album_track": songIni.AlbumTrack ??= ReadLong(value); break;
                                case "track": songIni.AlbumTrack ??= ReadLong(value); break;
                                case "playlist_track": songIni.PlaylistTrack ??= ReadLong(value); break;
                                case "song_length": songIni.SongLength ??= ReadLong(value); break;
                                case "preview_start_time": songIni.PreviewStartTime ??= ReadLong(value); break;
                                case "preview_end_time": songIni.PreviewEndTime ??= ReadLong(value); break;
                                case "loading_phrase": songIni.LoadingPhrase ??= value; break;

                                #endregion

                                #region Song/Chart Metadata (Game-Specific)

                                case "cassettecolor": songIni.CassetteColor ??= value; break;
                                case "tags": songIni.Tags ??= value; break;
                                case "preview": songIni.Preview ??= value?.Split(' ').Select(s => ReadLong(s) ?? -1).ToArray(); break;
                                case "playlist": songIni.Playlist ??= value; break;
                                case "sub_playlist": songIni.SubPlaylist ??= value; break;
                                case "modchart": songIni.Modchart ??= ReadBool(value); break;
                                case "lyrics": songIni.Lyrics ??= ReadBool(value); break;

                                #endregion

                                #region Track Difficulties

                                case "diff_band": songIni.DiffBand ??= ReadLong(value); break;
                                case "diff_guitar": songIni.DiffGuitar ??= ReadLong(value); break;
                                case "diff_guitarghl": songIni.DiffGuitarGHL ??= ReadLong(value); break;
                                case "diff_guitar_coop": songIni.DiffGuitarCoop ??= ReadLong(value); break;
                                case "diff_guitar_coop_ghl": songIni.DiffGuitarCoopGHL ??= ReadLong(value); break;
                                case "diff_guitar_real": songIni.DiffGuitarReal ??= ReadLong(value); break;
                                case "diff_guitar_real_22": songIni.DiffGuitarReal22 ??= ReadLong(value); break;
                                case "diff_rhythm": songIni.DiffRhythm ??= ReadLong(value); break;
                                case "diff_rhythm_ghl": songIni.DiffRhythmGHL ??= ReadLong(value); break;
                                case "diff_bass": songIni.DiffBass ??= ReadLong(value); break;
                                case "diff_bassghl": songIni.DiffBassGHL ??= ReadLong(value); break;
                                case "diff_bass_real": songIni.DiffBassReal ??= ReadLong(value); break;
                                case "diff_bass_real_22": songIni.DiffBassReal22 ??= ReadLong(value); break;
                                case "diff_drums": songIni.DiffDrums ??= ReadLong(value); break;
                                case "diff_drums_real": songIni.DiffDrumsReal ??= ReadLong(value); break;
                                case "diff_drums_real_ps": songIni.DiffDrumsRealPS ??= ReadLong(value); break;
                                case "diff_keys": songIni.DiffKeys ??= ReadLong(value); break;
                                case "diff_keys_real": songIni.DiffKeysReal ??= ReadLong(value); break;
                                case "diff_keys_real_ps": songIni.DiffKeysRealPS ??= ReadLong(value); break;
                                case "diff_vocals": songIni.DiffVocals ??= ReadLong(value); break;
                                case "diff_vocals_harm": songIni.DiffVocalsHarm ??= ReadLong(value); break;
                                case "diff_dance": songIni.DiffDance ??= ReadLong(value); break;

                                #endregion

                                #region Chart Properties

                                case "pro_drums": songIni.ProDrums ??= ReadBool(value); break;
                                case "pro_drum": songIni.ProDrums ??= ReadBool(value); break;
                                case "five_lane_drums": songIni.FiveLaneDrums ??= ReadBool(value); break;
                                case "vocal_gender": songIni.VocalGender ??= value; break;
                                case "real_guitar_tuning": songIni.RealGuitarTuning ??= value; break;
                                case "real_guitar_22_tuning": songIni.RealGuitar22Tuning ??= value; break;
                                case "real_bass_tuning": songIni.RealBassTuning ??= value; break;
                                case "real_bass_22_tuning": songIni.RealBass22Tuning ??= value; break;
                                case "real_keys_lane_count_right": songIni.RealKeysLaneCountRight ??= ReadLong(value); break;
                                case "real_keys_lane_count_left": songIni.RealKeysLaneCountLeft ??= ReadLong(value); break;
                                case "delay": songIni.Delay ??= ReadLong(value); break;
                                case "sustain_cutoff_threshold": songIni.SustainCutoffThreshold ??= ReadLong(value); break;
                                case "hopo_frequency": songIni.HopoFrequency ??= ReadLong(value); break;
                                case "eighthnote_hopo": songIni.EighthNoteHopo ??= ReadBool(value); break;
                                case "multiplier_note": songIni.MultiplierNote ??= ReadLong(value); break;
                                case "star_power_note": songIni.MultiplierNote ??= ReadLong(value); break;

                                #endregion

                                #region Chart Properties (Game-Specific)

                                case "drum_fallback_blue": songIni.DrumFallbackBlue ??= ReadBool(value); break;
                                case "tutorial": songIni.Tutorial ??= ReadBool(value); break;
                                case "boss_battle": songIni.BossBattle ??= ReadBool(value); break;
                                case "hopofreq": songIni.HopoFreq ??= ReadLong(value); break;
                                case "early_hit_window_size": songIni.EarlyHitWindowSize ??= value; break;
                                case "end_events": songIni.EndEvents ??= ReadBool(value); break;
                                case "sysex_slider": songIni.SysExSlider ??= ReadBool(value); break;
                                case "sysex_high_hat_ctrl": songIni.SysExHighHatCtrl ??= ReadBool(value); break;
                                case "sysex_rimshot": songIni.SysExRimshot ??= ReadBool(value); break;
                                case "sysex_open_bass": songIni.SysExOpenBass ??= ReadBool(value); break;
                                case "sysex_pro_slide": songIni.SysExProSlide ??= ReadBool(value); break;
                                case "guitar_type": songIni.GuitarType ??= ReadLong(value); break;
                                case "bass_type": songIni.BassType ??= ReadLong(value); break;
                                case "kit_type": songIni.KitType ??= ReadLong(value); break;
                                case "keys_type": songIni.KeysType ??= ReadLong(value); break;
                                case "dance_type": songIni.DanceType ??= ReadLong(value); break;

                                #endregion

                                #region Images and Other Resources

                                case "icon": songIni.Icon ??= value; break;
                                case "background": songIni.Background ??= value; break;
                                case "video": songIni.Video ??= value; break;
                                case "video_loop": songIni.VideoLoop ??= ReadBool(value); break;
                                case "video_start_time": songIni.VideoStartTime ??= ReadLong(value); break;
                                case "video_end_time": songIni.VideoEndTime ??= ReadLong(value); break;
                                case "cover": songIni.Cover ??= value; break;

                                #endregion

                                #region Images and Other Resources (Game-Specific)

                                case "link_name_a": songIni.LinkNameA ??= value; break;
                                case "link_name_b": songIni.LinkNameB ??= value; break;
                                case "banner_link_a": songIni.BannerLinkA ??= value; break;
                                case "banner_link_b": songIni.BannerLinkB ??= value; break;

                                #endregion

                                #region Miscellaneous (Game-Specific)

                                case "scores": songIni.Scores ??= value; break;
                                case "scores_ext": songIni.ScoresExt ??= value; break;
                                case "count": songIni.Count ??= ReadLong(value); break;
                                case "rating": songIni.Rating ??= ReadLong(value); break;
                                case "unlock_id": songIni.UnlockId ??= value; break;
                                case "unlock_require": songIni.UnlockRequire ??= value; break;
                                case "unlock_text": songIni.UnlockText ??= value; break;
                                case "unlock_completed": songIni.UnlockCompleted ??= ReadLong(value); break;
                                case "eof_midi_import_drum_accent_velocity": songIni.EoFMidiImportDrumAccentVelocity ??= ReadLong(value); break;
                                case "eof_midi_import_drum_ghost_velocity": songIni.EoFMidiImportDrumGhostVelocity ??= ReadLong(value); break;

                                    #endregion
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred while reading '{path}'!");
                Console.WriteLine(ex);
                return null;
            }

            return songIni;
        }

        /// <summary>
        /// Normalize out invalid and default values
        /// </summary>
        /// <param name="songIni">SongIni object to normalize</param>
        /// <param name="setter">Setter object representing changes</param>
        public static void Normalize(SongIni? songIni, Setter setter)
        {
            // If we don't have a valid SongIni, we can't do anything
            if (songIni == null)
                return;

            #region Song/Chart Metadata

            if (songIni.Version < 0) songIni.Version = null;
            if (songIni.AlbumTrack < 0 || songIni.AlbumTrack == 16000) songIni.AlbumTrack = null;
            if (songIni.PlaylistTrack < 0 || songIni.PlaylistTrack == 16000) songIni.PlaylistTrack = null;
            if (songIni.SongLength < 0) songIni.SongLength = null;
            if (songIni.PreviewStartTime < 0) songIni.PreviewStartTime = null;
            if (songIni.PreviewEndTime < 0) songIni.PreviewEndTime = null;

            if (setter.PlaylistFromAlbum)
                songIni.PlaylistTrack = songIni.AlbumTrack;

            #endregion

            #region Song/Chart Metadata (Game-Specific)

            if (songIni.Tags != "cover") songIni.Tags = null;
            if (songIni.Preview?.Length != 2 || songIni.Preview?.Any(l => l < 0) == true) songIni.Preview = null;

            if (setter.PlaylistFromAlbum)
                songIni.Playlist = $"{songIni.Artist} - {songIni.Album} ({songIni.Year})";
            if (!string.IsNullOrWhiteSpace(setter.Playlist))
                songIni.Playlist = setter.Playlist;
            if (!string.IsNullOrWhiteSpace(setter.SubPlaylist))
                songIni.SubPlaylist = setter.SubPlaylist;

            #endregion

            #region Track Difficulties

            if (songIni.DiffBand < 0) songIni.DiffBand = null;
            if (songIni.DiffGuitar < 0) songIni.DiffGuitar = null;
            if (songIni.DiffGuitarGHL < 0) songIni.DiffGuitarGHL = null;
            if (songIni.DiffGuitarCoop < 0) songIni.DiffGuitarCoop = null;
            if (songIni.DiffGuitarCoopGHL < 0) songIni.DiffGuitarCoopGHL = null;
            if (songIni.DiffGuitarReal < 0) songIni.DiffGuitarReal = null;
            if (songIni.DiffGuitarReal22 < 0) songIni.DiffGuitarReal22 = null;
            if (songIni.DiffRhythm < 0) songIni.DiffRhythm = null;
            if (songIni.DiffRhythmGHL < 0) songIni.DiffRhythmGHL = null;
            if (songIni.DiffBass < 0) songIni.DiffBass = null;
            if (songIni.DiffBassGHL < 0) songIni.DiffBassGHL = null;
            if (songIni.DiffBassReal < 0) songIni.DiffBassReal = null;
            if (songIni.DiffBassReal22 < 0) songIni.DiffBassReal22 = null;
            if (songIni.DiffDrums < 0) songIni.DiffDrums = null;
            if (songIni.DiffDrumsReal < 0) songIni.DiffDrumsReal = null;
            if (songIni.DiffDrumsRealPS < 0) songIni.DiffDrumsRealPS = null;
            if (songIni.DiffKeys < 0) songIni.DiffKeys = null;
            if (songIni.DiffKeysReal < 0) songIni.DiffKeysReal = null;
            if (songIni.DiffKeysRealPS < 0) songIni.DiffKeysRealPS = null;
            if (songIni.DiffVocals < 0) songIni.DiffVocals = null;
            if (songIni.DiffVocalsHarm < 0) songIni.DiffVocalsHarm = null;
            if (songIni.DiffDance < 0) songIni.DiffDance = null;

            #endregion

            #region Chart Properties

            if (songIni.VocalGender != "male" && songIni.VocalGender != "female") songIni.VocalGender = null;
            if (songIni.Delay == 0) songIni.Delay = null;
            if (songIni.MultiplierNote != 103 && songIni.MultiplierNote != 116) songIni.MultiplierNote = null;

            #endregion

            #region Chart Properties (Game-Specific)

            if (songIni.EarlyHitWindowSize != "none" && songIni.EarlyHitWindowSize != "half" && songIni.EarlyHitWindowSize != "full") songIni.EarlyHitWindowSize = null;

            #endregion
        
            #region Images and Other Resources

            if (songIni.VideoStartTime == 0) songIni.VideoStartTime = null;
            if (songIni.VideoEndTime < 0) songIni.VideoEndTime = null;

            #endregion

            #region Miscellaneous (Game-Specific)

            if (songIni.Count == 0) songIni.Count = null;
            if (songIni.Rating == 0) songIni.Rating = null;

            #endregion
        }

        /// <summary>
        /// Write a SongIni to an output file, if possible
        /// </summary>
        /// <param name="songIni">SongIni object to write</param>
        /// <param name="path">Path to the song.ini file</param>
        /// <returns>Indicates if the writing was a success or not</returns>
        public static bool WriteToFile(SongIni? songIni, string? path)
        {
            // If we don't have a valid SongIni, we can't do anything
            if (songIni == null)
                return false;

            // If we don't have a valid path, we can't do anything
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                using IniWriter iniWriter = new IniWriter(path);

                // Normalize to "song"
                iniWriter.WriteSection("song");

                #region Song/Chart Metadata

                if (songIni.Name != null) iniWriter.WriteKeyValuePair("name", songIni.Name);
                if (songIni.Artist != null) iniWriter.WriteKeyValuePair("artist", songIni.Artist);
                if (songIni.Album != null) iniWriter.WriteKeyValuePair("album", songIni.Album);
                if (songIni.Genre != null) iniWriter.WriteKeyValuePair("genre", songIni.Genre);
                if (songIni.SubGenre != null) iniWriter.WriteKeyValuePair("sub_genre", songIni.SubGenre);
                if (songIni.Year != null) iniWriter.WriteKeyValuePair("year", songIni.Year);
                if (songIni.Charter != null) iniWriter.WriteKeyValuePair("charter", songIni.Charter);
                if (songIni.Charter != null) iniWriter.WriteKeyValuePair("frets", songIni.Charter);
                if (songIni.Version != null) iniWriter.WriteKeyValuePair("version", songIni.Version.ToString());
                if (songIni.AlbumTrack != null) iniWriter.WriteKeyValuePair("album_track", songIni.AlbumTrack.ToString());
                if (songIni.AlbumTrack != null) iniWriter.WriteKeyValuePair("track", songIni.AlbumTrack.ToString());
                if (songIni.PlaylistTrack != null) iniWriter.WriteKeyValuePair("playlist_track", songIni.PlaylistTrack.ToString());
                if (songIni.SongLength != null) iniWriter.WriteKeyValuePair("song_length", songIni.SongLength.ToString());
                if (songIni.PreviewStartTime != null) iniWriter.WriteKeyValuePair("preview_start_time", songIni.PreviewStartTime.ToString());
                if (songIni.PreviewEndTime != null) iniWriter.WriteKeyValuePair("preview_end_time", songIni.PreviewEndTime.ToString());
                if (songIni.LoadingPhrase != null) iniWriter.WriteKeyValuePair("loading_phrase", songIni.LoadingPhrase);

                #endregion

                #region Song/Chart Metadata (Game-Specific)

                if (songIni.CassetteColor != null) iniWriter.WriteKeyValuePair("cassettecolor", songIni.CassetteColor);
                if (songIni.Tags != null) iniWriter.WriteKeyValuePair("tags", songIni.Tags);
                if (songIni.Preview != null) iniWriter.WriteKeyValuePair("preview", string.Join(' ', songIni.Preview));
                if (songIni.Playlist != null) iniWriter.WriteKeyValuePair("playlist", songIni.Playlist);
                if (songIni.SubPlaylist != null) iniWriter.WriteKeyValuePair("sub_playlist", songIni.SubPlaylist);
                if (songIni.Modchart != null) iniWriter.WriteKeyValuePair("modchart", songIni.Modchart.ToString());
                if (songIni.Lyrics != null) iniWriter.WriteKeyValuePair("lyrics", songIni.Lyrics.ToString());

                #endregion

                #region Track Difficulties

                if (songIni.DiffBand != null) iniWriter.WriteKeyValuePair("diff_band", songIni.DiffBand.ToString());
                if (songIni.DiffGuitar != null) iniWriter.WriteKeyValuePair("diff_guitar", songIni.DiffGuitar.ToString());
                if (songIni.DiffGuitarGHL != null) iniWriter.WriteKeyValuePair("diff_guitarghl", songIni.DiffGuitarGHL.ToString());
                if (songIni.DiffGuitarCoop != null) iniWriter.WriteKeyValuePair("diff_guitar_coop", songIni.DiffGuitarCoop.ToString());
                if (songIni.DiffGuitarCoopGHL != null) iniWriter.WriteKeyValuePair("diff_guitar_coop_ghl", songIni.DiffGuitarCoopGHL.ToString());
                if (songIni.DiffGuitarReal != null) iniWriter.WriteKeyValuePair("diff_guitar_real", songIni.DiffGuitarReal.ToString());
                if (songIni.DiffGuitarReal22 != null) iniWriter.WriteKeyValuePair("diff_guitar_real_22", songIni.DiffGuitarReal22.ToString());
                if (songIni.DiffRhythmGHL != null) iniWriter.WriteKeyValuePair("diff_rhythm_ghl", songIni.DiffRhythmGHL.ToString());
                if (songIni.DiffBass != null) iniWriter.WriteKeyValuePair("diff_bass", songIni.DiffBass.ToString());
                if (songIni.DiffBassReal != null) iniWriter.WriteKeyValuePair("diff_bass_real", songIni.DiffBassReal.ToString());
                if (songIni.DiffBassReal22 != null) iniWriter.WriteKeyValuePair("diff_bass_real_22", songIni.DiffBassReal22.ToString());
                if (songIni.DiffDrums != null) iniWriter.WriteKeyValuePair("diff_drums", songIni.DiffDrums.ToString());
                if (songIni.DiffDrumsReal != null) iniWriter.WriteKeyValuePair("diff_drums_real", songIni.DiffDrumsReal.ToString());
                if (songIni.DiffDrumsRealPS != null) iniWriter.WriteKeyValuePair("diff_drums_real_ps", songIni.DiffDrumsRealPS.ToString());
                if (songIni.DiffKeys != null) iniWriter.WriteKeyValuePair("diff_keys", songIni.DiffKeys.ToString());
                if (songIni.DiffKeysReal != null) iniWriter.WriteKeyValuePair("diff_keys_real", songIni.DiffKeysReal.ToString());
                if (songIni.DiffKeysRealPS != null) iniWriter.WriteKeyValuePair("diff_keys_real_ps", songIni.DiffKeysRealPS.ToString());
                if (songIni.DiffVocals != null) iniWriter.WriteKeyValuePair("diff_vocals", songIni.DiffVocals.ToString());
                if (songIni.DiffVocalsHarm != null) iniWriter.WriteKeyValuePair("diff_vocals_harm", songIni.DiffVocalsHarm.ToString());
                if (songIni.DiffDance != null) iniWriter.WriteKeyValuePair("diff_dance", songIni.DiffDance.ToString());

                #endregion

                #region Chart Properties

                if (songIni.ProDrums != null) iniWriter.WriteKeyValuePair("pro_drums", songIni.ProDrums.ToString());
                if (songIni.ProDrums != null) iniWriter.WriteKeyValuePair("pro_drum", songIni.ProDrums.ToString());
                if (songIni.FiveLaneDrums != null) iniWriter.WriteKeyValuePair("five_lane_drums", songIni.FiveLaneDrums.ToString());
                if (songIni.VocalGender != null) iniWriter.WriteKeyValuePair("vocal_gender", songIni.VocalGender);
                if (songIni.RealGuitarTuning != null) iniWriter.WriteKeyValuePair("real_guitar_tuning", songIni.RealGuitarTuning);
                if (songIni.RealGuitar22Tuning != null) iniWriter.WriteKeyValuePair("real_guitar_22_tuning", songIni.RealGuitar22Tuning);
                if (songIni.RealBassTuning != null) iniWriter.WriteKeyValuePair("real_bass_tuning", songIni.RealBassTuning);
                if (songIni.RealBass22Tuning != null) iniWriter.WriteKeyValuePair("real_bass_22_tuning", songIni.RealBass22Tuning);
                if (songIni.RealKeysLaneCountRight != null) iniWriter.WriteKeyValuePair("real_keys_lane_count_right", songIni.RealKeysLaneCountRight.ToString());
                if (songIni.RealKeysLaneCountLeft != null) iniWriter.WriteKeyValuePair("real_keys_lane_count_left", songIni.RealKeysLaneCountLeft.ToString());
                if (songIni.Delay != null) iniWriter.WriteKeyValuePair("delay", songIni.Delay.ToString());
                if (songIni.SustainCutoffThreshold != null) iniWriter.WriteKeyValuePair("sustain_cutoff_threshold", songIni.SustainCutoffThreshold.ToString());
                if (songIni.HopoFrequency != null) iniWriter.WriteKeyValuePair("hopo_frequency", songIni.HopoFrequency.ToString());
                if (songIni.EighthNoteHopo != null) iniWriter.WriteKeyValuePair("eighthnote_hopo", songIni.EighthNoteHopo.ToString());
                if (songIni.MultiplierNote != null) iniWriter.WriteKeyValuePair("multiplier_note", songIni.MultiplierNote.ToString());
                if (songIni.MultiplierNote != null) iniWriter.WriteKeyValuePair("star_power_note", songIni.MultiplierNote.ToString());

                #endregion

                #region Chart Properties (Game-Specific)

                if (songIni.DrumFallbackBlue != null) iniWriter.WriteKeyValuePair("drum_fallback_blue", songIni.DrumFallbackBlue.ToString());
                if (songIni.Tutorial != null) iniWriter.WriteKeyValuePair("tutorial", songIni.Tutorial.ToString());
                if (songIni.BossBattle != null) iniWriter.WriteKeyValuePair("boss_battle", songIni.BossBattle.ToString());
                if (songIni.HopoFreq != null) iniWriter.WriteKeyValuePair("hopofreq", songIni.HopoFreq.ToString());
                if (songIni.EarlyHitWindowSize != null) iniWriter.WriteKeyValuePair("early_hit_window_size", songIni.EarlyHitWindowSize);
                if (songIni.EndEvents != null) iniWriter.WriteKeyValuePair("end_events", songIni.EndEvents.ToString());
                if (songIni.SysExSlider != null) iniWriter.WriteKeyValuePair("sysex_slider", songIni.SysExSlider.ToString());
                if (songIni.SysExHighHatCtrl != null) iniWriter.WriteKeyValuePair("sysex_high_hat_ctrl", songIni.SysExHighHatCtrl.ToString());
                if (songIni.SysExRimshot != null) iniWriter.WriteKeyValuePair("sysex_rimshot", songIni.SysExRimshot.ToString());
                if (songIni.SysExOpenBass != null) iniWriter.WriteKeyValuePair("sysex_open_bass", songIni.SysExOpenBass.ToString());
                if (songIni.SysExProSlide != null) iniWriter.WriteKeyValuePair("sysex_pro_slide", songIni.SysExProSlide.ToString());
                if (songIni.GuitarType != null) iniWriter.WriteKeyValuePair("guitar_type", songIni.GuitarType.ToString());
                if (songIni.BassType != null) iniWriter.WriteKeyValuePair("bass_type", songIni.BassType.ToString());
                if (songIni.KitType != null) iniWriter.WriteKeyValuePair("kit_type", songIni.KitType.ToString());
                if (songIni.KeysType != null) iniWriter.WriteKeyValuePair("keys_type", songIni.KeysType.ToString());
                if (songIni.DanceType != null) iniWriter.WriteKeyValuePair("dance_type", songIni.DanceType.ToString());

                #endregion

                #region Images and Other Resources

                if (songIni.Icon != null) iniWriter.WriteKeyValuePair("icon", songIni.Icon);
                if (songIni.Background != null) iniWriter.WriteKeyValuePair("background", songIni.Background);
                if (songIni.Video != null) iniWriter.WriteKeyValuePair("video", songIni.Video);
                if (songIni.VideoLoop != null) iniWriter.WriteKeyValuePair("video_loop", songIni.VideoLoop.ToString());
                if (songIni.VideoStartTime != null) iniWriter.WriteKeyValuePair("video_start_time", songIni.VideoStartTime.ToString());
                if (songIni.VideoEndTime != null) iniWriter.WriteKeyValuePair("video_end_time", songIni.VideoEndTime.ToString());
                if (songIni.Cover != null) iniWriter.WriteKeyValuePair("cover", songIni.Cover);

                #endregion

                #region Images and Other Resources (Game-Specific)

                if (songIni.LinkNameA != null) iniWriter.WriteKeyValuePair("link_name_a", songIni.LinkNameA);
                if (songIni.LinkNameB != null) iniWriter.WriteKeyValuePair("link_name_b", songIni.LinkNameB);
                if (songIni.BannerLinkA != null) iniWriter.WriteKeyValuePair("banner_link_a", songIni.BannerLinkA);
                if (songIni.BannerLinkB != null) iniWriter.WriteKeyValuePair("banner_link_b", songIni.BannerLinkB);

                #endregion

                #region Miscellaneous (Game-Specific)

                if (songIni.Scores != null) iniWriter.WriteKeyValuePair("scores", songIni.Scores);
                if (songIni.ScoresExt != null) iniWriter.WriteKeyValuePair("scores_ext", songIni.ScoresExt);
                if (songIni.Count != null) iniWriter.WriteKeyValuePair("count", songIni.Count.ToString());
                if (songIni.Rating != null) iniWriter.WriteKeyValuePair("rating", songIni.Rating.ToString());
                if (songIni.UnlockId != null) iniWriter.WriteKeyValuePair("unlock_id", songIni.UnlockId);
                if (songIni.UnlockRequire != null) iniWriter.WriteKeyValuePair("unlock_require", songIni.UnlockRequire);
                if (songIni.UnlockText != null) iniWriter.WriteKeyValuePair("unlock_text", songIni.UnlockText);
                if (songIni.UnlockCompleted != null) iniWriter.WriteKeyValuePair("unlock_completed", songIni.UnlockCompleted.ToString());
                if (songIni.EoFMidiImportDrumAccentVelocity != null) iniWriter.WriteKeyValuePair("eof_midi_import_drum_accent_velocity", songIni.EoFMidiImportDrumAccentVelocity.ToString());
                if (songIni.EoFMidiImportDrumGhostVelocity != null) iniWriter.WriteKeyValuePair("eof_midi_import_drum_ghost_velocity", songIni.EoFMidiImportDrumGhostVelocity.ToString());

                #endregion

                iniWriter.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred while writing '{path}'!");
                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Read a boolean value from an input string
        /// </summary>
        /// <param name="value">String value to parse</param>
        /// <returns>Nullable boolean value to output</returns>
        private static bool? ReadBool(string? value)
        {
            // If the value is invalid, always return null
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.ToLowerInvariant() switch
            {
                "true" => true,
                "1" => true,
                "false" => false,
                "0" => false,
                _ => null,
            };
        }

        /// <summary>
        /// Read a long value from an input string
        /// </summary>
        /// <param name="value">String value to parse</param>
        /// <returns>Nullable value value to output</returns>
        private static long? ReadLong(string? value)
        {
            // If the value is invalid, always return null
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (long.TryParse(value, out long longValue))
                return longValue;

            return null;
        }
    }
}