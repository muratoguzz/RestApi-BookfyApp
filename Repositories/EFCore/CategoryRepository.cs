﻿using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public sealed class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext context) :
            base(context)
        {

        }

        public void CreateOneCategory(Category category) => Create(category);

        public void DeleteOneCategory(Category category) => Delete(category);
     
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges) =>
             await FindAll(trackChanges)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

        public async Task<Category> GetOneCategoryByIdAsync(int id, bool trackChanges) =>
            await FindByCondition(c => c.CategoryId.Equals(id), trackChanges)
                .SingleOrDefaultAsync();

        public void UpdateOneCategory(Category category) => Update(category);   
    }
}