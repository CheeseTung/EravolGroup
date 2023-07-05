using Eravol.UIClient.Repositories.General;
using Eravol.UIClient.ViewModels.General;
using Eravol.WebApi.Data.Models;
using Eravol.WebApi.ViewModels.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Eravol.UIClient.Pages.PostStatuses.Clients
{
    public class IndexModel : PageModel
    {
        const string HTTP_GET = "GET";
        const string HTTP_PUT = "PUT";
        const string HTTP_POST = "POST";
        const string HTTP_DELETE = "DELETE";
        const string BASE_URL = "https://localhost:7259";
        string RELATIVE_PATH_URL = $"/api/PostStatuses";

        private readonly IClientsRequestService<PagingRequestBase<PostStatus>> requestService;
		private readonly IHttpClientFactory clientFactory;

		public IndexModel(IClientsRequestService<PagingRequestBase<PostStatus>> requestService, IHttpClientFactory clientFactory)
        {
            this.requestService = requestService;
			this.clientFactory = clientFactory;
        }
        [BindProperty(SupportsGet = true)] public PagingRequestBase<PostStatus>? PagingRequest { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			//get token by session
			string? token = HttpContext.Session.GetString("AuthToken");
			if (token == null)
			{
				return RedirectToPage("/Forbidden");
			}
			if (!ModelState.IsValid)
			{
				return Page();
			}
			string searchTermStr = (PagingRequest.SearchTerm != null) ? $"SearchTerm={PagingRequest.SearchTerm}" : "";
			string currentPageStr = $"CurrentPage={PagingRequest.CurrentPage}";
			string pageSizeStr = $"PageSize={PagingRequest.PageSize}";
			string hasNextStr = $"HasNext={PagingRequest.HasNext}";
			string hasPreStr = $"HasPrevious={PagingRequest.HasPrevious}";

			string Url = $"{RELATIVE_PATH_URL}?{searchTermStr}&{currentPageStr}&{pageSizeStr}&{hasNextStr}&{hasPreStr}";
			//Create new client to send request to api
			var client = clientFactory.CreateClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
			client.BaseAddress = new Uri(BASE_URL);
			HttpResponseMessage response = await client.GetAsync(Url);

			string dataResponse = await response.Content.ReadAsStringAsync();
			PagingRequest = JsonConvert.DeserializeObject<PagingRequestBase<PostStatus>>(dataResponse);
			return Page();
		}
	}
}