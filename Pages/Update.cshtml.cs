using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ChinookContext;

namespace WebApp.Pages
{
    public class Update : PageModel
    {
        // Store relevant details
        public String Heading { get; set; }
        public Album Album { get; set; }
        public String ArtistName { get; set; }
        public Artist Artist { get; set; }
        public List<Track> Tracks { get; set; } = new List<Track>();
        public List<Genre> Genres {get; set;}

        // ............... ON GET ................
        public IActionResult OnGet(int albumId) // get album id from db
        {
            ChinookDatabase db = new ChinookDatabase(); // open db
            Genres = db.Genres.ToList(); // get genre list

            // Get album, artist name, and tracks by ID
            var album = db.Albums.Where(a => a.AlbumId == albumId)
                .Select(a => new
                {
                    Album = a,
                    ArtistName = db.Artists.Where(artist => artist.ArtistId == a.ArtistId)
                        .Select(artist => artist.Name).FirstOrDefault(),
                    Tracks = db.Tracks.Where(t => t.AlbumId == a.AlbumId).ToList()
                }).FirstOrDefault();

            if (album == null) // if album not found
            {
                return NotFound($"Album with ID {albumId} not found");
            }

            // extract details
            Album = album.Album;
            ArtistName = album.ArtistName;
            Tracks = album.Tracks;

            return Page();
        }

        // .................. ON POST ......................
        public IActionResult OnPost()
        {
            // GET RELEVANT DATABASE ENTITIES
            ChinookDatabase db = new ChinookDatabase();
            int albumId = int.Parse(Request.Form["albumId"]);
            var albumToUpdate = db.Albums.Single(a => a.AlbumId == albumId);
            var artistToUpdate = db.Artists.Single(a => a.ArtistId == albumToUpdate.ArtistId);
            var newTrackNames = Request.Form["tbxTracks[]"];
            var genreIds = Request.Form["tbxGenres[]"];
            var composerNames = Request.Form["tbxComposers[]"];


            // ............... UPDATE ALBUM TITLE ..................
            String newTitle = Request.Form["updAlbumTitle"]; // get user input
            // validation check
            if (string.IsNullOrWhiteSpace(newTitle) || newTitle.Length > 160)
            { // if validation error
                ViewData["ErrorMsg"] = "Please complete ALBUM Title (Max 160 characters)"; 
                return Page();
            } else { // no error - update album title
                albumToUpdate.Title = newTitle;
                db.Update(albumToUpdate);
            }

            // ................. UPDATE ARTIST NAME ...................
            String newArtist = Request.Form["updArtistName"]; // get user input
           // validation check
           if (string.IsNullOrWhiteSpace(newArtist) || newArtist.Length > 120){
                // if validation error
                ViewData["ErrorMsg"] = "Please complete ARTIST name (Max 120 characters)";
                return Page();
           } else { // no error - update artist name
                artistToUpdate.Name = newArtist;
                db.Update(artistToUpdate);
           }
            

            // .................... UPDATE TRACKS ...................
            var trackNames = Request.Form["trackNames"]; // get updated track names
            if (trackNames.Count > 0) 
            {
                // find track by track id in album
                var trackIds = db.Tracks.Where(t => t.AlbumId == albumId).Select(t => t.TrackId).ToList();
                for (int i = 0; i < trackIds.Count; i++) // loop through each track in album
                {   
                    // save each track update
                    var trackToUpdate = db.Tracks.Single(t => t.TrackId == trackIds[i]);
                    trackToUpdate.Name = trackNames[i];
                }
            }

            // ..................... DELETE ARTIST/TRACKS .......................
            // DELETE ARTIST
            var selectedArtist = Request.Form["deleteArtist"]; // get selected artist id
            if (!string.IsNullOrEmpty(selectedArtist) && selectedArtist == "true") // check if checkbox ticked
            {
                var artistToDelete = db.Artists.Single(a => a.ArtistId == albumToUpdate.ArtistId);
                artistToDelete.Name = "Unknown Artist"; // update name to Unknown
            }

            // DELETE TRACK
            var selectedTracks = Request.Form["deleteTracks"]; // get selected tracks
            if (!string.IsNullOrEmpty(selectedTracks))
            {
                var trackIds = selectedTracks.ToString().Split(',').Select(int.Parse).ToList(); // get individual selected track ids
                foreach (var trackId in trackIds) // loop through selected tracks
                {
                    var trackToDelete = db.Tracks.Single(t => t.TrackId == trackId);
                    trackToDelete.AlbumId = null; // set to null
                }
            }

            // ....................... ADD TRACKS ......................
            for (int i = 0; i < newTrackNames.Count; i++) // loop through each track
            {
                Track newTrack = new Track // add new entry to Tracks table
                {
                    Name = newTrackNames[i], // get track name
                    AlbumId = albumId,
                    MediaTypeId = 1,
                    GenreId = Convert.ToInt32(genreIds[i]), // get selected genre id
                    Composer = composerNames[i], // get composer name
                    Milliseconds = 0,
                    UnitPrice = 0.00
                };

                db.Tracks.Add(newTrack);
            }

            // Save all changes
            db.SaveChanges();

            // FINISH - REDIRECT TO INDEX
            return RedirectToPage(new { albumId = albumId });
        }
    }
}

