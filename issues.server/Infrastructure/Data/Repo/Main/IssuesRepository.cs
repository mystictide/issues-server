using Dapper;
using issues.server.Infrastructure.Models.Main;
using issues.server.Infrastructure.Models.Stats;
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
                string project = filter.ProjectID > 0 ? $"t.projectid = {filter.ProjectID}" : "t.projectid notnull";
                string type = filter.Type > 0 ? $"t.type = {filter.Type}" : "t.type notnull";
                string status = filter.Status > 0 ? $"t.status = {filter.Status}" : "t.status notnull";
                string priority = filter.Priority > 0 ? $"t.priority = {filter.Priority}" : "t.priority notnull";
                FilteredList<Issues> request = new FilteredList<Issues>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Issues> result = new FilteredList<Issues>();
                string WhereClause = $@"WHERE p.isactive = True and t.projectid in (select id from projects p where p.companyid = {filter.CompanyID}) and {project} and {type} and {status} and {priority} and t.title ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from issues t inner join projects p on p.id = t.projectid  {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT t.id, t.projectid, t.title, t.description, t.type, t.status, t.priority, t.createddate, t.enddate, t.isactive, u.id, u.firstname, u.lastname
                    FROM issues t
                    left join users u on u.id = t.createdby
                    inner join projects p on p.id = t.projectid 
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Issues, UserResponse, Issues>(query, (i, u) =>
                    {
                        i.CreatedBy = u ?? new UserResponse();
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
                SELECT t.id, t.createdBy as CreatedByID, t.projectid as ProjectID, t.title, t.description, t.type, t.status, t.priority, t.createddate, t.enddate, t.isactive
                FROM issues t
                WHERE t.id = {ID};";

                string aQuery = $@"
                Select *
                From users t
                WHERE t.id in (select i.userid from issueassignedusers i where i.issueid = {ID});";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Issues>(query);
                        string pQuery = $@"
                        Select *
                        From projects t
                        WHERE t.id = {res?.ProjectID};";
                        res.Project = await con.QueryFirstOrDefaultAsync<Projects>(pQuery);
                        string cQuery = $@"
                        Select *
                        From users t
                        WHERE t.id = {res?.CreatedByID};";
                        res.CreatedBy = await con.QueryFirstOrDefaultAsync<UserResponse>(cQuery);
                        res.AssignedTo = (List<UserResponse>?)await con.QueryAsync<UserResponse>(aQuery);
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

        public async Task<IEnumerable<Issues>?> GetCompanyIssues(int ID, int? limit)
        {
            try
            {
                string limited = limit.HasValue ? $"limit {limit}" : "";
                string query = $@"
                SELECT t.id, t.projectid, t.title, t.description, t.type, t.status, t.priority, t.createddate, t.enddate, t.isactive, u.id, u.firstname, u.lastname
                FROM issues t
                left join users u on u.id = t.createdby
                WHERE t.isactive = true and t.projectid in (select p.id from projects p where p.companyid = {ID}) {limited};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryAsync<Issues, UserResponse, Issues>(query, (i, u) =>
                           {
                               i.CreatedBy = u ?? new UserResponse();
                               return i;
                           }, splitOn: "id");
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

        public async Task<Issues?> ManageAssignedUsers(Issues entity)
        {
            try
            {
                string query = $@"DELETE from issueassignedusers where issueid = {entity?.ID};";
                using (var connection = GetConnection)
                {
                    await connection.QueryFirstOrDefaultAsync<int>(query);
                    foreach (var item in entity.AssignedTo)
                    {
                        query = $@"
                        INSERT INTO issueassignedusers (id, issueid, userid)
	 	                VALUES (default, {entity?.ID}, {item.ID});";
                        await connection.QueryFirstOrDefaultAsync<UserResponse>(query);
                    }
                    var res = await Get(entity.ID);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<int?> ManagePriority(int ID, int priority)
        {
            try
            {
                string query = $@"UPDATE issues SET priority =  {priority} where id = {ID}  RETURNING priority;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<int>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<int?> ManageStatus(int ID, int status)
        {
            try
            {
                string query = $@"UPDATE issues SET status =  {status} where id = {ID}  RETURNING status;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<int>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<int?> ManageType(int ID, int type)
        {
            try
            {
                string query = $@"UPDATE issues SET type = {type} where id = {ID} RETURNING type;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<int>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Comments?> GetComment(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM comments t
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Comments>(query);
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

        public async Task<Comments?> ManageComment(Comments entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";

                if (entity.Body.Contains("'"))
                {
                    entity.Body = entity.Body.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO comments (id, issueid, userid, body, createddate, isactive)
	 	        VALUES ({identity}, {entity.IssueID}, '{entity.User.ID}', '{entity.Body}', current_timestamp, true)
                ON CONFLICT (id) DO UPDATE 
                SET body = '{entity.Body}'
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Comments>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool?> DeleteComment(Comments entity)
        {
            try
            {
                string query = $@"DELETE from comments where id = {entity?.ID};";
                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<FilteredList<Comments>?> FilteredComments(Filter filter)
        {
            try
            {
                var filterModel = new Comments();
                FilteredList<Comments> request = new FilteredList<Comments>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Comments> result = new FilteredList<Comments>();
                string WhereClause = $@"WHERE t.issueid = {filter.IssueID} and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from comments t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT t.*, u.*
                    FROM comments t
                    left join users u on u.id = t.userid
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Comments, UserResponse, Comments>(query, (i, u) =>
                    {
                        i.User = u ?? new UserResponse();
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

        public async Task<IssueStats?> GetStatistics(int ID)
        {
            try
            {
                string query = $@"
                SELECT t.*,
                (select count(id) from issues bc where bc.projectid = t.id and bc.type = 1) as BugsCount,
                (select count(id) from issues bc where bc.projectid = t.id and bc.type = 2) as FeaturesCount,
                (select count(id) from issues bc where bc.projectid = t.id and bc.type = 3) as EnhancementsCount,
                (select count(id) from issues bc where bc.projectid = t.id and bc.type = 4) as TasksCount
                FROM projects t
                WHERE t.isactive = true and t.companyid = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<IssueStats>(query);
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
    }
}
