using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
using gbsitsec.Models;
using gbsitsec.ViewModel;
using Mailgun.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace gbsitsec.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration _Configuration { get; }
        private static string ApiKey;
        private static string Domain;
        private static string ListName;

        public HomeController(IConfiguration Configuration)
        {
            _Configuration = Configuration;
            ApiKey = _Configuration["MailGun:ApiKey"];
            Domain = _Configuration["MailGun:Domain"];
            ListName = _Configuration["MailGun:Listname"];
        }

        public ActionResult RemoveMailingList()
        {
            return Content(RemoveList().Content.ToString());
        }

        public static IRestResponse RemoveList()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}";
            request.AddParameter("list", "Don'tExistsLoanToAllah@gbsit.azurewebsites.net",
                                  ParameterType.UrlSegment);
            request.Method = Method.DELETE;
            return client.Execute(request);
        }

        public ActionResult ListMemberOfList()
        {
            return Content(ListMembers().Content.ToString());
        }

        public static IRestResponse ListMembers()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members/pages";
            request.AddParameter("list", ListName,
                                  ParameterType.UrlSegment);
            return client.Execute(request);
        }

        public ICollection<MailGunList> ListMembers(string listName)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members/pages";
            request.AddParameter("list", listName,
                                  ParameterType.UrlSegment);
            IRestResponse<List<MailGunList>> response = client.Execute<List<MailGunList>>(request);
            return response.Data;
        }

        public ActionResult RemoveMemberOfList()
        {
            return Content(RemoveListMember().Content.ToString());
        }

        public static IRestResponse RemoveListMember()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members/{member}";
            request.AddParameter("list", ListName,
                                  ParameterType.UrlSegment);
            request.AddParameter("member", "bar@example.com",
                                  ParameterType.UrlSegment);
            request.Method = Method.DELETE;
            return client.Execute(request);
        }

        // update a specific member info
        public ActionResult UpdateMemberOfList()
        {
            return Content(UpdateListMember().Content.ToString());
        }

        public static IRestResponse UpdateListMember()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members/{member}";
            request.AddParameter("list", ListName,
                                  ParameterType.UrlSegment);
            request.AddParameter("member", "sbhonhariya@accuratetechnologies.com",
                                  ParameterType.UrlSegment);
            request.AddParameter("subscribed", false);
            request.AddParameter("name", "Salim Bhonhariya");
            request.Method = Method.PUT;
            return client.Execute(request);
        }

        //POST /lists/<address>/members-- to Add a member to aspecific list
        //public ActionResult AddMemberToList()
        //{
        //    return Content(AddListMember().Content.ToString());
        //}

        //public static IRestResponse AddListMember()
        //{
        //    RestClient client = new RestClient();
        //    client.BaseUrl = new Uri("https://api.mailgun.net/v3");
        //    client.Authenticator =
        //        new HttpBasicAuthenticator("api",
        //                                    ApiKey);
        //    RestRequest request = new RestRequest();
        //    request.Resource = "lists/{list}/members";
        //    request.AddParameter("list", ListName,
        //                          ParameterType.UrlSegment);
        //    request.AddParameter("address", "mubina61@gmail.com");
        //    request.AddParameter("subscribed", true);
        //    request.AddParameter("name", "Mubina Salim");
        //    request.AddParameter("description", "Wife");
        //    request.AddParameter("vars", "{\"age\": 46}");
        //    request.Method = Method.POST;
        //    return client.Execute(request);
        //}
       [HttpGet]
        public ActionResult AddMemberToList()
        {
            return View(); 
        }
        [HttpPost]
        public ActionResult SaveMember(ItemListViewModel itemListViewModel)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/{list}/members";
            request.AddParameter("list", ListName,
                                  ParameterType.UrlSegment);

            request.AddParameter("address", itemListViewModel.address);
            request.AddParameter("subscribed", false);
            request.AddParameter("name", itemListViewModel.name);
            request.AddParameter("description", itemListViewModel.description);

            request.AddParameter("vars", "{\"age\": 26}");
            request.Method = Method.POST;
            try
            {
                client.Execute(request);
                ViewBag.AddedSuccessfully = "You will get an invitation to your given email soon";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View("AddMemberToList", itemListViewModel);
                throw;
            }
            return View();
        }


        // With View for it with multiple view models
        public IActionResult RetrievesMailingLists()
        {
            IEnumerable<MailGunList> mailGunLists = GetMailingLists();

            IList<MailGunItemViewModel> mailGunItemViewModels = new List<MailGunItemViewModel>();

            foreach (var item in mailGunLists)
            {
                var mailgunitemviewmodel = new MailGunItemViewModel
                {
                    Id = item.Id,
                    items = GetItemViewModelAttributes(item),
                    first = item.paging.first,
                    next = item.paging.next,
                    last = item.paging.last,
                    previous = item.paging.previous
                };
                mailGunItemViewModels.Add(mailgunitemviewmodel);
            }
            return View(mailGunItemViewModels);
        }

        public IActionResult GetGunMailLists()
        {
            IEnumerable<MailGunList> mailGunLists = GetMailingLists();

            IList<MailGunItemViewModel> mailGunItemViewModels = new List<MailGunItemViewModel>();

            foreach (var item in mailGunLists)
            {
                var mailgunitemviewmodel = new MailGunItemViewModel
                {
                    Id = item.Id,
                    items = GetItemViewModelAttributes(item),
                    first = item.paging.first,
                    next = item.paging.next,
                    last = item.paging.last,
                    previous = item.paging.previous
                };
                mailGunItemViewModels.Add(mailgunitemviewmodel);
            }
            return View(mailGunItemViewModels);
        }

        private List<ItemListViewModel> GetItemViewModelAttributes(MailGunList item)
        {
            List<ItemListViewModel> itemListViewModels = new List<ItemListViewModel>();

            foreach (var itemItem in item.items)
            {
                ItemListViewModel itemListViewModel = new ItemListViewModel
                {
                    access_level = itemItem.access_level,
                    address = itemItem.address,
                    created_at = itemItem.created_at,
                    description = itemItem.description,
                    members_count = itemItem.members_count,
                    name = itemItem.name
                };

                itemListViewModels.Add(itemListViewModel);
            };

            return itemListViewModels;
        }

        public ICollection<MailGunList> GetMailingLists()
        {
            IList<MailGunList> mailGunLists = new List<MailGunList>();
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/pages";
            // var result =  client.Execute<MailGunList>(request);
            IRestResponse<List<MailGunList>> response = client.Execute<List<MailGunList>>(request);

            return response.Data;
        }

        //Create a new
        public ICollection<MailGunList> GetEmailsFromMailingLists(string address)
        {
            IList<MailGunList> mailGunLists = new List<MailGunList>();
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.Resource = "lists/" + address;
            // var result =  client.Execute<MailGunList>(request);
            IRestResponse<List<MailGunList>> response = client.Execute<List<MailGunList>>(request);

            return response.Data;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<SendResponse> SendRegistrationEmail()
        {
            var sender = new MailgunSender(
                    "sandbox32e99c74ce19453196efca5157abe0f0.mailgun.org", // Mailgun Domain
                    ApiKey // Mailgun API Key
            );
            Email.DefaultSender = sender;

            string fromEmail = "salimbhonhariya@gmail.com";
            string toEmail = "sbhonhariya@accuratetechnologies.com";
            string subject = "LoanToAllah Email Registration";
            string body = "Please confirm the email so you can be a part of Allah's work.";
            var email = Email
                .From(fromEmail)
                .To(toEmail)
                .Subject(subject)
                .Body(body);
            return await email.SendAsync();
        }

        public async Task<HttpResponseMessage> SendGroupEmail()
        {
            //var mailGunLists = GetMailingLists();
            var mg = new MessageService(ApiKey);
            var listMembers = ListMembers(ListName);

            foreach (var member in listMembers)
            {
                foreach (var item1 in member.items)
                {
                    // await SendMessage(item1.address);
                    IRestResponse a = await SendHTMLComplexMessage(item1.address);
                   string result =  a.Content.ToString();
                }
            }
            return new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.Accepted };
        }

        // send it to listemail and every member will get it, but
        // sandbox32e99c74ce19453196efca5157abe0f0.mailgun.org is only for testing , we need verified domain to send real.
        public IRestResponse SendGroupMessage()
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <sandboxtestlist@>");
            // request.AddParameter("to", "bar@example.com");
            request.AddParameter("to", "Enter the name of list");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.Method = Method.POST;
            IRestResponse a = client.Execute(request);
            return a;
        }

        public ActionResult SendMessagetoMultipleRecipients()
        {
            return Content(SendGroupMessage().Content.ToString());
        }

        public async Task<IRestResponse> SendMessage(string emailaddress)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <LoanToAllah@gbsit.azurewebsites.net> ");
            //request.AddParameter("to", "bar@example.com");
            request.AddParameter("to", emailaddress);
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("o:tracking", true);
            request.Method = Method.POST;
            return client.Execute(request);
        }

        public async Task<IRestResponse> SendHTMLComplexMessage(string emailaddress)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            ApiKey);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Excited User <LoanToAllah@gbsit.azurewebsites.net>");
            request.AddParameter("to", emailaddress);
            //request.AddParameter("cc", "baz@example.com");
            //request.AddParameter("bcc", "bar@example.com");
            request.AddParameter("subject", "Hello");
            request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("o:tracking", true);
            request.AddParameter("html",
                                  "<html>" +
                                  "<h1>HTML version of the body</h1>" +
                                  "<br/>" +
                                  "<h2>HTML version of the body</h2>" +
                                  "<br/>" +
                                  "<h3>HTML version of the body</h3>" +
                                  "<br/>" +
                                  "<p>TestTest" +
                                  "TestTestTestTestTestTestTestTestTestTestTestTestTestTest" +
                                  "TestTestTestTestTestTestTestTestTestTestTestTestTestTest" +
                                  "</p>" +
                                  "</html>");
            request.AddFile("attachment", Path.Combine("files", "test.png"));
            request.AddFile("attachment", Path.Combine("files", "test.txt"));
            request.Method = Method.POST;
            return client.Execute(request);
        }
    }
}