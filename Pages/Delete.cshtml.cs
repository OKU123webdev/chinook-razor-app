using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ChinookContext;

namespace WebApp.Pages
{
    public class Delete : PageModel
    {
        // Store relevant details
        public String Heading { get; set; }
        public String AlbumTitle { get; set; }
        public String ArtistName { get; set; }
        public List<TrackDetails> TrackDetail { get; set; }
        public List<AlbumDetails> AlbumDetails { get; set; }
        public Int32 AlbumId { get; set; }

        // ............... ON GET ................
        public void OnGet(int albumID) // get album id from db
        {
            using (ChinookDatabase db = new ChinookDatabase()) // open db
            {
                // Get matching Album & Artist entities from ID
                AlbumDetails = db.Albums.Join(
                    db.Artists,
                    album => album.ArtistId,
                    artist => artist.ArtistId,
                    (album, artist) => new AlbumDetails
                    {
                        AlbumId = album.AlbumId,
                        Title = album.Title,
                        ArtistId = album.ArtistId,
                        Name = artist.Name
                    })
                    .Where(a => a.AlbumId == albumID)
                    .ToList(); // add results to list

                // Get tracks in album by matching id
                TrackDetail = db.Tracks.Where(t => t.AlbumId == albumID)
                    .Join(
                        db.Genres, track => track.GenreId, genre => genre.GenreId,
                        (track, genre) => new TrackDetails
                        {
                            TrackId = track.TrackId,
                            TrackName = track.Name,
                            GenreId = track.GenreId,
                            GenreName = genre.Name,
                            Composer = track.Composer,
                        }
                    ).ToList(); // add results to list

                // make sure TrackDetail not null
                if (TrackDetail == null)
                {
                    TrackDetail = new List<TrackDetails>();
                }

                // retrieve first album from list
                var album = AlbumDetails.FirstOrDefault();

                // if album found, extract details
                if (album != null)
                {
                    AlbumTitle = album.Title;
                    ArtistName = album.Name;
                    AlbumId = album.AlbumId;
                }
            }
        }

        // .................. ON POST ......................
        public IActionResult OnPost()
        {
            // open database
            ChinookDatabase db = new ChinookDatabase();

            // get album id from form
            int albumId = Int32.Parse(Request.Form["albumId"]);

            // DELETE TRACKS
            var tracksToUpdate = db.Tracks.Where(t => t.AlbumId == albumId).ToList(); // get tracks by album id
            foreach (var track in tracksToUpdate)
            {
                track.AlbumId = null;  // Disassociate track from album
            }
            db.SaveChanges();
            //var tracksAfter = db.Tracks.Where(t => t.AlbumId == albumId).ToList();

            // DELETE ALBUM
            var albumToDelete = db.Albums.SingleOrDefault(a => a.AlbumId == albumId); // get album by id
            if (albumToDelete != null) 
            {
                db.Albums.Remove(albumToDelete); // delete album from db
                db.SaveChanges();
            }

            // FINISH - REDIRECT TO INDEX
            return RedirectToPage("/Index");
        }
    }
}