# KrxMusicStation
ASP.NET Core web interface for Kodi music database to provide a search by text functionality with Yamaha RN303d network receiver.

A web application to create playlists out of music stored on the server's (laptop's) disc.
The playlists are saved to folder, from which Yamaha device can playback - this mechanism is enabled via Kodi app
(which serves as local wi-fi media server).
Additionally it splits music files that are saved as one file with a cuesheet describing its tracks,
as Yamaha receiver cannot read the cursheet but single files.

# Search panel:
The krx app enables for easy search-by-text & create playlist functionality.
The search-result list is dynamically updated, previewed in ranges (e.g. 1-50, 51-100 etc.),
can be sorted per artist, album, song name, bitrate, duration etc.
Quality search criterium is included:
- lossless (more than 320 kbps),
- high quality (more than 192),
- any quality.
User can select the songs from the search results list and move them to the edited playlist.

# Playlist panel:
User can either edit the default playlist (blank from the start), or chose existing playlist for editing.
When finished, user can either save the edited playlist, or save as (new or some existing playlist).
During saving for one-file-per-album files it extracts single tracks from the single file and saves them to a temp folder,
the single files are then used by the playlists.
Otherwise Yamaha device is not able to playback single tracks from the one-file-per-album.

## User stories:

1. As a user I want to search music by text from existing Kodi music library
*create simple ASP.NET Core website
*connect to kodi sqlite music database using EntityFrameworkCore
*create a simple search page (search by any of: path, file name, artist, song, album)
*deploy the app on laptop and test

2. As a user I want the app to startup automatically when I launch my laptop
*deploy an autorun of the app

3. As a user I want to navigate the search results list in ranges, only the visible range is then saved as playlist

4. As a user I want the search result list be dynamically updated as I type

5. As a user I want to save the music displayed on the search results list as a default playlist, so I can play it back on Yamaha

6. As a user I want to search music and specify the quality I'm interested in
*add bitrate table to db
*add bitrate data using file size and song length
*at startup scan which songs dont have the bitrate data and add them

7. As a user I want to pick songs from the search results list and add them to edited playlist

8. As a user I want to save the edited playlist as a playlist with specific name (not default)

9. As a user I want to open existing playlist for editing

10. As a user I want to save playlist that include songs from the one-file-per-album files

11. As a user I want to be able to remove tracks from edited playlist

12. As a user I want to manually change order of songs on edited playlist
