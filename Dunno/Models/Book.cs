using System;
namespace Dunno.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Displacement { get; set; }
        public byte[] Image { get; set; }
    }
}
