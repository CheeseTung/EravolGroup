﻿using Eravol.WebApi.Data.Models;
using Eravol.WebApi.ViewModels.Base;
using Eravol.WebApi.ViewModels.Categories;

namespace Eravol.WebApi.Repositories.Categories
{
    public interface IManageCategoryRepository
    {
		Task CreateCategoryAsync(Category category);
		Task<List<Category>> GetCategorySearchPaging(PagingRequestBase<Category> request);
    }
}
