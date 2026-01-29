using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ChinookContext;

namespace WebApp.Pages
{
    public class Insert : PageModel
    {
        // Store relevant details
        public List<Genre> Genres { get; set; }

        // ............... ON GET ................
        public void OnGet()
        {
            // get genres from db
            ChinookDatabase db = new ChinookDatabase();
            Genres = db.Genres.ToList();

        }

        // .................. ON POST ......................
        public IActionResult OnPost()
        {
            // GET USER INPUTS
            String albumTitle = Request.Form["tbxAlbumTitle"];
            String artistName = Request.Form["tbxArtistName"];
            var trackNames = Request.Form["tbxTracks[]"];
            var genreIds = Request.Form["tbxGenres[]"];
            var composerNames = Request.Form["tbxComposers[]"];

            // VALIDATION
            if (string.IsNullOrWhiteSpace(albumTitle) ||
                string.IsNullOrWhiteSpace(artistName) ||
                trackNames.Any(t => string.IsNullOrWhiteSpace(t)))
            {
                ModelState.AddModelError(string.Empty, "Album title, artist name, and all track names must be filled in.");
                return Page(); // Stay on the form and show an error
            }

            // open database
            ChinookDatabase db = new ChinookDatabase();

            // ............... ADD ARTIST NAME ................
            // check if artist exsists
            var artist = db.Artists.FirstOrDefault(a => a.Name == artistName);
            if (artist == null)
            { // add artist to db
                artist = new Artist { Name = artistName };
                db.Artists.Add(artist);
                db.SaveChanges();
            }

            // ................. ADD ALBUM TITLE ................
            Album insAlbum = new Album
            {
                Title = albumTitle,
                ArtistId = artist.ArtistId // link to artist by id
            };

            db.Albums.Add(insAlbum); // add title to db
            db.SaveChanges();

            // ................... ADD TRACKS ...................
            for (int i = 0; i < trackNames.Count; i++) // loop through each track
            {
                Track newTrack = new Track // add new entry to Tracks table
                {
                    Name = trackNames[i], // get track name
                    AlbumId = insAlbum.AlbumId,
                    MediaTypeId = 1,
                    GenreId = Convert.ToInt32(genreIds[i]), // get selected genre id
                    Composer = composerNames[i], // get composer name
                    Milliseconds = 0,
                    UnitPrice = 0.00
                };

                db.Tracks.Add(newTrack); // add tracks to db
            }

            // SAVE & RETURN TO INDEX
            db.SaveChanges();
            return Redirect("~/Index");
        }
    }
}

