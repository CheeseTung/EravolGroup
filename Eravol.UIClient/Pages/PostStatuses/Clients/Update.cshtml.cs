﻿using Eravol.WebApi.Data.Models;
using Eravol.WebApi.ViewModels.Categories;
using Eravol.WebApi.ViewModels.PostStatuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using System.Text;

namespace Eravol.UIClient.Pages.PostStatuses.Clients
{
	public class UpdateModel : PageModel
	{
		const string BASE_URL = "https://localhost:7259";
		string RELATIVE_PATH_URL = $"/api/PostStatuses";
		const string HTTP_GET = "GET";
		const string HTTP_PUT = "PUT";
		const string HTTP_POST = "POST";
		const string ROLE_ADMIN = "admin";
		const string ROLE_MEMBER = "member";
		private readonly IHttpClientFactory clientFactory;
		private readonly HttpClient client = null;

		public UpdateModel(IHttpClientFactory clientFactory)
		{
			this.clientFactory = clientFactory;
			client = new HttpClient();
		}
		[BindProperty(SupportsGet = true)] public int? PostStatusId { get; set; }
		[BindProperty] public PostStatus? postStatus { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			var client = clientFactory.CreateClient();
			client.BaseAddress = new Uri(BASE_URL);
			string url = $"{RELATIVE_PATH_URL}/PostStatusId?PostStatusId={PostStatusId}";
			HttpResponseMessage response = await client.GetAsync(url);
			string dataResponse = await response.Content.ReadAsStringAsync();
			postStatus = JsonConvert.DeserializeObject<PostStatus>(dataResponse);
			return Page();
		}
		public async Task<IActionResult> OnPostAsync()
		{
			var client = clientFactory.CreateClient();
			client.BaseAddress = new Uri(BASE_URL);
			string url = $"{RELATIVE_PATH_URL}/{postStatus.PostStatusId}";
			var json = JsonConvert.SerializeObject(postStatus);
			if(postStatus.PostStatusName is null)
			{
				TempData["FailedMessage"] = "Update Falied! Post Status Name Can't Null";
				return Page();
			}
			UpdatePostStatusRequest updatePostStatusRequest = new UpdatePostStatusRequest()
			{
				PostStatusName = postStatus.PostStatusName,
				PostStatusDesc = postStatus.PostStatusDesc
			};
			//json = JsonConvert.SerializeObject(updatePostStatusRequest);

			var formData = new MultipartFormDataContent
			{
				{ new StringContent(updatePostStatusRequest.PostStatusName), "PostStatusName" },
				{ new StringContent(updatePostStatusRequest.PostStatusDesc), "PostStatusDesc" }
			};

			HttpResponseMessage response = await client.PutAsync(url, formData);
			if (response.IsSuccessStatusCode)
			{
				TempData["SuccessMessage"] = "Update Successfully!";
				return RedirectToPage("./Index");
			}
			else
			{
				TempData["FailedMessage"] = "Update Falied! Please Try Again";
				return RedirectToPage("./Index");
			}
		}
	}
}
