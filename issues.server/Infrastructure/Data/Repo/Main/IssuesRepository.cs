using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Data.Repo.Helpers;
using issues.server.Infrastructure.Data.Interface.Main;

namespace issues.server.Infrastructure.Data.Repo.Main
{
    public class IssuesRepository : AppSettings, IIssue
    {
        public async Task<bool> Archive(Issues entity)
        {
            try
            {
                string query = $@"
                UPDATE issues
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

        public async Task<FilteredList<Issues>?> FilteredList(Filter filter)
        {
            try
            {
                var filterModel = new Issues();
                FilteredList<Issues> request = new FilteredList<Issues>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Issues> result = new FilteredList<Issues>();
                string kw = "''";
                if (filter.Keyword != null)
                {
                    kw = $@"'%{filter.Keyword}%'";
                }

                string WhereClause = $@"WHERE t.companyid = {filter.CompanyID} and t.name ilike '%{filter.Keyword}%'";
                string query_count = $@"Select Count(t.id) from issues t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
                    FROM issues t
                    {WhereClause}
                    order by id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Issues>(query);
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

        public async Task<Issues?> Get(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM issues t
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Issues>(query);
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

        public async Task<Issues?> Manage(Issues entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";

                if (entity.Title.Contains("'"))
                {
                    entity.Title = entity.Title.Replace("'", "''");
                }
                if (entity.Description.Contains("'"))
                {
                    entity.Description = entity.Description.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO issues (id, projectid, title, description, type, status, priority, createdby, createddate, enddate, isactive)
	 	        VALUES ({identity}, {entity.Project.ID}, '{entity.Title}', '{entity.Description}', {entity.Type}, {entity.Status}, {entity.Priority}, {entity.CreatedBy}, {entity.AssignedTo}, current_timestamp, null, true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Title}',
                      description =  '{entity.Description}',
                      type =  {entity.Type},
                      status =  {entity.Status},
                      priority =  {entity.Priority},
                      createdby =  {entity.CreatedBy}
                RETURNING *;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Issues>(query);
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
