using System;
using Microsoft.AspNetCore.Http;

namespace Dunno.Models.Helpers
{
    public class NewsPost : News
    {
        public IFormFile Image { get; set; }
    }
}
