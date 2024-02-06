using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrasructure.Models.Helpers;
using issues.server.Infrastructure.Models.Helpers;
using issues.server.Infrastructure.Models.Response;
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

                string WhereClause = $@"WHERE t.projectid in (select id from projects p where p.companyid = {filter.CompanyID}) and t.title ilike '%{filter.Keyword}%'";
                string query_count = $@"Select Count(t.id) from issues t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT t.id, t.projectid, t.title, t.description, t.type, t.status, t.priority, t.createddate, t.enddate, t.isactive, u.id, u.firstname, u.lastname, i.*
                    FROM issues t
                    left join users u on u.id = t.createdby
                    left join users i on i.id in (select iau.userid from issueassignedusers iau where iau.issueid = t.id)
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    var issuesDictionary = new Dictionary<int, Issues>();
                    result.data = await con.QueryAsync<Issues, UserResponse, UserResponse, Issues>(query, (i, u, a) =>
                    {
                        i.CreatedBy = u ?? new UserResponse();
                        Issues issueEntry;

                        if (!issuesDictionary.TryGetValue(i.ID, out issueEntry))
                        {
                            issueEntry = i;
                            issueEntry.AssignedTo = new List<UserResponse>();
                            issuesDictionary.Add(issueEntry.ID, issueEntry);
                        }
                        issueEntry?.AssignedTo?.Add(a);
                        return i;
                    }, splitOn: "id");
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
                int type = (int)entity.Type;
                int state = (int)entity.Status;
                int priority = (int)entity.Priority;

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
	 	        VALUES ({identity}, {entity.Project.ID}, '{entity.Title}', '{entity.Description}', {type}, {state}, {priority}, {entity.CreatedBy.ID}, current_timestamp, null, true)
                ON CONFLICT (id) DO UPDATE 
                SET title = '{entity.Title}',
                      projectid = '{entity.Project.ID}',
                      description =  '{entity.Description}',
                      type =  {type},
                      status =  {state},
                      priority =  {priority},
                      createdby =  {entity.CreatedBy.ID}
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Issues>(query);
                    query = $@"DELETE from issueassignedusers where issueid = {res?.ID};";
                    await connection.QueryFirstOrDefaultAsync<int>(query);
                    foreach (var item in entity.AssignedTo)
                    {
                        query = $@"
                        INSERT INTO issueassignedusers (id, issueid, userid)
	 	                VALUES (default, {res?.ID}, {item.ID});";
                        await connection.QueryFirstOrDefaultAsync<UserResponse>(query);
                    }
                    res.CreatedBy = new UserResponse();
                    res.AssignedTo = new List<UserResponse>();
                    res.CreatedBy.ID = entity.CreatedBy.ID;
                    res.AssignedTo = entity.AssignedTo;
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
