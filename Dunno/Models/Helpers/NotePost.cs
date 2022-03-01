using System;
using Microsoft.AspNetCore.Http;

namespace Dunno.Models.Helpers
{
    public class NotePost: Note
    {
        public IFormFile Image { get; set; }
    }
}
