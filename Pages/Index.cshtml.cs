using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ChinookContext;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace WebApp.Pages
{
    public class ViewAlbums : PageModel
    {
        // Store relevant details
        public String Heading { get; set; }
        public String CurrentGenre { get; set; }
        public String SearchQuery { get; set; }
        public List<AlbumDetails> AlbumDetails { get; set; }
        public List<Genre> Genres { get; set; }



        // ............... ON GET ................
        public void OnGet(int? genre, String search) // get genre/search input
        {
            Heading = "Chinook Music Store";

            // open db & get relevant details (as lists)
            ChinookDatabase db = new ChinookDatabase();
            List<Album> albums = db.Albums.ToList();
            List<Artist> artists = db.Artists.ToList();
            List<Track> tracks = db.Tracks.ToList();
            Genres = db.Genres.ToList();

            // start with all albums (no genre filter)
            CurrentGenre = null;

            // ........... SEARCH ..............
            SearchQuery = search; // user input
            // Search db for user input
            if (!String.IsNullOrEmpty(search))
            {
                String lowcaseSearch = search.ToLower();

                albums = albums.Where(album =>
                    album.Title.ToLower().Contains(lowcaseSearch) || // check album name
                    artists.Any(artist => artist.ArtistId == album.ArtistId &&
                        artist.Name.ToLower().Contains(lowcaseSearch)) // check artist name
                ).ToList(); // add results to list
            }

            // ........... GENRE FILTER ................
            if (genre.HasValue && genre.Value > 0) // check if genre selected
            {   
                // get albums from db with tracks containing selected genre id
                albums = albums.Where(
                    album => tracks.Any(
                        track => track.AlbumId == album.AlbumId && track.GenreId == genre.Value))
                        .ToList();
                Genre selectedGenre = Genres.FirstOrDefault(g => g.GenreId == genre.Value);
                // show filter message
                if (selectedGenre != null)
                {
                    CurrentGenre = $"Currently filtering by: {selectedGenre.Name}";
                }
            }

            // ................ TABLE DATA ...................
            // get album title and artist name by album id
            AlbumDetails = albums.Join(
                artists, album => album.ArtistId, artist => artist.ArtistId,
                (album, artist) => new AlbumDetails()
                {
                    AlbumId = album.AlbumId,
                    Title = album.Title,
                    ArtistId = artist.ArtistId,
                    Name = artist.Name
                    
                }).ToList(); // add results to list


            // ............. REORDER TABLE................
            String sortOrder = Request.Query["sortOrder"]; // check which order selected
            // ALBUM ACENDING
            if (sortOrder == "albumAsc")
            {
                AlbumDetails = AlbumDetails.OrderBy(a => a.Title).ToList();

            }
            // ALBUM DECENDING
            else if (sortOrder == "albumDesc")
            {
                AlbumDetails = AlbumDetails.OrderByDescending(a => a.Title).ToList();
            }
            // ARTIST DECENDING
            else if (sortOrder == "artistDesc")
            {
                AlbumDetails = AlbumDetails.OrderByDescending(a => a.Name).ToList();
            }
            // ARTIST ACSENDING
            else
            {
                AlbumDetails = AlbumDetails.OrderBy(a => a.Name).ToList();
            }

        }
    }
}