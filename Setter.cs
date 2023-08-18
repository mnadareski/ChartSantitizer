namespace ChartSanitizer
{
    internal class Setter
    {
        #region Clone Hero

        /// <summary>
        /// Set the playlist values from the album
        /// </summary>
        public bool PlaylistFromAlbum { get; set; }

        /// <summary>
        /// Set the name of the playlist
        /// </summary>
        public string? Playlist { get; set; }

        /// <summary>
        /// Set the name of the sub-playlist
        /// </summary>
        public string? SubPlaylist { get; set; }

        #endregion

        #region FoFiX

        /// <summary>
        /// Set the unlock ID of the song
        /// </summary>
        public string? UnlockId { get; set; }

        /// <summary>
        /// Set the unlock requirement of the song
        /// </summary>
        public string? UnlockRequire { get; set; }

        /// <summary>
        /// Set the unlock text of the song
        /// </summary>
        public string? UnlockText { get; set; }

        #endregion
    }
}