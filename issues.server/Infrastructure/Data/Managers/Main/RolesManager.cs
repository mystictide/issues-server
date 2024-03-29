﻿using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Main;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Managers.Main
{
    public class RolesManager : AppSettings, IRole
    {
        private readonly IRole _repo;
        public RolesManager()
        {
            _repo = new RolesRepository();
        }

        public async Task<bool> Archive(Roles entity)
        {
            return await _repo.Archive(entity);
        }

        public async Task<FilteredList<Roles>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter);
        }

        public async Task<Roles?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<IEnumerable<Roles>?> GetCompanyRoles(int ID)
        {
            return await _repo.GetCompanyRoles(ID);
        }

        public async Task<Roles?> Manage(Roles entity)
        {
            return await _repo.Manage(entity);
        }
    }
}
