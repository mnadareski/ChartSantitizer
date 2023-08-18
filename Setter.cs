namespace ChartSanitizer
{
    internal class Setter
    {
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
    }
}