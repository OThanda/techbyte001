using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.Practices.Unity;
using TechBytes.Web.Loggers;
using TechBytes.Web.Models;

namespace TechBytes.Web.Filters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        [Dependency]
        internal IRecorder MyRecorder { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ActionArguments.Any())
            {
                if (actionContext.ActionArguments.ContainsKey("isbn"))
                {                
                    var isbn = actionContext.ActionArguments["isbn"];
                    if (null != isbn)
                    {
                        MyRecorder.Write(String.Format("A request is made for a book with ISBN {0}", isbn));
                    }
                }               
                else if (actionContext.ActionArguments.ContainsKey("model"))
                {
                    var modelBook = actionContext.ActionArguments["model"];                   
                    var book = modelBook as Book;
                    if (null != book)
                    {
                        MyRecorder.Write(String.Format("A request is coming in as an object model with ISBN {0}", book.ISBN));
                    }
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            MyRecorder.Write(String.Format("LogActionFilter TimeStamp {0}", DateTime.Now));
            MyRecorder.Write("LogActionFilter OnActionExecuted Response " + actionExecutedContext.Response.StatusCode.ToString());

            var books = new List<Book>();

            var objectContent = actionExecutedContext.Response.Content as ObjectContent;
            if (objectContent != null)
            {
                var type = objectContent.ObjectType; //type of the returned object
                var value = objectContent.Value; //holding the returned value

                books = value as List<Book>;
            }

            var stringContent = actionExecutedContext.Response.Content as StringContent;
            if (stringContent != null)
            {
                books = stringContent.ReadAsAsync<List<Book>>().Result;
            }
                
            foreach (var book in books)
            {
                MyRecorder.Write(String.Format("Books POST Method with ISBN {0}", book.ISBN));
            }
        }
    }
}