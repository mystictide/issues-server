using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Repo.Main
{
    public class ProjectsRepository : AppSettings, IProject
    {
        public async Task<bool> Archive(Projects entity)
        {
            try
            {
                string query = $@"
                UPDATE projects
                SET isactive = {entity.IsActive} 
                WHERE id = {entity.ID} 
                RETURNING isactive;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<FilteredList<Projects>?> FilteredList(Filter filter)
        {
            try
            {
                var filterModel = new Projects();
                FilteredList<Projects> request = new FilteredList<Projects>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Projects> result = new FilteredList<Projects>();
                string kw = "''";
                if (filter.Keyword != null)
                {
                    kw = $@"'%{filter.Keyword}%'";
                }

                string WhereClause = $@"WHERE t.companyid = {filter.CompanyID} and t.name ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from projects t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
                    FROM projects t
                    {WhereClause}
                    order by id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Projects>(query);
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Projects?> Get(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM projects t
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Projects>(query);
                        return res;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Projects>?> GetCompanyProjects(int ID, int? limit)
        {
            try
            {
                string limited = limit.HasValue ? $"limit {limit}" : "";
                string query = $@"
                SELECT *
                FROM projects t {limited}
                WHERE t.companyid = {ID} and isactive = True;";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryAsync<Projects>(query);
                        return res;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Projects?> Manage(Projects entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";

                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }
                if (entity.Description.Contains("'"))
                {
                    entity.Description = entity.Description.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO projects (id, companyid, assignedto, name, description, createddate, enddate, isactive)
	 	        VALUES ({identity}, {entity.CompanyID}, {entity.AssignedTo}, '{entity.Name}', '{entity.Description}', current_timestamp, null, true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}',
                      description =  '{entity.Description}'
                RETURNING *;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Projects>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
    }
}
