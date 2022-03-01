using System;
using Microsoft.AspNetCore.Http;

namespace Dunno.Models.Helpers
{
    public class BookPost : Book
    {
        public IFormFile Image { get; set; }
    }
}
