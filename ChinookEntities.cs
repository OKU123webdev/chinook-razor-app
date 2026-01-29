public class Album
{
    public Int32 AlbumId { get; set; }
    public String Title { get; set; }
    public Int32 ArtistId { get; set; }
}

public class Artist
{
    public Int32 ArtistId { get; set; }
    public String Name { get; set; }
}

public class Genre
{
    public Int32 GenreId {get; set;}
    public String Name {get; set;}
}

public class Track
{
    public Int32 TrackId{get;set;}
    public string Name { get; set; }
    public Int32? AlbumId { get; set; }
    public Int32  MediaTypeId { get; set; }
    public Int32 GenreId { get; set; }
    public String Composer { get; set; }
    public Int32 Milliseconds { get; set; }
    public Int32 Bytes { get; set; }
    public Double UnitPrice { get; set; }
}

public class AlbumDetails
{
    public Int32 AlbumId {get; set;}
    public String Title {get; set;}
    public Int32 ArtistId {get; set;}
    public String Name {get; set;}
    /* public int? GenreId {get; set;}
    public String Genre {get; set;} */
}

public class TrackDetails{
    public String TrackName {get; set;}
    public Int32 AlbumId {get; set;}
    public String ArtistName {get; set;}
    public Int32 GenreId {get; set;}
    public String Composer {get; set;}
    public String GenreName {get; set;}
    public Int32 TrackId {get; set;}
}