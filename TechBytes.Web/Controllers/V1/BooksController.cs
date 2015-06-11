using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using TechBytes.Web.Models;

namespace TechBytes.Web.Controllers.V1
{
    public class BooksController : ApiController
    {
        public HttpResponseMessage Get(string isbn)
        {
            var books = new List<Book>();
            if (string.IsNullOrWhiteSpace(isbn))
            {
                books.AddRange(GetAll());
            }
            else
            {
                var book = from b in GetAll()
                           where b.ISBN == isbn
                           select b;

                books.AddRange(book);
            }

            var serializedData = JsonConvert.SerializeObject(books);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedData)
            };
        }

        private List<Book> GetAll()
        {
            var books = new List<Book>();
            books.Add(new Book()
            {
                ISBN = "1",
                Title = "ASP.NET",
                Author = "AAA"
            });
            books.Add(new Book()
            {
                ISBN = "2",
                Title = "ASP.NET MVC",
                Author = "BBB"
            });
            books.Add(new Book()
            {
                ISBN = "3",
                Title = "ASP.NET Web Api",
                Author = "CCC"
            });

            return books;
        }
    }
}
